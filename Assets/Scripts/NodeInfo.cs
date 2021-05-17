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
using System;
using TMPro;

public class NodeInfo : MonoBehaviour
{
    // start tpi

    [Header("Node infos")]
    public List<Info> infos = new List<Info>();
    public static List<Info> nodesInfos = new List<Info>();

    [Header("Text boxes")]
    public TMP_Text infotitle;
    public TMP_Text infodesc;


    public static TMP_Text infoTitle;
    public static TMP_Text infoDesc;


    private void Awake()
    {
        nodesInfos = infos;
        infoTitle = infotitle;
        infoDesc = infodesc;
    }

    /// <summary>
    /// Infos for the specified node.
    /// </summary>
    [Serializable]
    public class Info
    {
        /// <summary>
        /// the type of node targeted by this info
        /// </summary>
        public Nodes.NodeTypes nodeTypes;
        /// <summary>
        /// Name of the node
        /// </summary>
        public string infoTextTitle;
        /// <summary>
        /// Info on the node
        /// </summary>
        [TextArea]
        public string infoText;
    }
    //end tpi
}
