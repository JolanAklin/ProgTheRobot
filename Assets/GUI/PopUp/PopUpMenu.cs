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

public class PopUpMenu : PopUp
{
    public enum MenuSettings
    {
        save = 0,
        load,
        newFile,
        options,
        credits
    }

    [Serializable]
    public class MenuSettingsClass
    {
        public GameObject menuObj;
        public string menuType;
    }

    public MenuSettingsClass[] menuSettings;
    [HideInInspector]
    public Dictionary<int, GameObject> menuSettingsDict = new Dictionary<int, GameObject>();
    public string[] choicesName;
    public List list;
    public GameObject content;

    private void Start()
    {
        List<List.ListElement> choices = new List<List.ListElement>();
        foreach (MenuSettingsClass menuSetting in menuSettings)
        {
            int id = (int)Enum.Parse(typeof(MenuSettings), menuSetting.menuType);
            menuSettingsDict.Add(id, menuSetting.menuObj);
            choices.Add(new List.ListElement() { displayedText = choicesName[id], actionOnClick = () => { 
                InstantiateSubMenu(id); 
            } });
        }
        list.Init(choices, 0);
    }

    public GameObject InstantiateSubMenu(int subMenuType)
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
        return Instantiate(menuSettings[subMenuType].menuObj, content.transform);
    }

    public void Quit()
    {
        Close();
        Application.Quit();
    }
}
