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

    public void ChangeInput(TMP_InputField tMP_InputField)
    {
        input = tMP_InputField.text;
        if (!ValidateInput())
        {
            ChangeBorderColor(errorColor);
            Manager.instance.canExecute = false;
            return;
        }
        Manager.instance.canExecute = true;
        ChangeBorderColor(defaultColor);
    }

    private bool ValidateInput()
    {
        string[] delimiters = new string[] { "=", "<", ">", ">=", "<=", "<>" };
        inputSplited = input.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        if (inputSplited.Length > 2 || inputSplited.Length == 1)
            return false;
        return true;
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
        //string expression = inputSplited[0].Replace(" ", string.Empty).Trim();
        string[] delimiters = new string[] { " " };
        string[] expressionSplited1 = inputSplited[0].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        string expr = string.Join("", rs.robot.varsManager.ReplaceStringsByVar(expressionSplited1));
        if(expr == null)
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
                if (NodesDict.ContainsKey(nextNodeId))
                    NodesDict[nextNodeId].Execute();
            }else
            {
                if (NodesDict.ContainsKey(nextNodeIdFalse))
                    NodesDict[nextNodeIdFalse].Execute();
            }

        }else if (input.Contains("<"))
        {
            if (value1 < value2)
            {
                if (NodesDict.ContainsKey(nextNodeId))
                    NodesDict[nextNodeId].Execute();
            }
            else
            {
                if (NodesDict.ContainsKey(nextNodeIdFalse))
                    NodesDict[nextNodeIdFalse].Execute();
            }
        }
        else if(input.Contains(">"))
        {
            if (value1 > value2)
            {
                if (NodesDict.ContainsKey(nextNodeId))
                    NodesDict[nextNodeId].Execute();
            }
            else
            {
                if (NodesDict.ContainsKey(nextNodeIdFalse))
                    NodesDict[nextNodeIdFalse].Execute();
            }
        }
        else if(input.Contains(">="))
        {
            if (value1 >= value2)
            {
                if (NodesDict.ContainsKey(nextNodeId))
                    NodesDict[nextNodeId].Execute();
            }
            else
            {
                if (NodesDict.ContainsKey(nextNodeIdFalse))
                    NodesDict[nextNodeIdFalse].Execute();
            }
        }
        else if(input.Contains("<="))
        {
            if (value1 <= value2)
            {
                if (NodesDict.ContainsKey(nextNodeId))
                    NodesDict[nextNodeId].Execute();
            }
            else
            {
                if (NodesDict.ContainsKey(nextNodeIdFalse))
                    NodesDict[nextNodeIdFalse].Execute();
            }
        }
        else if(input.Contains("<>"))
        {
            if (value1 != value2)
            {
                if (NodesDict.ContainsKey(nextNodeId))
                    NodesDict[nextNodeId].Execute();
            }
            else
            {
                if (NodesDict.ContainsKey(nextNodeIdFalse))
                    NodesDict[nextNodeIdFalse].Execute();
            }
        }
    }

    public override void CallNextNode()
    {
        //unused
    }

    public override void PostExecutionCleanUp(object sender, EventArgs e)
    {
        Debug.Log("Node if clean up do nothing");
    }
}
