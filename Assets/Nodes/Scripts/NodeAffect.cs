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

    private VarsManager.Var var;

    public void ChangeInput(TMP_InputField tMP_InputField)
    {
        input = tMP_InputField.text;
        if(ValidateInput())
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
        if(!inputSplited[0].Any(char.IsDigit))
        {
            try
            {
                var = VarsManager.Instance.getVar(inputSplited[0]);
                if (inputSplited[1] == "=")
                {
                    for (int i = 0; 2 < inputSplited.Length-1; i++)
                    {

                    }
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
        string[] inputSplited = input.Split(' ');
        string expression = string.Join("", inputSplited, 2, inputSplited.Length - 2).Trim();
        var.Value = Convert.ToInt32(new DataTable().Compute(expression, null));
        var.Persist();
        //Debugger.Log($"{var.Name} : {var.Value}");
        throw new System.NotImplementedException();
    }
}
