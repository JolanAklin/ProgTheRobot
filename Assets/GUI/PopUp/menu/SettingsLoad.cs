using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SettingsLoad : MonoBehaviour
{
    public Action cancelAction;
    public Action importAction;
    public Action loadAction;

    #region buttons action
    public void SetCancelAction(Action action)
    {
        cancelAction = action;
    }
    public void SetImportAction(Action action)
    {
        importAction = action;
    }
    public void SetLoadAction(Action action)
    {
        loadAction = action;
    }

    public void Cancel()
    {
        cancelAction();
    }
    public void Import()
    {
        importAction();
    }
    public void Load()
    {
        loadAction();
    }
    #endregion
}
