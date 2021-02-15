using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System.Data;
using System;

public class NodeAffect : Nodes
{
    private string input;
    private string[] inputSplited;

    private VarsManager.Var var;

    public void ChangeInput(TMP_InputField tMP_InputField)
    {
        input = tMP_InputField.text;
        if(!ValidateInput())
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
        string[] delimiters = new string[] { " " };
        inputSplited = input.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        if (inputSplited.Length <= 2)
            return false;
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
                            return VarsManager.CheckVarName(inputSplited[i]);
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
        if (inputSplited != null)
        {
            if (var == null)
            {
                var = VarsManager.Instance.GetVar(inputSplited[0]);
                if (var == null)
                {
                    Debugger.LogError("Une erreur est survenue");
                    return;
                }
            }

            string expression = string.Join("", inputSplited, 2, inputSplited.Length - 2).Trim();
            var.Value = Convert.ToInt32(new DataTable().Compute(expression, null));
            var.Persist();
        }
        else
        {
            // stop execution
            Debugger.LogError("La variable spécifiée n'est pas connue");
            ChangeBorderColor(errorColor);
        }
    }

    public override void PostExecutionCleanUp()
    {
        var = null;
    }
}
