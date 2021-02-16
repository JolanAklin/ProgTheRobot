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
        throw new System.NotImplementedException();
    }

    public override void CallNextNode()
    {
        if (NodesDict.ContainsKey(nextNodeId))
            NodesDict[nextNodeId].Execute();
    }

    public override void PostExecutionCleanUp()
    {
        throw new System.NotImplementedException();
    }
}
