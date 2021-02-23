using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Linq;

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
        buttonImage.color = Robot.robots[Robot.idSelected].color;
    }

    public void Select(int id)
    {
        Button button = buttons[id];
        ListElement choice = choices[id];
        ButtonClicked(button);
        if (!choice.isAddRobot)
            button.onClick?.Invoke();
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
