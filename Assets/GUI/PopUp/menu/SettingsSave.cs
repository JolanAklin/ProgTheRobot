// Copyright 2021 Jolan Aklin

//This file is part of Prog the robot.

//Prog the robot is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 3 of the License.

//Prog the robot is distributed in the hope that it will be useful,
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
