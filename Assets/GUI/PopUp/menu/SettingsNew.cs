using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class SettingsNew : MonoBehaviour
{
    private PopUpMenu menu;
    public string fileName;
    public TMP_InputField inputField;
    public Action okAction;
    public Action cancelAction;


    private void Start()
    {
        menu = GameObject.FindGameObjectWithTag("MainMenu").GetComponent<PopUpMenu>();
        cancelAction = () =>
        {
            menu.Close();
        };
        okAction = () =>
        {
            if (fileName.Length > 0)
            {
                SaveManager.instance.fileName = fileName;
                SaveManager.instance.New();
                menu.Close();
            }
            else
            {
                inputField.Select();
            }
        };
    }


    public void OnEndEditFilename(TMP_InputField inputField)
    {
        fileName = inputField.text;
    }

    public void SetOkAction(Action action)
    {
        okAction = action;
    }

    public void SetCancelAction(Action action)
    {
        cancelAction = action;
    }

    public void Ok()
    {
        okAction();
    }

    public void Cancel()
    {
        cancelAction();
    }
}
