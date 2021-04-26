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
using UnityEngine.UI;

public class PopUpAddScript : MonoBehaviour
{
    [HideInInspector]
    public string scriptName;

    private Action cancelAction;
    private Action okAction;
    private Action<RobotScript> addScriptAction;

    // start tpi
    public GameObject listContent;
    public GameObject buttonsPanelPrefab;
    public GameObject buttonScriptPrefab;
    //end tpi

    private void Start()
    {
        // start tpi
        foreach (RobotScript.UnassignedScript unassignedScript in RobotScript.unassignedRobotScript)
        {
            // create a panel to hold the scripts
            GameObject buttonsPanel = Instantiate(buttonsPanelPrefab, Vector3.zero, Quaternion.identity, listContent.transform);

            // create a button for each script
            GameObject buttonScript = Instantiate(buttonScriptPrefab, Vector3.zero, Quaternion.identity, buttonsPanel.transform);
            buttonScript.transform.GetChild(0).GetComponent<TMP_Text>().text = unassignedScript.main.name;
            // when the button with the main script is clicked
            buttonScript.GetComponent<Button>().onClick.AddListener(() =>
            {
                //create a new robot
                Robot robot = new Robot(Color.red, "Robot", 1000, false);
                Manager.instance.listRobot.AddChoice(robot.id, robot.ConvertToListElement());
                Manager.instance.listRobot.Select(robot.id);

                // add the main script 
                robot.MainScript = unassignedScript.main;
                unassignedScript.main.robot = robot;
                List.ListElement element = robot.AddScript(unassignedScript.main);
                Manager.instance.list.AddChoice(element);
                Manager.instance.list.SelectLast();
                Manager.instance.onScriptAdded?.Invoke(this, EventArgs.Empty);

                // add all other children of the main script
                foreach (RobotScript rs in unassignedScript.childrens)
                {
                    rs.robot = robot;
                    element = robot.AddScript(rs);
                    Manager.instance.list.AddChoice(element);
                    Manager.instance.list.SelectLast();
                    Manager.instance.onScriptAdded?.Invoke(this, EventArgs.Empty);
                }
                Manager.instance.ChangeRobotSettings();
                this.PopUpClose();
            });
            foreach (RobotScript rs in unassignedScript.childrens)
            {
                buttonScript = Instantiate(buttonScriptPrefab, Vector3.zero, Quaternion.identity, buttonsPanel.transform);
                buttonScript.transform.GetChild(0).GetComponent<TMP_Text>().text = rs.name;
                // called when a children script button is clicked
                buttonScript.GetComponent<Button>().onClick.AddListener(() =>
                {
                    addScriptAction.Invoke(rs);
                });
            }
        }
        // end tpi
    }

    public void PopUpClose()
    {
        Destroy(this.gameObject);
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
            PopUpWarning sw = WindowsManager.InstantiateWindow((int)Enum.Parse(typeof(WindowsManager.popUp), "saveWarning"), Manager.instance.canvas.transform).GetComponent<PopUpWarning>();
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
