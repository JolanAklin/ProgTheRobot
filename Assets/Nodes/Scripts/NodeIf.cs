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
        string expression = inputSplited[0].Replace(" ", string.Empty).Trim();
        int value1 = Convert.ToInt32(new DataTable().Compute(expression, null));
        expression = inputSplited[1].Replace(" ", string.Empty).Trim();
        int value2 = Convert.ToInt32(new DataTable().Compute(expression, null));
        if (input.Contains("="))
        {
            if (value1 == value2)
            {

            }

        }else if (input.Contains("<"))
        {
            if(value1 < value2)
            {

            }
        }
        else if(input.Contains(">"))
        {
            if (value1 > value2)
            {

            }
        }
        else if(input.Contains(">="))
        {
            if (value1 >= value2)
            {

            }
        }
        else if(input.Contains("<="))
        {
            if (value1 <= value2)
            {

            }
        }
        else if(input.Contains("<>"))
        {
            if (value1 != value2)
            {

            }
        }
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
