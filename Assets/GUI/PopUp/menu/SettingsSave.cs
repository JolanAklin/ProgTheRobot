using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class SettingsSave : MonoBehaviour
{
    public string fileName;

    public Action saveAction;
    public Action saveAsAction;

    public void OnEndEditFileName(TMP_InputField inputField)
    {
        fileName = inputField.text;
    }

    #region buttons action
    public void SetSaveActiion(Action action)
    {
        saveAction = action;
    }
    public void SetSaveAsAction(Action action)
    {
        saveAsAction = action;
    }

    public void Save()
    {
        saveAction();
    }
    public void SaveAs()
    {
        saveAsAction();
    }
    #endregion
}
