// Copyright 2021 Jolan Aklin

//This file is part of Prog The Robot.

//Prog The Robot is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 3 of the License.

//Prog The Robot is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Prog the robot.  If not, see<https://www.gnu.org/licenses/>.

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
    public ToggleScript toggleScript;

    public TMP_Text versionText;

    public Action okAction;
    public Action cancelAction;

    private void Start()
    {
        versionText.text = $"Version : {Application.version}";

        toggleScript.Value = Manager.instance.connectHandleAlwaysShown;
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
                    ChangeConnectHandleDisplayMethod(toggleScript);
                    SaveManager.instance.savePath = filePath.EndsWith("/") ? filePath : filePath + "/";
                    SaveManager.instance.SaveSettings();
                    menu.Close();
                }else
                {
                    PopUpWarning sw = PopUpManager.ShowPopUp(PopUpManager.PopUpTypes.saveWarning).GetComponent<PopUpWarning>();
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

    private void ChangeConnectHandleDisplayMethod(ToggleScript toggle)
    {
        Manager.instance.ChangeConnectHandleDisplayMethod(toggle);
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
