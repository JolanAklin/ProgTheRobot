using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NodeForLoop : Nodes
{
    private string input;
    public void ChangeInput(TMP_InputField tMP_InputField)
    {
        input = tMP_InputField.text;
        if (!ValidateInput())
        {
            nodeErrorCode = (int)Nodes.ErrorCode.wrongInput;
            Debug.LogError("Wrong Input");
            Manager.instance.canExecute = false;
            return;
        }
        Manager.instance.canExecute = true;
    }

    public bool ValidateInput()
    {
        int varStart;
        int varEnd;
        int varStep;
        // Pour i = 0 Jusque 2 Pas 1
        string[] inputSplited = input.Split(' ');
        try
        {
            if(inputSplited[0] == "Pour")
                if (inputSplited[2] == "=")
                    if (int.TryParse(inputSplited[3], out varStart))
                        if (inputSplited[4] == "Jusque")
                            if (int.TryParse(inputSplited[5], out varEnd))
                            {
                                if (inputSplited[6] == "Pas")
                                {
                                    if (int.TryParse(inputSplited[7], out varEnd))
                                    {
                                        Debug.Log("full");
                                    }
                                    else
                                        return false;
                                }
                                else
                                    return false;

                                Debug.Log("half");
                                return true;
                            }

            return false;

        }catch
        {
            return false;
        }
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
