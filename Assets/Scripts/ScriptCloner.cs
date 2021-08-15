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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// start tpi
public class ScriptCloner : MonoBehaviour
{
    /// <summary>
    /// Clone the specified script
    /// </summary>
    /// <param name="robotScriptToClone">The source to clone from</param>
    /// <returns>The cloned script</returns>
    public static RobotScript CloneScript(RobotScript robotScriptToClone)
    {
        RobotScript robotScriptClone = new RobotScript();
        robotScriptClone.name = robotScriptToClone.name;
        robotScriptClone.isMainScript = robotScriptToClone.isMainScript;

        // clone nodes
        Nodes[] nodesToClone = new Nodes[robotScriptToClone.nodes.Count];
        int i = 0;
        foreach (GameObject node in robotScriptToClone.nodes)
        {
            nodesToClone[i] = node.GetComponent<Nodes>();
            i++;
        }
        Dictionary<int, OldNodeId> matchOldNewIds;
        Nodes[] clonedNodesArray = CloneNodes(nodesToClone, robotScriptClone, out matchOldNewIds);
        foreach (Nodes node in clonedNodesArray)
        {
            robotScriptClone.nodes.Add(node.gameObject);
        }

        // clone splines
        robotScriptClone.splines.AddRange(CloneSpline(robotScriptToClone.splines.ToArray(), robotScriptClone, matchOldNewIds));
        

        return robotScriptClone;
    }

    /// <summary>
    /// Describe node only with his ids;
    /// </summary>
    public class OldNodeId
    {
        public int id;
        public int parentId = -1;
        public int nextNodeId = -1;
        public int nextNodeIdSecondary = -1;
    }

    /// <summary>
    /// Clone nodes inside the node array
    /// </summary>
    /// <param name="nodesToClone">The array containing nodes to clone</param>
    /// <param name="robotScript">The script to clone nodes to</param>
    /// <returns>An array with cloned nodes</returns>
    public static Nodes[] CloneNodes(Nodes[] nodesToClone, RobotScript robotScript, out Dictionary<int, OldNodeId> matchOldNewIds)
    {
        // store the new node id and the old node id
        matchOldNewIds = new Dictionary<int, OldNodeId>();

        Transform nodeHolder = GameObject.FindGameObjectWithTag("NodeHolder").transform;
        Nodes[] clonedNode = new Nodes[nodesToClone.Length];

        int i = 0;

        // create cloned nodes
        foreach (Nodes nodeToClone in nodesToClone)
        {
            GameObject node = SaveManager.instance.nodeObjects.Find(x => x.nodeType == nodeToClone.NodeType.ToString()).gameObject; // find the correct gameobject to instantiate
            GameObject nodeInstance = SaveManager.instance.InstantiateSavedObj(node, nodeToClone.transform.position, Quaternion.identity, nodeHolder);

            Nodes nodeInstanceScript = nodeInstance.GetComponent<Nodes>();
            nodeInstanceScript.SetId();
            nodeInstanceScript.rs = robotScript;

            OldNodeId oldNodeId = new OldNodeId() { id = nodeToClone.id, nextNodeId = nodeToClone.nextNodeId, parentId = nodeToClone.parentId };

            // add the node to the old and new node match
            if (nodeToClone.GetType() == typeof(NodeIf))
            {
                NodeIf nodeIf = (NodeIf)nodeToClone;
                oldNodeId.nextNodeIdSecondary = nodeIf.nextNodeIdFalse;
            }else if (nodeToClone.GetType() == typeof(NodeWhileLoop))
            {
                NodeWhileLoop nodeWhile = (NodeWhileLoop)nodeToClone;
                oldNodeId.nextNodeIdSecondary = nodeWhile.nextNodeInside;
            }else if (nodeToClone.GetType() == typeof(NodeForLoop))
            {
                NodeForLoop nodeFor = (NodeForLoop)nodeToClone;
                oldNodeId.nextNodeIdSecondary = nodeFor.nextNodeInside;
            }
            matchOldNewIds.Add(nodeInstanceScript.id, oldNodeId);

            // change the id of the serialized object to keep the right id
            Nodes.SerializableNode serializableNode = nodeToClone.SerializeNode();
            serializableNode.id = nodeInstanceScript.id;

            // set the right value for the node
            nodeInstanceScript.DeSerializeNode(serializableNode);

            if (nodeToClone.NodeType == Nodes.NodeTypes.start)
            {
                robotScript.nodeStart = nodeInstance.GetComponent<Nodes>();
            }
            clonedNode[i] = nodeInstanceScript;
            i++;
        }

        i = 0;
        // update the nextNodeId of all nodes
        foreach (Nodes node in clonedNode)
        {
            // update the node nextnodeid and parentid
            OldNodeId oldNodeId = matchOldNewIds[node.id];
            try
            {
                KeyValuePair<int, OldNodeId> oldNextId = matchOldNewIds.First(x => x.Value.id == oldNodeId.nextNodeId);
                node.nextNodeId = oldNextId.Key;
            }
            catch (Exception) { node.nextNodeId = -1; }

            try
            {
                KeyValuePair<int, OldNodeId> oldParentId = matchOldNewIds.First(x => x.Value.id == oldNodeId.parentId);
                node.parentId = oldParentId.Key;
            }
            catch (Exception) { node.parentId = -1; }


            if (node.GetType() == typeof(NodeIf))
            {
                NodeIf nodeIf = (NodeIf)node;
                try
                {
                    KeyValuePair<int, OldNodeId> oldNextId = matchOldNewIds.First(x => x.Value.id == oldNodeId.nextNodeIdSecondary);
                    nodeIf.nextNodeIdFalse = oldNextId.Key;
                }
                catch (Exception) { nodeIf.nextNodeIdFalse = -1; }
            }
            else if (node.GetType() == typeof(NodeWhileLoop))
            {
                NodeWhileLoop nodeWhile = (NodeWhileLoop)node;
                try
                {
                    KeyValuePair<int, OldNodeId> oldNextId = matchOldNewIds.First(x => x.Value.id == oldNodeId.nextNodeIdSecondary);
                    nodeWhile.nextNodeInside = oldNextId.Key;
                }
                catch (Exception) { nodeWhile.nextNodeInside = -1; }
            }
            else if (node.GetType() == typeof(NodeForLoop))
            {
                NodeForLoop nodeFor = (NodeForLoop)node;
                try
                {
                    KeyValuePair<int, OldNodeId> oldNextId = matchOldNewIds.First(x => x.Value.id == oldNodeId.nextNodeIdSecondary);
                    nodeFor.nextNodeInside = oldNextId.Key;
                }
                catch (Exception) { nodeFor.nextNodeInside = -1; }
            }

            // put the node in his loop
            if (node.parentId > -1)
            {
                if(Nodes.NodesDict[node.parentId].GetType() == typeof(NodeWhileLoop) || Nodes.NodesDict[node.parentId].GetType() == typeof(NodeForLoop))
                {
                    LoopArea loopArea = Nodes.NodesDict[node.parentId].NodesLoopArea;
                    if (!loopArea.parent.transform.IsChildOf(node.transform))
                    {
                        // changing the sorting order of the canvas to ensure that the node is on the top
                        node.Canvas.sortingOrder = loopArea.nodeCanvas.sortingOrder + 1;
                        Nodes parentNode = loopArea.parent.GetComponent<Nodes>();
                        node.parentId = parentNode.id;

                        node.transform.parent = loopArea.parent.transform;
                        float zpos = (loopArea.parent.transform.position.z - 0.001f);
                        node.transform.position = new Vector3(node.transform.position.x, node.transform.position.y, zpos);
                        parentNode.NodesInsideLoop.Add(node);

                        node.ParentLoopArea = loopArea;
                        node.handleStartArray[0].loopArea = node.ParentLoopArea;
                        node.handleEndArray[0].loopArea = node.ParentLoopArea;
                        if (node.NodeType == Nodes.NodeTypes.test)
                        {
                            node.handleStartArray[1].loopArea = node.ParentLoopArea;
                        }
                    }
                }
                else
                {
                    node.parentId = -1;
                }

            }
        }

        return clonedNode;
    }

