using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class PopUpReadWrite : MonoBehaviour
{
    private Action okAction;

    public TMP_Text infoText;
    public TMP_InputField inputField;

    public void Init(string text)
    {
        infoText.text = text;
        inputField.interactable = true;
    }
    public void Init(string text, string number)
    {
        infoText.text = text;
        inputField.text = number;
        inputField.interactable = false;
    }

    public void DestroyPopup()
    {
        Destroy(this.gameObject);
    }

    public int value()
    {
        int value;
        if(int.TryParse(inputField.text, out value))
        {
            return value;
        }
        return 0;
    }

    public void CheckValue()
    {
        int temp;
        if(!int.TryParse(inputField.text, out temp))
        {
            infoText.text = "Entrer seulement des nombres";
        }
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
