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
    public Color defaultColor;
    public Color selectColor;
    public TMP_Text completionIndication;
    public List<CompletionIndication> completionIndications = new List<CompletionIndication>();
    public LanguageManager.BranchType type;
    private Image image;

    public CompletionMenu completionMenu;
    public PopUpNodeInput popUpFillNodeInput;

    [Serializable]
    public class CompletionIndication
    {
        public LanguageManager.BranchType type;
        public string indication;
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
        popUpFillNodeInput.Complete(text.text, popUpFillNodeInput.toReplace);
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
