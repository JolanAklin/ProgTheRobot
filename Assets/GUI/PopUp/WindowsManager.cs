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

public class WindowsManager : MonoBehaviour
{
    public enum popUp
    {
        menu = 0,
        robotModif,
        colorPicker,
        saveWarning,
        addScript,
        readWrite,
        nodeInfo,
        wait,
    }

    [Serializable]
    public class popUpClass
    {
        public GameObject popUpObj;
        public string popUpType;
    }

    public GameObject editWindow;

    public popUpClass[] popUpWindows;
    [HideInInspector]
    public Dictionary<int, GameObject> popUpWindowsDict = new Dictionary<int, GameObject>();

    public static WindowsManager instance;

    private void Awake()
    {
        instance = this;
        foreach (popUpClass popUp in popUpWindows)
        {
            popUpWindowsDict.Add((int)((popUp)Enum.Parse(typeof(popUp), popUp.popUpType)), popUp.popUpObj);
        }
    }

    public static GameObject InstantiateWindow(int WindowType, Transform parentTransform)
    {
        return Instantiate(instance.popUpWindowsDict[WindowType], parentTransform);
    }

    public void ShowMain()
    {
        editWindow.SetActive(false);
    }

    public void ShowEdit()
    {
        editWindow.SetActive(true);
    }

    public void ShowMainMenu()
    {
        PopUpMenu pm = InstantiateWindow((int)Enum.Parse(typeof(popUp), "menu"), Manager.instance.canvas.transform).GetComponent<PopUpMenu>();
    }
}
