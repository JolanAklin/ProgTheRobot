using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Language;

public class NodeReadWrite : Nodes
{
    private string input;
    private TMP_InputField inputField;
    private string[] inputSplited;

    public void ChangeInput(TMP_InputField tMP_InputField)
    {
        input = tMP_InputField.text;
        inputField = tMP_InputField;
        if (!ValidateInput())
        {
            ChangeBorderColor(errorColor);
            Manager.instance.canExecute = false;
            return;
        }
        Manager.instance.canExecute = true;
        ChangeBorderColor(defaultColor);
    }

    new private void Awake()
    {
        base.Awake();
        Manager.instance.OnLanguageChanged += TranslateText;
    }

    private void OnDestroy()
    {
        Manager.instance.OnLanguageChanged -= TranslateText;
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

    public override void SerializeNode()
    {
        throw new System.NotImplementedException();
    }
    public override void DeSerializeNode()
    {
        throw new System.NotImplementedException();
    }
    public override void Execute()
    {
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
                    CallNextNode();
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
                    CallNextNode();
                });
                break;
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
        Debug.Log("node read write clean up do nothing");
    }
}
