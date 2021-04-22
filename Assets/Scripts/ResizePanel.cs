// Copyright 2021 Jolan Aklin

//This file is part of Prog the robot.

//Prog the robot is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 3 of the License.

//Prog the robot is distributed in the hope that it will be useful,
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

public class ResizePanel : MonoBehaviour
{
    public GameObject panelToResize;
    public bool horizontal = true;
    private LayoutElement layoutElement;
    private RectTransform rectTransform;
    private RectTransform resizePanelRectTransform;

    private Action moveAction;

    public string cursorType;

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
