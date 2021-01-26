using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Language;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public static Manager instance;

    public event EventHandler OnLanguageChanged;

    public GameObject canvas;

    //change the resolution of the canvas when screen resolution changes
    private CanvasScaler canvasScaler;
    private Resolution res;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }

        canvasScaler = canvas.GetComponent<CanvasScaler>();
        res.height = Screen.height;
        res.width = Screen.width;
        canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
    }

    private void Update()
    {
        if(res.height != Screen.height || res.width != Screen.width)
        {
            canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
            res.height = Screen.height;
            res.width = Screen.width;

        }
    }

    private void Start()
    {
        Translation.Init();
        Translation.LoadData("fr");
        OnLanguageChanged?.Invoke(instance, EventArgs.Empty);

        // demo of showing a popup
        //PopUpRobot pr = Instantiate(WindowsManager.instance.popUpWindowsDict[(int)Enum.Parse(typeof(WindowsManager.popUp), "robotModif")], canvas.transform).GetComponent<PopUpRobot>();
        testList();
    }

    public void ChangeLanguage(ToggleScript toggle)
    {
        if(toggle.Value)
        {
            Translation.LoadData("eng");
            OnLanguageChanged?.Invoke(instance, EventArgs.Empty);
        }else
        {
            Translation.LoadData("fr");
            OnLanguageChanged?.Invoke(instance, EventArgs.Empty);
        }
    }


    // testing the new list obj
    public List list;
    private void testList()
    {
        List<List.ListElement> elements = new List<List.ListElement>();
        elements.Add(new List.ListElement { displayedText = "test", actionOnClick = () => {Debug.Log("test");} });
        elements.Add(new List.ListElement { displayedText = "sdf", actionOnClick = () => {Debug.Log("234");} });
        elements.Add(new List.ListElement { displayedText = "qathdf sdf g", actionOnClick = () => {Debug.Log("25");} });
        list.Init(elements, 0);
    }

}
