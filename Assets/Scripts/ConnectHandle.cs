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
using UnityEngine.EventSystems;

public class ConnectHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Nodes node; // which is his parent
    public bool isInput = false;
    [HideInInspector]
    public bool canBeClicked = true;
    [HideInInspector]
    public Image image;

    // used when the node is a loop or a if node
    public bool ifFalse;
    public bool inLoopOut = false;

    public int handleNumber;

    
    public LoopArea loopArea;
    
    private void Awake()
    {
        canBeClicked = true;
        image = GetComponent<Image>();
        Manager.instance.OnSpline += ShowHide; // this event while be triggered when a spline is created or ended
        if (image != null && isInput && !Manager.instance.connectHandleAlwaysShown)
        {
            image.enabled = false;
        }
    }

    private void OnDestroy()
    {
        Manager.instance.OnSpline -= ShowHide;
    }

    // when the handle is clicked, will ask the manager.
    public void Click()
    {
        Nodes nextNode = null;
        if(!isInput)
        {
            if(canBeClicked)
            {
                // call a function in the manager script and give a delegate to perform the right action to set the node id
                if (ifFalse)
                    nextNode = Manager.instance.ConnectNode(isInput, transform, node, (id) => { node.gameObject.GetComponent<NodeIf>().nextNodeIdFalse = id; }, handleNumber) ;
                else if(inLoopOut)
                {
                    NodeForLoop nodeForLoop;
                    if(node.gameObject.TryGetComponent(out nodeForLoop))
                        nextNode = Manager.instance.ConnectNode(isInput, transform, node, (id) => { nodeForLoop.nextNodeInside = id; }, handleNumber);
                    NodeWhileLoop nodeWhileLoop;
                    if (node.gameObject.TryGetComponent(out nodeWhileLoop))
                        nextNode = Manager.instance.ConnectNode(isInput, transform, node, (id) => { nodeWhileLoop.nextNodeInside = id; }, handleNumber);
                }
                else
                    nextNode = Manager.instance.ConnectNode(isInput, transform, node, (id) => { node.nextNodeId = id; }, handleNumber);

                canBeClicked = false;
            }
        }else
        {
            nextNode = Manager.instance.ConnectNode(isInput, transform, node, (id) => {}, handleNumber);
        }

        if (nextNode != null)
        {
            nextNode = null;
            Manager.instance.node = null;
        }
        if (!isInput)
        {
            image.enabled = false;
        }
    }

    /// <summary>
    /// hide the handle
    /// </summary>
    public void Hide()
    {
        image.enabled = false;
    }

    // show and hide the input image (red dot)
    public void ShowHide(object sender, Manager.OnSplineEventArgs e)
    {
        if(isInput)
        {
            if (e.splineStarted)
            {
                image.enabled = true;
            }
            else if(!Manager.instance.connectHandleAlwaysShown)
            {
                image.enabled = false;
            }
        }else if(canBeClicked)
        {
            if (e.splineStarted && !Manager.instance.connectHandleAlwaysShown)
            {
                image.enabled = false;
            }
            else
            {
                image.enabled = true;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            node.preventMove = true;
            Click();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            node.preventMove = false;
        }
    }
}
