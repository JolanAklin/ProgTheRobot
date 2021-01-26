using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PopUpMenu : MonoBehaviour
{
    public enum MenuSettings
    {
        save = 0,
        load,
        import,
        newFile,
        options,
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

    private void Start()
    {
        foreach (MenuSettingsClass menuSetting in menuSettings)
        {
            menuSettingsDict.Add((int)Enum.Parse(typeof(MenuSettings), menuSetting.menuType), menuSetting.menuObj);
        }
    }

    public void Quit()
    {
        // test if there is changes before exiting
        Application.Quit();
    }
}
