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

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(BoxCollider2D))]
public class LoopArea : MonoBehaviour
{
    public GameObject parent;
    public Canvas nodeCanvas;
    public Image image;
    new public BoxCollider2D collider;
    private RectTransform rectTransform;

    private void OnValidate()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    //start tpi
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(CalculateTopLeftPoint(), 0.1f);
    //    Gizmos.DrawSphere(CalculateBottomRightPoint(), 0.1f);
    //}

    /// <summary>
    /// Calculate the top left corner position of this object
    /// </summary>
    /// <returns>The top left corner position</returns>
    private Vector3 CalculateTopLeftPoint()
    {
        Vector3 position = transform.position;
        position += new Vector3(-rectTransform.rect.width / 200, rectTransform.rect.height/200, 0);
        return position;
    }
    /// <summary>
    /// Calculate the bottom right corner position of this object
    /// </summary>
    /// <returns>The bottom right corner position</returns>
    private Vector3 CalculateBottomRightPoint()
    {
        Vector3 position = transform.position;
        position += new Vector3(rectTransform.rect.width / 200, -rectTransform.rect.height / 200, 0);
        return position;
    }
    /// <summary>
    /// Give the left border of the loopArea
    /// </summary>
    /// <returns>left border</returns>
    public float Left()
    {
        return CalculateTopLeftPoint().x;
    }
    /// <summary>
    /// Give the right border of the loopArea
    /// </summary>
    /// <returns>right border</returns>
    public float Right()
    {
        return CalculateBottomRightPoint().x;
    }
    /// <summary>
    /// Give the top border of the loopArea
    /// </summary>
    /// <returns>top border</returns>
    public float Top()
    {
        return CalculateTopLeftPoint().y;
    }
    /// <summary>
    /// Give the bottom border of the loopArea
    /// </summary>
    /// <returns>bottom border</returns>
    public float Bottom()
    {
        return CalculateBottomRightPoint().y;
    }
    //end tpi
}
