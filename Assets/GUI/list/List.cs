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
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class List : MonoBehaviour
{

    public class ListElement
    {
        public bool isAddScript;
        public string displayedText;
        public UnityAction actionOnClick;
    }

    private uint defaultSelectedIndex = 0;
    private List<ListElement> choices = new List<ListElement>();
    public GameObject listButton;
    private List<Button> buttons = new List<Button>();
    public ColorBlock colorBlockBase;
    public ColorBlock colorBlockSelected;
    private Button currentSelectedButton;
    private Button addButton;
    public GameObject Content;

    public void Init(List<ListElement> listChoices, uint defaulSelected)
    {
        defaultSelectedIndex = defaulSelected;
        choices = listChoices;
        LoadChoice();
    }

    public void ChangeList(List<ListElement> listChoices, uint defaulSelected)
    {
        foreach (Button button in buttons)
        {
            Destroy(button.gameObject);
        }
        buttons.Clear();
        defaultSelectedIndex = defaulSelected;
        choices = listChoices;
        LoadChoice();
    }

    public void AddChoice(ListElement element)
    {
        choices.Add(element);
        CreateChoice(element);
    }

    private Button CreateChoice(ListElement choice)
    {
        Button button = Instantiate(listButton, Content.transform).GetComponent<Button>();
        if (!choice.isAddScript)
        {
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            buttonText.text = choice.displayedText;
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
        buttons.Add(button);
        if(addButton != null)
        {
            addButton.transform.SetAsLastSibling();
        }
        return button;
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

    // create all button from the choices list
    private void LoadChoice()
    {
        if (defaultSelectedIndex >= choices.Count)
            defaultSelectedIndex = 0;
        if (defaultSelectedIndex < 0)
            defaultSelectedIndex = 0;

        int i = 0;
        foreach (ListElement choice in choices)
        {
            Button button = CreateChoice(choice);
            if(defaultSelectedIndex == i)
            {
                ButtonClicked(button);
                if(!choice.isAddScript)
                    button.onClick?.Invoke();
            }
            i++;
        }
    }

    public void Select(int id)
    {
        Button button = buttons[id];
        ListElement choice = choices[id];
        ButtonClicked(button);
        if (!choice.isAddScript)
            button.onClick?.Invoke();
    }
    public void SelectLast()
    {
        Button button = buttons.Last();
        ListElement choice = choices.Last();
        ButtonClicked(button);
        if (!choice.isAddScript)
            button.onClick?.Invoke();
    }

    // Called when a button from the list is clicked
    public void ButtonClicked(Button sender)
    {
        if(currentSelectedButton != null)
            currentSelectedButton.colors = colorBlockBase;
        sender.colors = colorBlockSelected;
        currentSelectedButton = sender;
    }
}
