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
            SaveManager.instance.LoadFile(fileNameToLoad);
            menu.Close();
        };
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
    #endregion
}
