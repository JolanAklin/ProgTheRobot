using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class List : MonoBehaviour
{
    public List<string> choices = new List<string>();
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
        foreach (string choice in choices)
        {
            Button button = Instantiate(listButton, Content.transform).GetComponent<Button>();
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            buttonText.text = choice;
            button.colors = colorBlockBase;
            button.onClick.AddListener(() => ButtonClicked(button));
            buttons.Add(button);
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
