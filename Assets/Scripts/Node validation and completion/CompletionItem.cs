using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CompletionItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text text;
    public Color defaultColor;
    public Color selectColor;
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void Complete()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("test");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = selectColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = defaultColor;
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }
}
