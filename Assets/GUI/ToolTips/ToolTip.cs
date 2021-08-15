using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    public bool isToolTipOpened { get; private set; }
    protected RectTransform rectTransform;

    [SerializeField] protected RectTransform bgRectTransform;
    [SerializeField] protected RectTransform canvasRectTransform;

    protected void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    protected void MoveToolTip()
    {
        Vector2 anchPos = Input.mousePosition / canvasRectTransform.localScale.x;

        if(bgRectTransform.anchorMin.y == 0 && bgRectTransform.anchorMax.y == 0)
        {
            if (anchPos.x + bgRectTransform.rect.width > canvasRectTransform.rect.width)
            {
                //tooltip is outside the canvas on the right side
                anchPos.x = canvasRectTransform.rect.width - bgRectTransform.rect.width;
            }

            if (anchPos.y + bgRectTransform.rect.height > canvasRectTransform.rect.height)
            {
                //tooltip is outside the canvas on the top side
                anchPos.y = canvasRectTransform.rect.height - bgRectTransform.rect.height;
            }

            if (anchPos.x < 0)
            {
                //tooltip is outside the canvas on the left side
                anchPos.x = 0;
            }
            if (anchPos.y < 0)
            {
                //tooltip is outside the canvas on the bottom side
                anchPos.y = 0;
            }
        }
        else if(bgRectTransform.anchorMin.y == 1 && bgRectTransform.anchorMax.y == 1)
        {
            if (anchPos.x + bgRectTransform.rect.width > canvasRectTransform.rect.width)
            {
                //tooltip is outside the canvas on the right side
                anchPos.x = canvasRectTransform.rect.width - bgRectTransform.rect.width;
            }

            if (anchPos.y > canvasRectTransform.rect.height)
            {
                //tooltip is outside the canvas on the top side
                anchPos.y = canvasRectTransform.rect.height;
            }

            if (anchPos.x < 0)
            {
                //tooltip is outside the canvas on the left side
                anchPos.x = 0;
            }
            if (anchPos.y - bgRectTransform.rect.height < 0)
            {
                //tooltip is outside the canvas on the bottom side
                anchPos.y = bgRectTransform.rect.height;
            }
        }

        rectTransform.anchoredPosition = anchPos;
    }

    public void ShowToolTip()
    {
        MoveToolTip();
        isToolTipOpened = true;
        gameObject.SetActive(true);
    }
    public void HideToolTip()
    {
        isToolTipOpened = false;
        gameObject.SetActive(false);
    }
}
