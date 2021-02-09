using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class NodeAffect : Nodes
{
    private string input;

    private string varAffect;

    public void ChangeInput(TMP_InputField tMP_InputField)
    {
        input = tMP_InputField.text;
        if(ValidateInput())
        {
            Debug.LogError("wrong input");
        }
    }

    private bool ValidateInput()
    {
        string[] inputSplited = input.Split(' ');
        if(!inputSplited[0].Any(char.IsDigit))
        {
            varAffect = inputSplited[0];
            if (inputSplited[1] == "=")
                return true;
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
        throw new System.NotImplementedException();
    }
}
