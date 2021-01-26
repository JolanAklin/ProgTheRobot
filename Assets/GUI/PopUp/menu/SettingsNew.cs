using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class SettingsNew : MonoBehaviour
{
    public string fileName;
    public Action okAction;


    public void OnEndEditFilename(TMP_InputField inputField)
    {
        fileName = inputField.text;
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
