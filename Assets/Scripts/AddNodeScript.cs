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
using System;
using UnityEngine.UI;

public class AddNodeScript : MonoBehaviour
{
    private Dictionary<int, Action> nodes = new Dictionary<int, Action>(); // define which action is done for what node when clicked on the menu
    private Transform nodeHolder; // nodes will be children of this object
    public List<nodeObject> nodeObjects = new List<nodeObject>();

    // object used to fill the list of node object in the inspector
    [Serializable]
    public class nodeObject
    {
        public string nodeType;
        public GameObject gameObject;
    }

    // create the add node panel and sets it's actions
    private void Start()
    {
        nodeHolder = GameObject.FindGameObjectWithTag("NodeHolder").transform;

        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        // enumerate over the node type enum in the Nodes class
        foreach (Nodes.NodeTypes nodeType in (Nodes.NodeTypes[])Enum.GetValues(typeof(Nodes.NodeTypes)))
        {
            AddAction(nodeType.ToString(), () =>
            {
                // test if there is already a start node on the script, if yes, the script won't allow the creation of another one
                if(RobotScript.robotScripts[Manager.instance.currentlySelectedScript].nodeStart != false && nodeType.ToString() == "start")
                {
                    Debugger.Log("Il ne peut y avoir qu'un seul bloc de départ");
                }else
                {
                    Vector3 spawnPos = Round(NodeDisplay.instance.nodeCamera.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono),1);
                    spawnPos.z = 0;
                    GameObject node = nodeObjects.Find(x => x.nodeType == nodeType.ToString()).gameObject; // find the correct gameobject to instantiate
                    GameObject instantiatedNode = Instantiate(node, spawnPos, Quaternion.identity, nodeHolder);

                    // add the start node to the right robotscript for execution purposes
                    if (nodeType.ToString() == "start")
                    {
                        RobotScript.robotScripts[Manager.instance.currentlySelectedScript].nodeStart = instantiatedNode.GetComponent<Nodes>();
                    }

                    node.transform.position = spawnPos;
                    RobotScript.robotScripts[Manager.instance.currentlySelectedScript].nodes.Add(instantiatedNode);
                    Nodes nodeScript = instantiatedNode.GetComponent<Nodes>();
                    nodeScript.rs = RobotScript.robotScripts[Manager.instance.currentlySelectedScript]; // make the node aware in which robotScript he is

                    SelectionManager.instance.AddNodeToSelection(nodeScript);
                    nodeScript.StartMove();
                }
                canvas.GetComponent<UIRaycaster>().panelOpen = false;
                Destroy(this.gameObject);
            });
        }
    }

    // save the required action to create each node
    public void AddAction(string nodeTypeName, Action action)
    {
        int nodeType = (int)Enum.Parse(typeof(Nodes.NodeTypes), nodeTypeName);
        if (nodes.ContainsKey(nodeType))
        {
            nodes[nodeType] = action;
        }else
        {
            nodes.Add(nodeType, action);
        }
    }

    // do the action stored for the required node
    public void DoAction(string nodeType)
    {
        nodes[(int)Enum.Parse(typeof(Nodes.NodeTypes), nodeType)]();
    }

    private Vector3 Round(Vector3 vector3, int round)
    {
        return new Vector3((float)Math.Round(vector3.x, round), (float)Math.Round(vector3.y,round), (float)Math.Round(vector3.z,round));
    }
}
