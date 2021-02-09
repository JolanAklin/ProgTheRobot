using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class NodeForLoop : Nodes
{
    private string input;

    private int varStart; // value of the var at the start of the loop
    private int varEnd; // until value
    private int varStep; // increment
    private string varName; // name of the for var
    public void ChangeInput(TMP_InputField tMP_InputField)
    {
        input = tMP_InputField.text;
        if (!ValidateInput())
        {
            nodeErrorCode = (int)Nodes.ErrorCode.wrongInput;
            ChangeBorderColor(errorColor);
            Manager.instance.canExecute = false;
            return;
        }
        Debug.Log($"varStart : {varStart}");
        Debug.Log($"varEnd : {varEnd}");
        Debug.Log($"varStep : {varStep}");
        Debug.Log($"varName : {varName}");
        ChangeBorderColor(defaultColor);
        Manager.instance.canExecute = true;
    }

    public bool ValidateInput()
    {
        // need to add the var support
        // Pour i = 0 Jusque 2 Pas 1
        // Pour i = 0 Jusque 3
        string[] inputSplited = input.Split(' ');
        try
        {
            if(inputSplited[0] == "Pour")
                if(!inputSplited[1].Any(char.IsDigit))
                    if (inputSplited[2] == "=")
                        if (int.TryParse(inputSplited[3], out varStart))
                            if (inputSplited[4] == "Jusque")
                                if (int.TryParse(inputSplited[5], out varEnd))
                                {
                                    varStep = 1;
                                    if(inputSplited.Length == 8)
                                    {
                                        if (inputSplited[6] == "Pas")
                                        {
                                            if (!int.TryParse(inputSplited[7], out varStep))
                                                return false;
                                        }
                                        if (inputSplited.Length > 8)
                                            return false;
                                    }

                                    if (inputSplited.Length > 6 && inputSplited.Length != 8)
                                        return false;

                                    varName = inputSplited[1];

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
