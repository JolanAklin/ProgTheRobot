using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class SettingsOptions : MonoBehaviour
{
    public string filePath;
    public Action okAction;


    public void OnEndEditFilePath(TMP_InputField inputField)
    {
        filePath = inputField.text;
    }

    public void SetOkAction(Action action)
    {
        okAction = action;
    }
    public void Ok()
    {
        okAction();
    }
}
