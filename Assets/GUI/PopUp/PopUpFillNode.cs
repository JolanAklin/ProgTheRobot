using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System.Text.RegularExpressions;

public class PopUpFillNode : PopUp
{
    public TMP_Text validationTypeText;
    public TMP_InputField input;
    public Validator.ValidationType validationType;

    private Validator.ValidationReturn validationReturn;

    public void Init(Validator.ValidationReturn validationReturn)
    {
        this.validationReturn = validationReturn;
        validationTypeText.text = validationReturn.ToString();
    }

    public void Validate()
    {
        string toValidate = input.text;
        toValidate = toValidate.Replace("<color=red><b>", "");
        toValidate = toValidate.Replace("</b></color>", "");
        toValidate = FormatInput(toValidate);
        input.text = toValidate;
        Validator.InverseKV();
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

    public void Display(Validator.ValidationReturn validationReturn)
    {
        this.validationReturn = validationReturn;
        Dictionary<uint, string> tags = new Dictionary<uint, string>();
        string displayString = input.text;
        uint offset = 0;
        Debug.Log(validationReturn);
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
            Debug.Log(displayString);
        }
    }

    // really need to verify the way the error is displayed, because of the <b> and <color> attribute. They could move the caret to a different place that is in the validation return from the validator
    public void ShowError(int index)
    {
        string errorMessage = "";
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
