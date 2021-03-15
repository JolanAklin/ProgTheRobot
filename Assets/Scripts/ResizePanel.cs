using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ResizePanel : MonoBehaviour
{
    public GameObject panelToResize;
    public bool horizontal = true;
    private LayoutElement layoutElement;
    private RectTransform rectTransform;
    private RectTransform resizePanelRectTransform;

    private Action moveAction;

    private void Start()
    {
        layoutElement = panelToResize.GetComponent<LayoutElement>();
        rectTransform = panelToResize.GetComponent<RectTransform>();
        resizePanelRectTransform = this.GetComponent<RectTransform>();

        
    }
    private void Update()
    {
        if(rectTransform.rect.width > 0 && horizontal)
        {
            resizePanelRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, rectTransform.rect.width, resizePanelRectTransform.rect.width);
        }
        if (rectTransform.rect.height > 0 && !horizontal)
        {
            resizePanelRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, rectTransform.rect.height, resizePanelRectTransform.rect.height);
        }
        moveAction?.Invoke();

        if(Input.GetMouseButtonUp(0))
        {
            EndMove();
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    public void StartMove()
    {
        if (moveAction == null)
            moveAction = () => { Move(); };
    }

    public void EndMove()
    {
        moveAction = null;
    }

    private void Move()
    {
        if(horizontal)
        {
            resizePanelRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, Input.mousePosition.x, resizePanelRectTransform.rect.width);
            layoutElement.preferredWidth = resizePanelRectTransform.anchoredPosition.x;
            resizePanelRectTransform.anchoredPosition = new Vector2(rectTransform.rect.width,0);
        }
        else
        {
            resizePanelRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, Input.mousePosition.y, resizePanelRectTransform.rect.height);
            layoutElement.preferredHeight = resizePanelRectTransform.anchoredPosition.y - 25;
            resizePanelRectTransform.anchoredPosition = new Vector2(0, rectTransform.rect.height);
        }
    }
}
