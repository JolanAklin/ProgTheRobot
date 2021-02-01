using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

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
    private List<ListElement> choices = new List<ListElement>();
    public GameObject listButton;
    private List<Button> buttons = new List<Button>();
    public ColorBlock colorBlockBase;
    public ColorBlock colorBlockSelected;
    private Button currentSelectedButton;
    public GameObject Content;
    private Button addButton;

    public void Init(List<ListElement> listChoices, uint defaulSelected)
    {
        defaultSelectedIndex = defaulSelected;
        choices = listChoices;
        LoadChoice();
    }

    public void AddChoice(ListElement listElement)
    {
        choices.Add(listElement);
        CreateChoice(listElement);
    }

    private Button CreateChoice(ListElement choice)
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
        buttons.Add(button);
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
        foreach (ListElement choice in choices)
        {
            Button button = CreateChoice(choice);
            if(defaultSelectedIndex == i)
            {
                ButtonClicked(button);
                button.onClick?.Invoke();
            }
            i++;
        }
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
