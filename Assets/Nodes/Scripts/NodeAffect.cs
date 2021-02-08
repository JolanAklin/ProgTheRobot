using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NodeAffect : Nodes
{
    private string input;

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
        throw new System.NotImplementedException();
    }
}
