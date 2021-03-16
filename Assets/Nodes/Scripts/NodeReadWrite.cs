using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Language;

public class NodeReadWrite : Nodes
{
    private string input;
    public TMP_InputField inputField;
    private string[] inputSplited;

    public void ChangeInput(TMP_InputField tMP_InputField)
    {
        input = tMP_InputField.text;
        inputField = tMP_InputField;
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

    private bool ValidateInput()
    {
        string[] delimiters = new string[] { " " };
        inputSplited = input.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        if (inputSplited.Length > 2 || inputSplited.Length == 1)
            return false;
        if(inputSplited.Length != 0)
            switch (inputSplited[0])
            {
                case "Afficher":
                case "Display":
                case "Lire":
                case "Read":
                    if(VarsManager.CheckVarName(inputSplited[1]))
                    {
                        TranslateText(this, EventArgs.Empty);
                        return true;
                    }
                    return false;

                default:
                    return false;
            }
        return true;
    }

    public void TranslateText(object sender, EventArgs e)
    {
        if (Translation.CurrentLanguage == "eng")
        {
            input = input.Replace("Afficher", "Display");
            input = input.Replace("Lire", "Read");

        }
        if (Translation.CurrentLanguage == "fr")
        {
            input = input.Replace("Display", "Afficher");
            input = input.Replace("Read", "Lire");
        }
        inputField.text = input;
    }

    public override void Execute()
    {
        if (!ExecManager.Instance.isRunning)
            return;
        ChangeBorderColor(currentExecutedNode);

        string[] delimiters = new string[] { " " };
        inputSplited = input.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        PopUpReadWrite rw = WindowsManager.InstantiateWindow((int)Enum.Parse(typeof(WindowsManager.popUp), "readWrite"), Manager.instance.canvas.transform).GetComponent<PopUpReadWrite>();
        switch (inputSplited[0])
        {
            case "Afficher":
            case "Display":
                rw.Init($"Affichage de {inputSplited[1]}", rs.robot.varsManager.GetVar(inputSplited[1]).Value.ToString());
                rw.SetOkAction(() => { 
                    rw.DestroyPopup();
                    StartCoroutine("WaitBeforeCallingNextNode");
                });
                break;
            case "Lire":
            case "Read":
                rw.Init($"Lecture de {inputSplited[1]}");
                rw.SetOkAction(() => {
                    VarsManager.Var var = rs.robot.varsManager.GetVar(inputSplited[1],0);
                    var.Value = rw.value();
                    var.Persist();
                    rw.DestroyPopup();
                    StartCoroutine("WaitBeforeCallingNextNode");
                });
                break;
        }
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
        ChangeBorderColor(defaultColor);
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
            type = "readWrite",
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
