using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using TMPro;

public class SettingsOptions : MonoBehaviour
{
    private PopUpMenu menu;
    public TMP_InputField inputField;
    public string filePath;
    public Action okAction;
    public Action cancelAction;

    private void Start()
    {
        menu = GameObject.FindGameObjectWithTag("MainMenu").GetComponent<PopUpMenu>();
        inputField.text = SaveManager.instance.savePath;
        filePath = inputField.text;
        cancelAction = () =>
        {
            menu.Close();
        };
        okAction = () =>
        {
            if(filePath.Length > 0)
            {
                if(Directory.Exists(filePath))
                {
                    
                    SaveManager.instance.savePath = filePath.EndsWith("/") ? filePath : filePath + "/";
                    SaveManager.instance.SaveSettings();
                    menu.Close();
                }else
                {
                    PopUpWarning sw = WindowsManager.InstantiateWindow((int)Enum.Parse(typeof(WindowsManager.popUp), "saveWarning"), Manager.instance.canvas.transform).GetComponent<PopUpWarning>();
                    sw.warningText.text = "Ce repertoire n'existe pas";
                    sw.quitButton.gameObject.SetActive(false);
                    sw.saveButton.gameObject.SetActive(false);
                    sw.SetCancelAction(() =>
                    {
                        sw.Close();
                    });
                    inputField.Select();
                }    
            }else
            {
                inputField.Select();
            }
        };
    }

    public void OnEndEditFilePath(TMP_InputField inputField)
    {
        filePath = inputField.text;
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
