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
            // start tpi
            SaveLoadFileBrowser.instance.ShowSaveFileDialog(
            (paths) =>
            {
                if (paths != null)
                {
                    if (paths[0].Length > 0)
                    {
                        SaveManager.instance.filepath = paths[0];
                        SaveManager.instance.LoadFile();
                        menu.Close();
                    }
                }
            },
            () =>
            {
                Debug.Log("canceled");
            });
            // end tpi
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
