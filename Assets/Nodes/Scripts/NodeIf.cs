using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;
using System.Data;

public class NodeIf : Nodes
{
    private string input;
    private string[] inputSplited;
    public int nextNodeIdFalse;

    public TMP_InputField inputField;

    new private void Awake()
    {
        base.Awake();
        ExecManager.onChangeBegin += LockAllInput;
    }

    public void OnDestroy()
    {
        ExecManager.onChangeBegin -= LockAllInput;
    }

    public void LockAllInput(object sender, ExecManager.onChangeBeginEventArgs e)
    {
        inputField.interactable = !e.started;
    }

    public void ChangeInput(TMP_InputField tMP_InputField)
    {
        input = tMP_InputField.text;
        if (!ValidateInput())
        {
            nodeErrorCode = ErrorCode.wrongInput;
            ChangeBorderColor(errorColor);
            Manager.instance.canExecute = false;
            return;
        }
        nodeErrorCode = ErrorCode.ok;
        Manager.instance.canExecute = true;
        ChangeBorderColor(defaultColor);
    }

    private bool ValidateInput()
    {
        //string[] delimiters = new string[] { "=", "<", ">", ">=", "<=", "<>" };
        //inputSplited = input.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        //if(inputSplited.Length == 1)
        //{
        //    switch(inputSplited[0])
        //    {
        //        case "MurEnFace":
        //        case "WallInFront":
        //        case "MurADroite":
        //        case "WallRight":
        //        case "MurAGauche":
        //        case "WallLeft":
        //        case "Sorti":
        //        case "Out":
        //        case "RobotSurUnePrise":
        //        case "RobotOnAnOutlet":
        //        case "CaseMarqué":
        //        case "TileMarked":
        //        case "CaseDevantOccupée":
        //        case "TileInFrontOccupied":
        //        case "BallonSurLeSol":
        //        case "BallOnTheGround":
        //        case "Vrai":
        //        case "True":
        //        case "Faux":
        //        case "False":
        //            return true;
        //    }
        //}
        //if (inputSplited.Length > 2 || inputSplited.Length == 1)
        //    return false;
        return true;
    }

    public override void Execute()
    {
        // test if the robot has enough power to execute the node, if not he stop the code execution
        if (rs.robot.power <= nodeExecPower)
        {
            ExecManager.Instance.StopExec();
            rs.End();
            ChangeBorderColor(defaultColor);
            Debugger.Log($"Le robot {rs.robot.robotName} n'a plus assez d'énergie");
            return;
        }
        rs.robot.power -= nodeExecPower;

        if (!ExecManager.Instance.isRunning)
            return;
        ChangeBorderColor(currentExecutedNode);

        IEnumerator coroutine = WaitBeforeCallingNextNode(nextNodeIdFalse);

        VarsManager.Evaluation eval = rs.robot.varsManager.Evaluate(input);

        if (!eval.error)
            if (eval.result)
            {
                StartCoroutine("WaitBeforeCallingNextNode");
            }
            else
            {
                StartCoroutine(coroutine);
            }
        else
            Debugger.LogError("Une erreur est seurvenue durant l'évaluation de l'expression");
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

    IEnumerator WaitBeforeCallingNextNode(int nodeId)
    {
        if (!ExecManager.Instance.debugOn)
        {
            yield return new WaitForSeconds(executedColorTime / Manager.instance.execSpeed);
            ChangeBorderColor(defaultColor);
            NodesDict[nodeId].Execute();
        }
        else
        {
            ExecManager.Instance.buttonNextAction = () => {
                NodesDict[nodeId].Execute();
                ChangeBorderColor(defaultColor);
            };
            ExecManager.Instance.ShowVar();
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
        SerializableNode serializableNode = new SerializableNode()
        {
            id = id,
            nextNodeId = nextNodeId,
            parentId = parentId,
            type = "test",
            position = new float[] { transform.position.x, transform.position.y, transform.position.z },
            nodeSettings = new List<string>(),
            size = new float[] { canvasRect.sizeDelta.x, canvasRect.sizeDelta.y },

        };
        serializableNode.nodeSettings.Add(input);
        serializableNode.nodeSettings.Add(nextNodeIdFalse.ToString());
        return serializableNode;
    }
    public override void DeSerializeNode(SerializableNode serializableNode)
    {
        id = serializableNode.id;
        nextNodeId = serializableNode.nextNodeId; //this is the next node in the execution order
        parentId = serializableNode.parentId;
        input = serializableNode.nodeSettings[0];
        inputField.text = input;
        nextNodeIdFalse = Convert.ToInt32(serializableNode.nodeSettings[1]);
        Resize(new Vector2(serializableNode.size[0], serializableNode.size[1]));
    }
    #endregion
}
