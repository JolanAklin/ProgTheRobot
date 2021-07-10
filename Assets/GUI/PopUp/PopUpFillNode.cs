using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System.Text.RegularExpressions;
using System;

public class PopUpFillNode : PopUp
{
    public TMP_Text validationTypeText;
    public TMP_InputField input;
    public Validator.ValidationType validationType;

    private Validator.ValidationReturn validationReturn;

    public Action cancelAction;
    public Action OkAction;

    /// <summary>
    /// Initialise the pop up
    /// </summary>
    /// <param name="validationType">What type of validation to use</param>
    public void Init(Validator.ValidationType validationType)
    {
        this.validationType = validationType;
        validationTypeText.text = validationType.ToString();
    }

    /// <summary>
    /// Validate the input
    /// </summary>
    public void Validate()
    {
        string toValidate = input.text;
        toValidate = toValidate.Replace("<color=red><b>", "");
        toValidate = toValidate.Replace("</b></color>", "");
        toValidate = FormatInput(toValidate);
        input.text = toValidate;
        Display(Validator.Validate(validationType, toValidate));
    }

    /// <summary>
    /// Format the string
    /// </summary>
    /// <param name="input">the string to format</param>
    /// <returns>The formated string</returns>
    private string FormatInput(string input)
    {
        input = input.Replace("=", " = ");
        input = input.Replace("<", " < ");
        input = input.Replace(">", " > ");
        input = input.Replace("<=", " <= ");
        input = input.Replace(">=", " >= ");
        input = input.Replace("<>", " <> ");
        input = input.Replace("+", " + ");
        input = input.Replace("-", " - ");
        input = input.Replace("*", " * ");
        input = input.Replace("/", " / ");
        input = input.Replace("(", " ( ");
        input = input.Replace(")", " ) ");

        string pattern = @"\s+";
        input = Regex.Replace(input, pattern, " ");
        return input;
    }

    /// <summary>
    /// Display errors on the input field
    /// </summary>
    /// <param name="validationReturn">The ValidationReturn returned from the validator</param>
    public void Display(Validator.ValidationReturn validationReturn)
    {
        this.validationReturn = validationReturn;
        Dictionary<uint, string> tags = new Dictionary<uint, string>();
        string displayString = input.text;
        uint offset = 0;
        if(validationReturn.validationStatus == Validator.ValidationStatus.KO)
        {
            foreach (KeyValuePair<uint, Validator.ValidationReturn.Error> error in validationReturn.specificErrors)
            {
                if(!(tags.ContainsKey(error.Value.startPos) || tags.ContainsKey(error.Value.endPos)))
                {
                    tags.Add(error.Value.startPos, "<color=red><b>");
                    tags.Add(error.Value.endPos, "</b></color>");
                }
            }
            Dictionary<uint, string> orderedTags = new Dictionary<uint, string>();
            foreach (KeyValuePair<uint, string> kvp in tags.OrderBy(key => key.Key))
            {
                orderedTags.Add(kvp.Key, kvp.Value);
            }
            foreach (KeyValuePair<uint, string> tag in orderedTags)
            {
                displayString = displayString.Insert((tag.Key + offset < displayString.Length) ? (int)(tag.Key + offset) : displayString.Length, tag.Value);
                offset += (uint)tag.Value.Length;
            }
            input.text = displayString;
        }
    }

    /// <summary>
    /// Show error messsage
    /// </summary>
    /// <param name="index">The index where the caret is</param>
    public void ShowError(int index)
    {
        string errorMessage = "";
        if(validationReturn.specificErrors != null && validationReturn.generalErrors != null)
        {
            foreach (var errors in validationReturn.specificErrors)
            {
                if(index >= errors.Key && index < errors.Value.endPos)
                {
                    errorMessage = errors.Value.message;
                    Debug.Log(errorMessage);
                    break;
                }
            }
        }
    }

    public void Cancel()
    {
        cancelAction();
    }
    public void Ok()
    {
        OkAction();
    }

}
