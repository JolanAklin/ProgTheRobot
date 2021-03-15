using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class PopUpWarning : MonoBehaviour
{
    private Action quitAction;
    private Action cancelAction;
    private Action saveAction;

    public TMP_Text warningText;
    public Button quitButton;
    public Button saveButton;

    public void Close()
    {
        Destroy(this.gameObject);
    }

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
