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

    public void ShowInfo(bool isAnError, string text)
    {
        if (isAnError)
            ShowError();
        else
            ShowValid();

        infoText.text = text;
    }

    public void ShowError()
    {
        validImage.SetActive(false);
        errorImage.SetActive(true);
    }

    public void ShowValid()
    {
        errorImage.SetActive(false);
        validImage.SetActive(true);
    }
}