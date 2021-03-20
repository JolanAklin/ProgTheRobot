using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Language;
using System;

public class NodeCommande : Nodes
{
    private string input;
    public TMP_InputField inputField;

    new private void Awake()
    {
        base.Awake();
        Manager.instance.OnLanguageChanged += TranslateText;
        ExecManager.onChangeBegin += LockAllInput;
    }

    private void OnDestroy()
    {
        Manager.instance.OnLanguageChanged -= TranslateText;
        ExecManager.onChangeBegin -= LockAllInput;
    }

    public void LockAllInput(object sender, ExecManager.onChangeBeginEventArgs e)
    {
        inputField.interactable = !e.started;
    }

    public void ChangeInput(TMP_InputField tMP_InputField)
    {
        inputField = tMP_InputField;
        input = tMP_InputField.text;
        if (!ValidateInput())
        {
            nodeErrorCode = ErrorCode.wrongInput;
            ChangeBorderColor(errorColor);
            Manager.instance.canExecute = false;
            Debugger.LogError("Commande inconnue");
            return;
        }
        nodeErrorCode = ErrorCode.ok;
        Manager.instance.canExecute = true;
        ChangeBorderColor(defaultColor);
    }


    private bool ValidateInput()
    {
        switch (input)
        {
            case "":
            case "Avancer":
            case "GoForward":
            case "TournerADroite":
            case "TurnRight":
            case "TournerAGauche":
            case "TurnLeft":
            case "Marquer":
            case "Mark":
            case "Demarquer":
            case "Unmark":
            case "Recharger":
            case "Reload":
            case "PoserBallon":
            case "PlaceBall":
            case "PrendreBallon":
            case "TakeBall":
            case "LancerBallon":
            case "ThrowBall":
                TranslateText(this, EventArgs.Empty);
                return true;
        }
        return false;
    }

    public void TranslateText(object sender, EventArgs e)
    {
        switch (input)
        {
            case "":
                break;
            case "Avancer":
            case "GoForward":
                input = Translation.Get("GoForward");
                break;
            case "TournerADroite":
            case "TurnRight":
                input = Translation.Get("TurnRight");
                break;
            case "TournerAGauche":
            case "TurnLeft":
                input = Translation.Get("TurnLeft");
                break;
            case "Marquer":
            case "Mark":
                input = Translation.Get("Mark");
                break;
            case "Demarquer":
            case "Unmark":
                input = Translation.Get("Unmark");
                break;
            case "Recharger":
            case "Reload":
                input = Translation.Get("Reload");
                break;
            case "PoserBallon":
            case "PlaceBall":
                input = Translation.Get("PlaceBall");
                break;
            case "PrendreBallon":
            case "TakeBall":
                input = Translation.Get("TakeBall");
                break;
            case "LancerBallon":
            case "ThrowBall":
                input = Translation.Get("Throwball");
                break;
        }
        inputField.text = input;
    }

    public override void Execute()
    {
        if (!ExecManager.Instance.isRunning)
            return;
        ChangeBorderColor(currentExecutedNode);

        switch (input)
        {
            case "":
                break;
            case "Avancer":
            case "GoForward":
                rs.robot.robotManager.GoForward(() => { StartCoroutine("WaitBeforeCallingNextNode"); }, noPower);
                break;
            case "TournerADroite":
            case "TurnRight":
                rs.robot.robotManager.TurnRight(() => { StartCoroutine("WaitBeforeCallingNextNode"); }, noPower);
                break;
            case "TournerAGauche":
            case "TurnLeft":
                rs.robot.robotManager.TurnLeft(() => { StartCoroutine("WaitBeforeCallingNextNode"); }, noPower);
                break;
            case "Marquer":
            case "Mark":
                rs.robot.robotManager.Mark();
                break;
            case "Demarquer":
            case "Unmark":
                rs.robot.robotManager.Unmark();
                break;
            case "Recharger":
            case "Reload":
                rs.robot.robotManager.Reload();
                break;
            case "PoserBallon":
            case "PlaceBall":
                rs.robot.robotManager.PlaceBall();
                break;
            case "PrendreBallon":
            case "TakeBall":
                rs.robot.robotManager.TakeBall();
                break;
            case "LancerBallon":
            case "ThrowBall":
                rs.robot.robotManager.ThrowBall();
                break;

            default:
                Debugger.LogError("Commande inconnue");
                break;
        }
    }

    // get called by the robotmanager when an action require more power than the robot has
    private void noPower()
    {
        ExecManager.Instance.StopExec();
        rs.End();
        ChangeBorderColor(defaultColor);
        Debugger.Log($"Le robot {rs.robot.robotName} n'a plus assez d'�nergie");
        Debug.Log("there");
    }

    IEnumerator WaitBeforeCallingNextNode()
    {
        if (!ExecManager.Instance.debugOn)
        {
            yield return new WaitForSeconds(executedColorTime / Manager.instance.execSpeed);
            ChangeBorderColor(defaultColor);
            CallNextNode();
        }
        else
        {
            ExecManager.Instance.buttonNextAction = () => {
                CallNextNode();
                ChangeBorderColor(defaultColor);
            };

        }
    }

    public override void CallNextNode()
    {
        if (NodesDict.ContainsKey(nextNodeId))
            NodesDict[nextNodeId].Execute();
    }

    public override void PostExecutionCleanUp(object sender, EventArgs e)
    {
        ChangeBorderColor(defaultColor);
    }

    #region save stuff
    public override SerializableNode SerializeNode()
    {
        SerializableNode serializableNode = new SerializableNode() {
            id = id,
            nextNodeId = nextNodeId,
            type = "execute",
            position = new float[] { transform.position.x, transform.position.y, transform.position.z },
            nodeSettings = new List<string>(),
            size = new float[] { canvas.sizeDelta.x, canvas.sizeDelta.y },
        };
        serializableNode.nodeSettings.Add(input);
        return serializableNode;
    }
    public override void DeSerializeNode(SerializableNode serializableNode)
    {
        id = serializableNode.id;
        nextNodeId = serializableNode.nextNodeId; //this is the next node in the execution order
        input = serializableNode.nodeSettings[0];
        inputField.text = input;
        Resize(new Vector2(serializableNode.size[0], serializableNode.size[1]));
    }
    #endregion
}
