using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Language;

public class NodeWhileLoop : Nodes
{
    private string input;
    private string[] inputSplited;
    private TMP_InputField inputField;

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
        string[] delimiters = new string[] { "=", "<", ">", ">=", "<=", "<>" };
        inputSplited = input.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        if (inputSplited.Length > 2 || inputSplited.Length == 1)
            return false;
        delimiters = new string[] { "While", "TantQue" };
        string[] inputSplitedFirstPart = inputSplited[0].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        if (inputSplitedFirstPart.Length > 1 || inputSplitedFirstPart[0] == "TantQue" || inputSplitedFirstPart[0] == "While")
            return false;
        TranslateText(this, EventArgs.Empty);
        return true;
    }

    private void TranslateText(object sender, EventArgs e)
    {
        if (Translation.CurrentLanguage == "eng")
        {
            input = input.Replace("TantQue", "While");
        }
        if (Translation.CurrentLanguage == "fr")
        {
            input = input.Replace("While", "TantQue");
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
