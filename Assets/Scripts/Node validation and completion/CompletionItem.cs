using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Image))]
public class CompletionItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text text;
    public string toReplace;
    public Color defaultColor;
    public Color selectColor;
    public TMP_Text completionIndication;
    public List<CompletionIndication> completionIndications = new List<CompletionIndication>();
    public CompletionType type;
    private Image image;

    public CompletionMenu completionMenu;
    public PopUpFillNode popUpFillNode;

    [Serializable]
    public class CompletionIndication
    {
        public CompletionType type;
        public string indication;
    }

    public enum CompletionType
    {
        integer,
        boolean,
        sub,
    }

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Start()
    {
        foreach (CompletionIndication indication in completionIndications)
        {
            if (indication.type == type)
            {
                completionIndication.text = indication.indication;
                break;
            }
        }
    }

    public void Complete()
    {
        popUpFillNode.Complete(text.text, toReplace);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Complete();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        completionMenu.SelectCompletionItem(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        completionMenu.SelectCompletionItem(this);
    }

    public void ChangeColor(bool isSelected)
    {
        if(isSelected)
            image.color = selectColor;
        else
            image.color = defaultColor;
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }
}
