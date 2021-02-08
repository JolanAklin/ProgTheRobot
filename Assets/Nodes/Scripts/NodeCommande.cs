using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NodeCommande : Nodes
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


    private bool ValidateInput()
    {
        switch (input)
        {
            case "Avancer":
            case "TournerADroite":
            case "TournerAGauche":
            case "Marquer":
            case "Démarquer":
            case "Recharger":
            case "PoserBallon":
            case "PrendreBallon":
            case "LancerBallon":
                return true;
            default:
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
