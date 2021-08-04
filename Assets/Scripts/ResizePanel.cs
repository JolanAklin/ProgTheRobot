// Copyright 2021 Jolan Aklin

//This file is part of Prog The Robot.

//Prog The Robot is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 3 of the License.

//Prog The Robot is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Prog the robot.  If not, see<https://www.gnu.org/licenses/>.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class ResizePanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject panelToResize;
    public bool horizontal = true;
    private LayoutElement layoutElement;
    private RectTransform rectTransform;
    private RectTransform resizePanelRectTransform;

    public CursorManager.CursorDef.CursorTypes cursorType;

    private void Awake()
    {
        layoutElement = panelToResize.GetComponent<LayoutElement>();
        rectTransform = panelToResize.GetComponent<RectTransform>();
        resizePanelRectTransform = this.GetComponent<RectTransform>();

        WindowResized.instance.onWindowResized += PlaceResizePanel;
    }

    private void Start()
    {
        StartCoroutine("PlaceResizeBarAfterDelay");
    }

    IEnumerator PlaceResizeBarAfterDelay()
    {
        yield return new WaitForEndOfFrame();
        PlaceResizePanel(this, EventArgs.Empty);
    }

    private void PlaceResizePanel(object sender, EventArgs e)
    {
        if (rectTransform.rect.width > 0 && horizontal)
        {
            resizePanelRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, rectTransform.rect.width, resizePanelRectTransform.rect.width);
        }
        if (rectTransform.rect.height > 0 && !horizontal)
        {
            resizePanelRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, rectTransform.rect.height, resizePanelRectTransform.rect.height);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorManager.instance.ChangeCursor(cursorType);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorManager.instance.ChangeCursor(CursorManager.CursorDef.CursorTypes.arrow);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        CursorManager.instance.ChangeCursor(cursorType, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        PlaceResizePanel(this, EventArgs.Empty);
        Move();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        CursorManager.instance.UnLockCursorTexture();
        CursorManager.instance.ChangeCursor(CursorManager.CursorDef.CursorTypes.arrow);
    }

    private void Move()
    {
        if(horizontal)
        {
            resizePanelRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, GetPointOnUi.GetMousePosOnUi().x, resizePanelRectTransform.rect.width);
            layoutElement.preferredWidth = resizePanelRectTransform.anchoredPosition.x;
            resizePanelRectTransform.anchoredPosition = new Vector2(rectTransform.rect.width,0);
        }
        else
        {
            resizePanelRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, GetPointOnUi.GetMousePosOnUi().y, resizePanelRectTransform.rect.height);
            layoutElement.preferredHeight = resizePanelRectTransform.anchoredPosition.y - 25;
            resizePanelRectTransform.anchoredPosition = new Vector2(0, rectTransform.rect.height);
        }
    }

}
