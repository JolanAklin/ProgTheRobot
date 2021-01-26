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

    //change the resolution of the canvas when screen resolution changes
    public CanvasScaler canvasScaler;
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

}
