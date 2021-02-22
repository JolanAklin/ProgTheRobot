using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Language;
using System.Data;

public class NodeWhileLoop : Nodes
{
    private string input;
    private string[] inputSplited;
    private TMP_InputField inputField;

    public int nextNodeInside = -1;

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
        if (!(inputSplited[0].IndexOf("TantQue") == 0 || inputSplited[0].IndexOf("While") == 0))
            return false;
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
        string[] delimiters = new string[] { " ", "While", "TantQue" };
        string[] expressionSplited1 = inputSplited[0].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        string expr = string.Join("", rs.robot.varsManager.ReplaceStringsByVar(expressionSplited1));
        if (expr == null)
        {
            Debugger.Log("Variable inconnue");
            ChangeBorderColor(errorColor);
            return;
        }
        int value1 = Convert.ToInt32(new DataTable().Compute(expr, null));
        //expression = inputSplited[1].Replace(" ", string.Empty).Trim();
        string[] expressionSplited2 = inputSplited[1].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        expr = string.Join("", rs.robot.varsManager.ReplaceStringsByVar(expressionSplited2));
        if (expr == null)
        {
            Debugger.Log("Variable inconnue");
            ChangeBorderColor(errorColor);
            return;
        }
        int value2 = Convert.ToInt32(new DataTable().Compute(expr, null));


        if (input.Contains("="))
        {
            if (value1 == value2)
            {
                if (NodesDict.ContainsKey(nextNodeInside))
                    NodesDict[nextNodeInside].Execute();
            }
            else
            {
                if (NodesDict.ContainsKey(nextNodeId))
                    NodesDict[nextNodeId].Execute();
            }

        }
        else if (input.Contains("<"))
        {
            if (value1 < value2)
            {
                if (NodesDict.ContainsKey(nextNodeInside))
                    NodesDict[nextNodeInside].Execute();
            }
            else
            {
                if (NodesDict.ContainsKey(nextNodeId))
                    NodesDict[nextNodeId].Execute();
            }
        }
        else if (input.Contains(">"))
        {
            if (value1 > value2)
            {
                if (NodesDict.ContainsKey(nextNodeInside))
                    NodesDict[nextNodeInside].Execute();
            }
            else
            {
                if (NodesDict.ContainsKey(nextNodeId))
                    NodesDict[nextNodeId].Execute();
            }
        }
        else if (input.Contains(">="))
        {
            if (value1 >= value2)
            {
                if (NodesDict.ContainsKey(nextNodeInside))
                    NodesDict[nextNodeInside].Execute();
            }
            else
            {
                if (NodesDict.ContainsKey(nextNodeId))
                    NodesDict[nextNodeId].Execute();
            }
        }
        else if (input.Contains("<="))
        {
            if (value1 <= value2)
            {
                if (NodesDict.ContainsKey(nextNodeInside))
                    NodesDict[nextNodeInside].Execute();
            }
            else
            {
                if (NodesDict.ContainsKey(nextNodeId))
                    NodesDict[nextNodeId].Execute();
            }
        }
        else if (input.Contains("<>"))
        {
            if (value1 != value2)
            {
                if (NodesDict.ContainsKey(nextNodeInside))
                    NodesDict[nextNodeInside].Execute();
            }
            else
            {
                if (NodesDict.ContainsKey(nextNodeId))
                    NodesDict[nextNodeId].Execute();
            }
        }
    }

    public override void CallNextNode()
    {
        if (NodesDict.ContainsKey(nextNodeId))
            NodesDict[nextNodeId].Execute();
    }

    public override void PostExecutionCleanUp(object sender, EventArgs e)
    {
        Debug.Log("node while clean up do nothing");
    }
}
