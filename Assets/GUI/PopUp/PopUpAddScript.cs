using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class PopUpAddScript : MonoBehaviour
{
    [HideInInspector]
    public string scriptName;
    //a voir pour les items pour la liste 

    private Action cancelAction;
    private Action okAction;

    public void PopUpClose()
    {
        Destroy(this.gameObject);
    }

    public void OnEndEditScriptName(TMP_InputField inputField)
    {
        scriptName = inputField.text;
    }

    #region buttons action
    public void SetCancelAction(Action action)
    {
        cancelAction = action;
    }
    public void SetOkAction(Action action)
    {
        okAction = action;
    }

    public void Cancel()
    {
        cancelAction();
    }
    public void Ok()
    {
        okAction();
    }
    #endregion
}
