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
using UnityEngine.UI;

public class PopUpAddScript : PopUp
{
    [HideInInspector]
    public string scriptName;

    private Action cancelAction;
    private Action okAction;
    private Action<RobotScript> addScriptAction;

    // start tpi
    public GameObject listContent;
    public GameObject buttonsPanelPrefab;
    public GameObject buttonMainPrefab;
    public GameObject buttonChildrenPrefab;
    //end tpi

    private void Start()
    {
        // start tpi

        // create the script list with all the script even the deleted ones
        foreach (RobotScript.ScriptsInRobotHierarchy unassignedScript in RobotScript.unassignedRobotScript)
        {
            // create a panel to hold the scripts
            GameObject buttonsPanel = Instantiate(buttonsPanelPrefab, Vector3.zero, Quaternion.identity, listContent.transform);
            ScriptListCollapsable collapsable = buttonsPanel.GetComponent<ScriptListCollapsable>();

            // create and add all scripts button
            collapsable.objectToHide.Add(CreateMainScriptImportButton(unassignedScript, buttonsPanel, true));
            collapsable.objectToHide.AddRange(CreateChildrenScriptImportButtons(unassignedScript, buttonsPanel));
            collapsable.Collapse();
        }

        foreach (Robot robot in Robot.robots.Values)
        {
            RobotScript.ScriptsInRobotHierarchy hierarchy = robot.GetScriptHierarchy();
            // create a panel to hold the scripts
            GameObject buttonsPanel = Instantiate(buttonsPanelPrefab, Vector3.zero, Quaternion.identity, listContent.transform);
            ScriptListCollapsable collapsable = buttonsPanel.GetComponent<ScriptListCollapsable>();

            // create and add all scripts button
            collapsable.objectToHide.Add(CreateMainScriptImportButton(hierarchy, buttonsPanel));
            collapsable.objectToHide.AddRange(CreateChildrenScriptImportButtons(hierarchy, buttonsPanel));
            collapsable.Collapse();

        }
        // end tpi
    }

    /// <summary>
    /// Create the import button for the main script
    /// </summary>
    /// <param name="unassignedScript">A ScriptsInRobotHierarchy object to define the main script</param>
    /// <param name="buttonsPanel">The panel where the button will be added</param>
    /// <param name="shouldRemoveEntry">Remove the entry from RobotScript.unassignedRobotScript, should only be true when the ScriptsInRobotHierarchy come from this list</param>
    /// <returns>The created button</returns>
    private GameObject CreateMainScriptImportButton(RobotScript.ScriptsInRobotHierarchy unassignedScript, GameObject buttonsPanel, bool shouldRemoveEntry = false)
    {
        // create a button for each script
        GameObject buttonScript = Instantiate(buttonMainPrefab, Vector3.zero, Quaternion.identity, buttonsPanel.transform);
        buttonScript.transform.GetChild(0).GetComponent<TMP_Text>().text = unassignedScript.main.name;
        // when the button with the main script is clicked
        buttonScript.GetComponent<Button>().onClick.AddListener(() =>
        {
            // start tpi
            // test if there is room available on the terrain for a new robot
            TerrainManager terrainManager = GameObject.FindGameObjectWithTag("Terrain").GetComponent<TerrainManager>();
            if (terrainManager.TerrainSize[0] * terrainManager.TerrainSize[1] < Robot.robots.Count + 1)
            {
                PopUpWarning w = PopUpManager.ShowPopUp(PopUpManager.PopUpTypes.saveWarning).GetComponent<PopUpWarning>();
                w.warningText.text = "Il n'y a plus de place pour un autre robot";
                w.quitButton.gameObject.SetActive(false);
                w.saveButton.gameObject.SetActive(false);
                w.SetCancelAction(() =>
                {
                    w.Close();
                });
                return;
            }
            // end tpi

            //create a new robot
            Robot robot = new Robot(Color.red, "Robot", 1000, false);
            Manager.instance.listRobot.AddChoice(robot.id, robot.ConvertToListElement());
            Manager.instance.listRobot.Select(robot.id);

            // add the main script 
            RobotScript clone = ScriptCloner.CloneScript(unassignedScript.main);
            robot.MainScript = clone;
            clone.robot = robot;
            List.ListElement element = robot.AddScript(clone);
            Manager.instance.list.AddChoice(element);
            Manager.instance.list.SelectLast();
            Manager.instance.onScriptAdded?.Invoke(this, EventArgs.Empty);

            // add all other children of the main script
            foreach (RobotScript rs in unassignedScript.childrens)
            {
                clone = ScriptCloner.CloneScript(rs);
                clone.robot = robot;
                element = robot.AddScript(clone);
                Manager.instance.list.AddChoice(element);
                Manager.instance.list.SelectLast();
                Manager.instance.onScriptAdded?.Invoke(this, EventArgs.Empty);
            }
            if (shouldRemoveEntry)
                RobotScript.unassignedRobotScript.Remove(unassignedScript);
            Manager.instance.ChangeRobotSettings();
            this.Close();
        });
        return buttonScript;
    }

    /// <summary>
    /// Create import buttons for the children of the main script
    /// </summary>
    /// <param name="unassignedScript">A ScriptsInRobotHierarchy object to define all childrens</param>
    /// <param name="buttonsPanel">The panel where the button will be added</param>
    /// <returns>An array of all add script buttons</returns>
    private GameObject[] CreateChildrenScriptImportButtons(RobotScript.ScriptsInRobotHierarchy unassignedScript, GameObject buttonsPanel)
    {
        GameObject[] buttons = new GameObject[unassignedScript.childrens.Count];
        int i = 0;
        GameObject buttonScript;
        foreach (RobotScript rs in unassignedScript.childrens)
        {
            buttonScript = Instantiate(buttonChildrenPrefab, Vector3.zero, Quaternion.identity, buttonsPanel.transform);
            buttonScript.transform.GetChild(0).GetComponent<TMP_Text>().text = rs.name;
            // called when a children script button is clicked
            buttonScript.GetComponent<Button>().onClick.AddListener(() =>
            {
                addScriptAction.Invoke(rs);
            });
            buttons[i] = buttonScript;
            i++;
        }
        return buttons;
    }

    public void OnEndEditScriptName(TMP_InputField inputField)
    {
        scriptName = inputField.text;
    }

    #region buttons action
    public void SetCancelAction(Action action)
    {
        cancelAction = action;
    }
    public void SetOkAction(Action action)
    {
        okAction = action;
    }
    // start tpi

    /// <summary>
    /// Set the addScriptAction, this action is called when a child script button is clicked
    /// </summary>
    /// <param name="action">The new addScriptAction</param>
    public void SetAddScriptAction(Action<RobotScript> action)
    {
        addScriptAction = action;
    }
    //end tpi

    public void Cancel()
    {
        cancelAction();
    }
    public void Ok()
    {
        if(scriptName.Length == 0)
        {
            gameObject.SetActive(false);
            PopUpWarning sw = PopUpManager.ShowPopUp(PopUpManager.PopUpTypes.saveWarning).GetComponent<PopUpWarning>();
            sw.warningText.text = "Veuillez entrer un nom pour l'organigramme";
            sw.quitButton.gameObject.SetActive(false);
            sw.saveButton.gameObject.SetActive(false);
            sw.SetCancelAction(() =>
            {
                sw.Close();

                gameObject.SetActive(true);
            });
            return;
        }
        okAction();
    }
    #endregion
}
