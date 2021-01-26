using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PopUpWarning : MonoBehaviour
{
    private Action quitAction;
    private Action cancelAction;
    private Action saveAction;

    #region buttons action
    public void SetQuitAction(Action action)
    {
        quitAction = action;
    }
    public void SetCancelAction(Action action)
    {
        cancelAction = action;
    }
    public void SetSaveAction(Action action)
    {
        saveAction = action;
    }

    public void Quit()
    {
        quitAction();
    }
    public void Cancel()
    {
        cancelAction();
    }
    public void save()
    {
        saveAction();
    }
    #endregion
}
