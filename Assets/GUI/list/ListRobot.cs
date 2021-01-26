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

    // Start is called before the first frame update
    void Start()
    {
        LoadChoice();
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
            Button button = Instantiate(listButton, Content.transform).GetComponent<Button>();
            Image buttonImage = button.GetComponentInChildren<Image>();
            buttonImage.color = choice.robotColor;
            button.colors = colorBlockBase;
            button.onClick.AddListener(() => ButtonClicked(button));
            button.onClick.AddListener(choice.actionOnClick);
            buttons.Add(button);
            if(defaultSelectedIndex == i)
                ButtonClicked(button);
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
