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

// resize node's graphicals elements
public class ThreeElementNodeVisual : MonoBehaviour
{
    public Transform nodeRoot;
    public BoxCollider2D nodeCollider;
    public RectTransform canvas;
    public bool needResize = true;
    public bool verticalResize = false;

    public RectTransform leftSide; // or top
    public RectTransform rightSide; // or bottom
    public RectTransform middleSide; // middleSide is dumb but changing it will imply that I have to change all the nodes middle object reference in the inspector 

    public GameObject[] objectWithAdaptScript;
    private List<AdaptCollider> adaptColliders = new List<AdaptCollider>();

    private void Start()
    {
        foreach (GameObject obj in objectWithAdaptScript)
        {
            adaptColliders.AddRange(obj.GetComponents<AdaptCollider>());
        }
        Resize();
    }

    public void Resize()
    {
        if(needResize)
        {
            if(verticalResize)
            {
                middleSide.sizeDelta = new Vector2(0, canvas.rect.height - (rightSide.rect.height + leftSide.rect.height));
            }
            else
            {
                middleSide.sizeDelta = new Vector2(canvas.rect.width - (rightSide.rect.width + leftSide.rect.width), 0);
            }
        }
        if(nodeCollider != null)
            nodeCollider.size = new Vector2(canvas.rect.width*canvas.localScale.x, middleSide.rect.height * canvas.localScale.y);
        //canvas.position = new Vector2(nodeRoot.position.x, nodeRoot.position.y);

        foreach (AdaptCollider adaptCollider in adaptColliders)
        {
            adaptCollider.Resize();
        }
    }
}
