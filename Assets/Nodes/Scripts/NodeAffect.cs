using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System.Data;
using System;
using System.Text.RegularExpressions;
using Language;

public class NodeAffect : Nodes
{
    private string input;

    private VarsManager.Var var;

    public void ChangeInput(TMP_InputField tMP_InputField)
    {
        input = tMP_InputField.text;
        if(!ValidateInput())
        {
            Debug.LogError("wrong input");
            ChangeBorderColor(errorColor);
            Manager.instance.canExecute = false;
            return;
        }
        Manager.instance.canExecute = true;
        ChangeBorderColor(defaultColor);
    }

    private bool ValidateInput()
    {
        string[] inputSplited = input.Split(' ');
        if (!inputSplited[0].Any(char.IsDigit))
        {
            try
            {
                if (inputSplited[1] == "=")
                {
                    for (int i = 2; i < inputSplited.Length; i++)
                    {
                        if (!(inputSplited[i].Any(Char.IsDigit) || inputSplited[i].Any(Char.IsLetter)))
                        {
                            if (inputSplited[i].Length == 1)
                            {
                                switch (inputSplited[i][0])
                                {
                                    case '+':
                                    case '-':
                                    case '*':
                                    case '/':
                                    case '(':
                                    case ')':
                                        break;
                                    default:
                                        return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        return false;
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
        // calculate and set the var
        string[] inputSplited = VarsManager.Instance.ReplaceStringByVar(input.Split(' '));
        if(inputSplited != null)
        {
            if(var != null)
                var = VarsManager.Instance.getVar(inputSplited[0]);

            string expression = string.Join("", inputSplited, 2, inputSplited.Length - 2).Trim();
            var.Value = Convert.ToInt32(new DataTable().Compute(expression, null));
            var.Persist();
        }
        else
        {
            // stop execution
            Debugger.LogError(Translation.Get("varNotExist"));
            ChangeBorderColor(errorColor);
        }
    }

    public override void PostExecutionCleanUp()
    {
        var = null;
    }
}
