// Copyright 2021 Jolan Aklin

//This file is part of Prog the robot.

//Prog the robot is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 3 of the License.

//FileTeleporter is distributed in the hope that it will be useful,
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
