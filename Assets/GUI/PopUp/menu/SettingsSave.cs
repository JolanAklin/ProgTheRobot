using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class SettingsSave : MonoBehaviour
{
    private PopUpMenu menu;

    public string fileName;
    public TMP_Text saveButtonText;
    public TMP_InputField inputField;

    public Action saveAction;
    public Action saveAsAction;
    public Action cancelAction;

    private void Start()
    {
        menu = GameObject.FindGameObjectWithTag("MainMenu").GetComponent<PopUpMenu>();
        cancelAction = () =>
        {
            menu.Close();
        };
        saveButtonText.text = $"Enregistrer {SaveManager.instance.fileName}";
        saveAsAction = () =>
        {
            if (fileName.Length > 0)
            {
                SaveManager.instance.fileName = fileName;
                SaveManager.instance.Save();
                menu.Close();
            }
            else
            {
                inputField.Select();
            }
        };
        saveAction = () =>
        {
            SaveManager.instance.Save();
            menu.Close();
        };
    }

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
    public void SetCancelAction(Action action)
    {
        cancelAction = action;
    }

    public void Save()
    {
        saveAction();
    }
    public void SaveAs()
    {
        saveAsAction();
    }
    public void Cancel()
    {
        cancelAction();
    }
    #endregion
}
