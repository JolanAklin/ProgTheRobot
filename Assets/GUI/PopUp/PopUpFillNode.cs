using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class PopUpFillNode : PopUp
{
    public Action cancelAction;
    public Action OkAction;

    public List<PopUpNodeInput> customInputFields = new List<PopUpNodeInput>();

    public GameObject errorImage;
    public GameObject validImage;
    public TMP_Text infoText;

    public void Cancel()
    {
        cancelAction();
    }
    public void Ok()
    {
        bool validated = true;
        foreach (PopUpNodeInput customInput in customInputFields)
        {
            customInput.Validate();
            if(customInput.validation.validationStatus == Validator.ValidationStatus.KO)
            {
                validated = false;
                break;
            }
        }
        if(validated)
            OkAction();
        else
        {
            ShowError();
            infoText.text = "not all inputs are valid";
        }
    }

    /// <summary>
    /// Display a text with a valid or an error logo
    /// </summary>
    /// <param name="isAnError">If set to true, it will display an error logo</param>
    /// <param name="text">The text to show</param>
    public void ShowInfo(bool isAnError, string text)
    {
        if (isAnError)
            ShowError();
        else
            ShowValid();

        infoText.text = text;
    }

    /// <summary>
    /// Will show an error sign
    /// </summary>
    public void ShowError()
    {
        validImage.SetActive(false);
        errorImage.SetActive(true);
    }

    /// <summary>
    /// Will show an valid sign
    /// </summary>
    public void ShowValid()
    {
        errorImage.SetActive(false);
        validImage.SetActive(true);
    }
}