    /// <summary>
    /// Cloned the given spline array
    /// </summary>
    /// <param name="splinesToClone">The array to clone the splines from</param>
    /// <param name="robotScript">The script that will hold the splines</param>
    /// <param name="idDelta">The idDelta</param>
    /// <returns></returns>
    private static GameObject[] CloneSpline(GameObject[] splinesToClone, RobotScript robotScript, Dictionary<int, OldNodeId> matchOldNewIds)
    {
        Transform nodeHolder = GameObject.FindGameObjectWithTag("NodeHolder").transform;
        GameObject[] clonedSplines = new GameObject[splinesToClone.Length];
        int i = 0;
        foreach (GameObject splineToClone in splinesToClone)
        {
            SplineManager.SerializedSpline serializedSpline = splineToClone.GetComponent<SplineManager>().SerializeSpline();

            // change the node and script to the new cloned ones
            try
            {
                KeyValuePair<int, OldNodeId> oldNextId = matchOldNewIds.First(x => x.Value.id == serializedSpline.idNodeStart);
                serializedSpline.idNodeStart = oldNextId.Key;
            }
            catch (Exception) { }

            try
            {
                KeyValuePair<int, OldNodeId> oldNextId = matchOldNewIds.First(x => x.Value.id == serializedSpline.idNodeEnd);
                serializedSpline.idNodeEnd = oldNextId.Key;
            }
            catch (Exception) { }
            serializedSpline.robotScriptId = robotScript.id;

            // create and set the right value for the spline
            GameObject splineLinkInstance = Instantiate(SaveManager.instance.splineLink, new Vector3(0, 0, -899), Quaternion.identity, nodeHolder);
            SplineManager splineManager = splineLinkInstance.GetComponent<SplineManager>();
            splineManager.DeSerializeSpline(serializedSpline);
            clonedSplines[i] = splineLinkInstance;
            i++;
        }
        return clonedSplines;
    }
}
// end tpi
