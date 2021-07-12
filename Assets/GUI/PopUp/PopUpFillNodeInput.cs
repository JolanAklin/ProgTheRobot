using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class PopUpFillNodeInput : MonoBehaviour, IPointerClickHandler
{
    public Camera mainCamera;
    public TMP_InputField inputField;
    public PopUpFillNode popUpFillNode;

    public void OnPointerClick(PointerEventData eventData)
    {
        popUpFillNode.ShowError(inputField.caretPosition);
    }
}
