using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Language;
using System;

public class NodeCommande : Nodes
{
    private string input;
    private TMP_InputField inputField;

    new private void Awake()
    {
        base.Awake();
        Manager.instance.OnLanguageChanged += TranslateText;
    }

    private void OnDestroy()
    {
        Manager.instance.OnLanguageChanged -= TranslateText;
    }

    public void ChangeInput(TMP_InputField tMP_InputField)
    {
        inputField = tMP_InputField;
        input = tMP_InputField.text;
        if (!ValidateInput())
        {
            nodeErrorCode = (int)Nodes.ErrorCode.wrongInput;
            ChangeBorderColor(errorColor);
            Manager.instance.canExecute = false;
            Debugger.LogError("Commande inconnue");
            return;
        }
        Manager.instance.canExecute = true;
        ChangeBorderColor(defaultColor);
    }


    private bool ValidateInput()
    {
        switch (input)
        {
            case "":
            case "Avancer":
            case "GoForward":
            case "TournerADroite":
            case "TurnRight":
            case "TournerAGauche":
            case "TurnLeft":
            case "Marquer":
            case "Mark":
            case "Démarquer":
            case "Unmark":
            case "Recharger":
            case "Reload":
            case "PoserBallon":
            case "PlaceBall":
            case "PrendreBallon":
            case "TakeBall":
            case "LancerBallon":
            case "ThrowBall":
                TranslateText(this, EventArgs.Empty);
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

    public void TranslateText(object sender, EventArgs e)
    {
        switch (input)
        {
            case "":
                break;
            case "Avancer":
            case "GoForward":
                input = Translation.Get("GoForward");
                break;
            case "TournerADroite":
            case "TurnRight":
                input = Translation.Get("TurnRight");
                break;
            case "TournerAGauche":
            case "TurnLeft":
                input = Translation.Get("TurnLeft");
                break;
            case "Marquer":
            case "Mark":
                input = Translation.Get("Mark");
                break;
            case "Démarquer":
            case "Unmark":
                input = Translation.Get("Unmark");
                break;
            case "Recharger":
            case "Reload":
                input = Translation.Get("Reload");
                break;
            case "PoserBallon":
            case "PlaceBall":
                input = Translation.Get("PlaceBall");
                break;
            case "PrendreBallon":
            case "TakeBall":
                input = Translation.Get("TakeBall");
                break;
            case "LancerBallon":
            case "ThrowBall":
                input = Translation.Get("Throwball");
                break;
        }
        inputField.text = input;
    }

    public override void Execute()
    {
        switch (input)
        {
            case "":
                break;
            case "Avancer":
            case "GoForward":
                rs.robot.robotManager.GoForward(() => { CallNextNode(); });
                break;
            case "TournerADroite":
            case "TurnRight":
                rs.robot.robotManager.TurnRight(() => { CallNextNode(); });
                break;
            case "TournerAGauche":
            case "TurnLeft":
                rs.robot.robotManager.TurnLeft(() => { CallNextNode(); });
                break;
            case "Marquer":
            case "Mark":
                rs.robot.robotManager.Mark();
                break;
            case "Démarquer":
            case "Unmark":
                rs.robot.robotManager.Unmark();
                break;
            case "Recharger":
            case "Reload":
                rs.robot.robotManager.Reload();
                break;
            case "PoserBallon":
            case "PlaceBall":
                rs.robot.robotManager.PlaceBall();
                break;
            case "PrendreBallon":
            case "TakeBall":
                rs.robot.robotManager.TakeBall();
                break;
            case "LancerBallon":
            case "ThrowBall":
                rs.robot.robotManager.ThrowBall();
                break;

            default:
                Debugger.LogError("Commande inconnue");
                break;
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
