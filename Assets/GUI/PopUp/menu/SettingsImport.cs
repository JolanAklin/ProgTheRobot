using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SettingsImport : MonoBehaviour
{
    private PopUpMenu menu;
    public Action cancelAction;
    public Action importAction;

    private void Start()
    {
        menu = GameObject.FindGameObjectWithTag("MainMenu").GetComponent<PopUpMenu>();
        cancelAction = () =>
        {
            menu.Close();
        };
    }

    #region buttons action
    public void SetCancelAction(Action action)
    {
        cancelAction = action;
    }
    public void SetImportAction(Action action)
    {
        importAction = action;
    }

    public void Cancel()
    {
        cancelAction();
    }
    public void Import()
    {
        importAction();
    }
    #endregion
}
