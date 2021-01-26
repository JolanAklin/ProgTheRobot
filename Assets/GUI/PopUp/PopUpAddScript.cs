using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PopUpAddScript : MonoBehaviour
{
    public string scriptName;
    //a voir pour les items pour la liste 

    private Action cancelAction;
    private Action okAction;

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
