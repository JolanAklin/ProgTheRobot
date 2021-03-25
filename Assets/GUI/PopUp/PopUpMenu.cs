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

    public void Close()
    {
        Destroy(this.gameObject);
    }

    public void Quit()
    {
        // test if there is changes before exiting
        Application.Quit();
    }
}
