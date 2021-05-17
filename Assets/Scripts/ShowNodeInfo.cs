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
using TMPro;

public class ShowNodeInfo : MonoBehaviour
{
    // start tpi
    public Nodes.NodeTypes nodeType;
    private RectTransform rect;

    private NodeInfo.Info nodeInfo;


    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        nodeInfo = NodeInfo.nodesInfos.Find(x => x.nodeTypes == nodeType);
    }

    private void Update()
    {
        // display the info of a node if the mouse pointer is over the rect
        if(RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition))
        {
            NodeInfo.infoTitle.text = nodeInfo.infoTextTitle;
            NodeInfo.infoDesc.text = nodeInfo.infoText;
        }
    }
    //end tpi
}
