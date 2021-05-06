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
using System.IO;

public class SettingsLoad : MonoBehaviour
{
    private PopUpMenu menu;
    // must delete importaction and gui button
    public Action cancelAction;
    public Action importAction;
    public Action loadAction;
    public Action loadFromSomeWhereElseAction;

    public List projectList;

    private string fileNameToLoad;

    private void Start()
    {
        menu = GameObject.FindGameObjectWithTag("MainMenu").GetComponent<PopUpMenu>();
        List<List.ListElement> choices = new List<List.ListElement>();
        foreach (string file in Directory.EnumerateFiles(SaveManager.instance.savePath))
        {
            if(file.EndsWith(".pr"))
            {
                string[] fileSplit = file.Split('/');
                string fileName = fileSplit[fileSplit.Length - 1];
                choices.Add(new List.ListElement() { displayedText = fileName, actionOnClick = () =>
                {
                    fileNameToLoad = fileName;
                } });
            }
        }
        projectList.Init(choices, 0);

        cancelAction = () =>
        {
            menu.Close();
        };

        loadAction = () =>
        {
            SaveManager.instance.LoadFile(fileNameToLoad, false);
            menu.Close();
        };
        // start tpi
        loadFromSomeWhereElseAction = () =>
        {
            SaveLoadFileBrowser.instance.ShowLoadFileDialog((paths) =>
            {
                if(paths != null)
                {
                    SaveManager.instance.LoadFile(paths[0], true);
                    menu.Close();
                }
            },
            () =>
            {
                Debug.Log("canceled");
            });
        };
        // end tpi
    }

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

    public void LoadFromSomeWhereElse()
    {
        loadFromSomeWhereElseAction();
    }
    #endregion
}
