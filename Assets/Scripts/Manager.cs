using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Language;

public class Manager : MonoBehaviour
{
    public static Manager instance;

    public event EventHandler OnLanguageChanged;

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
    }

    private void Start()
    {
        Translation.Init();
        ChangeLanguage("fr");
    }
    public void ChangeLanguage(string lang)
    {
        Translation.LoadData(lang);
        OnLanguageChanged?.Invoke(instance, EventArgs.Empty);
    }

}
