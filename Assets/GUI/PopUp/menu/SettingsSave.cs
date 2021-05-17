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
using TMPro;
using System.IO;
using UnityEngine.UI;

public class SettingsSave : MonoBehaviour
{
    private PopUpMenu menu;

    public string fileName;
    public TMP_Text saveButtonText;
    public TMP_InputField inputField;
    public Button saveButton;

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
        if(SaveManager.instance.filepath.Length > 1)
        {
            saveButtonText.text = $"Enregistrer {Path.GetFileName(SaveManager.instance.filepath)}";
            saveButton.interactable = true;
        }
        else
        {
            saveButtonText.text = "Enregistrer";
            saveButton.interactable = false;
        }
        // start tpi
        saveAsAction = () =>
        {
            SaveLoadFileBrowser.instance.ShowSaveFileDialog(
                (paths) =>
                {
                    if (paths != null)
                    {
                        if (paths[0].Length > 0)
                        {
                            SaveManager.instance.filepath = paths[0];
                            SaveManager.instance.Save();
                            menu.Close();
                        }
                    }
                },
                () =>
                {
                    Debug.Log("canceled");
                });
        };
        // end tpi
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
