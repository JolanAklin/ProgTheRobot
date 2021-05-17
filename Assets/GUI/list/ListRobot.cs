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
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Linq;
using Cinemachine;

public class ListRobot : MonoBehaviour
{

    public class ListElement
    {
        public bool isAddRobot = false;
        public Color robotColor;
        public UnityAction actionOnClick;
    }

    public uint defaultSelectedIndex;
    //change this list to be able to use robot instead of string
    private Dictionary<int, ListElement> choices = new Dictionary<int, ListElement>();
    public GameObject listButton;
    private Dictionary<int, Button> buttons = new Dictionary<int, Button>();
    public ColorBlock colorBlockBase;
    public ColorBlock colorBlockSelected;
    private Button currentSelectedButton;
    private Button addButton;
    public GameObject Content;

    private CinemachineVirtualCamera robotVcam;

    private void Awake()
    {
        robotVcam = GameObject.FindGameObjectWithTag("RobotVcam").GetComponent<CinemachineVirtualCamera>();
    }

    public void Init(Dictionary<int, ListElement> listChoices, uint defaulSelected)
    {
        defaultSelectedIndex = defaulSelected;
        choices = listChoices;
        LoadChoice();
    }

    public void RemoveRobot(int id)
    {
        choices.Remove(id);
        Destroy(buttons[id].gameObject);
        buttons.Remove(id);
        Manager.instance.list.Clear();
    }

    public void AddChoice(int id, ListElement listElement)
    {
        choices.Add(id, listElement);
        CreateChoice(id, listElement);
    }

    private Button CreateChoice(int id, ListElement choice)
    {
        Button button = Instantiate(listButton, Content.transform).GetComponent<Button>();
        if (!choice.isAddRobot)
        {
            Image buttonImage = button.transform.GetChild(0).GetChild(0).GetComponentInChildren<Image>();
            buttonImage.color = choice.robotColor;
        }
        else
        {
            button.transform.GetChild(0).gameObject.SetActive(false);
            button.transform.GetChild(1).gameObject.SetActive(true);
            addButton = button;
        }
        button.colors = colorBlockBase;
        button.onClick.AddListener(() => ButtonClicked(button));
        button.onClick.AddListener(choice.actionOnClick);
        buttons.Add(id, button);
        if (addButton != null)
        {
            addButton.transform.SetAsLastSibling();
        }
        return button;
    }

    // create all button from the choices list
    private void LoadChoice()
    {
        if (defaultSelectedIndex >= choices.Count)
            defaultSelectedIndex = 0;
        if (defaultSelectedIndex < 0)
            defaultSelectedIndex = 0;

        int i = 0;
        foreach (KeyValuePair<int, ListElement> choice in choices)
        {
            Button button = CreateChoice(choice.Key, choice.Value);
            if(defaultSelectedIndex == i)
            {
                ButtonClicked(button);
                if (!choice.Value.isAddRobot)
                    button.onClick?.Invoke();
            }
            i++;
        }
    }

    public Dictionary<int, ListElement> getChoices()
    {

        Dictionary<int, ListElement> dictChoices = choices.ToDictionary(entry => entry.Key, entry => entry.Value);
        return dictChoices;
    }

    // Called when a button from the list is clicked
    public void ButtonClicked(Button sender)
    {
        if(currentSelectedButton != null)
            currentSelectedButton.colors = colorBlockBase;
        sender.colors = colorBlockSelected;
        currentSelectedButton = sender;
    }

    public void UpdateButtonColor()
    {
        Image buttonImage = buttons[Robot.idSelected].transform.GetChild(0).GetChild(0).GetComponentInChildren<Image>();
        buttonImage.color = Robot.robots[Robot.idSelected].Color;
    }

    public void Select(int id)
    {
        Button button = buttons[id];
        ListElement choice = choices[id];
        ButtonClicked(button);
        if (!choice.isAddRobot)
        {
            button.onClick?.Invoke();
            CameraTargetRobot(id);
        }
    }

    public void SelectFirst()
    {
        int id = buttons.ElementAt(1).Key;
        Button button = buttons[id];
        ListElement choice = choices[id];
        ButtonClicked(button);
        if (!choice.isAddRobot)
        {
            button.onClick?.Invoke();
            CameraTargetRobot(id);
        }
    }
    // start tpi
    /// <summary>
    /// Target the robot with the camera
    /// </summary>
    /// <param name="id">The robot to target</param>
    public void CameraTargetRobot(int id)
    {
        if(robotVcam != null)
        {
            Transform cameraPoint = Robot.robots[id].robotManager.cameraPoint.transform;
            cameraPoint.GetComponent<CameraController>().SetDefaultPosRot();
            robotVcam.Follow = cameraPoint;
            robotVcam.LookAt = cameraPoint;
        }
    }
    //end tpi

    public void ChangeChoiceColor(int id, Color color)
    {
        choices[id].robotColor = color;
    }

    public int Count()
    {
        return buttons.Count;
    }

    public void Clear()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }
        buttons.Clear();
        choices.Clear();
        defaultSelectedIndex = 0;
        addButton = null;
        currentSelectedButton = null;
    }
}
