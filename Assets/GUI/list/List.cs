using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.UI;
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

    // Start is called before the first frame update
    void Start()
    {
    }

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

    public Button CreateChoice(ListElement choice)
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
