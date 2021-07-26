using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class PopUpFillNodeInput : MonoBehaviour
{
    public CustomInputField inputField;
    public PopUpNodeInput popUpNodeInput;

    private int lastCaretPosition = 0;

    private void Update()
    {
        if(inputField.caretPosition != lastCaretPosition)
        {
            popUpNodeInput.ShowError(inputField.caretPosition);
            lastCaretPosition = inputField.caretPosition;
        }
    }
}
