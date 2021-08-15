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

/// <summary>
/// this class will handle the selection of nodes, copy and pasting of nodes
/// </summary>
public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance;

    private int selectedNodesInScript = -1;
    private List<Nodes> selectedNodes = new List<Nodes>();
    public List<Nodes> SelectedNodes { get => selectedNodes; private set => selectedNodes = value; }

    private Nodes[] nodesToCopy;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Add a node to the selection
    /// </summary>
    /// <param name="selectedNode">The node to add to the selection</param>
    /// <param name="startOfSelection">Restart the selection, the node specified will be the first one in the selection</param>
    public void AddNodeToSelection(Nodes selectedNode, bool startOfSelection = true)
    {
        // reset the selection if not all nodes are in the same script
        if (selectedNodesInScript == -1)
            selectedNodesInScript = selectedNode.rs.id;
        // reset the selection if not all nodes have the same parent
        if (selectedNodes.Count >= 1)
            if (SelectedNodes[0].parentId != selectedNode.parentId)
                ResetSelection();

        if (selectedNode.rs.id != selectedNodesInScript)
        {
            ResetSelection();
        }

        if (selectedNode == null)
            return;
        if (startOfSelection)
        {
            ResetSelection();
        }
        SelectedNodes.Add(selectedNode);
        selectedNode.ChangeBorderColor(selectedNode.selectedColor);
    }

    public void ResetSelection()
    {
        selectedNodesInScript = -1;
        // put all the nodes back to the right color
        foreach (Nodes node in selectedNodes)
        {
            if (node.isMoving)
                node.EndMove();
            if (node.NodeErrorCode == Nodes.ErrorCode.notConnected || node.NodeErrorCode == Nodes.ErrorCode.wrongInput)
                node.ChangeBorderColor(node.errorColor);
            else
                node.ChangeBorderColor(node.defaultColor);
        }
        SelectedNodes.Clear();
    }

    /// <summary>
    /// Delete selected nodes and their splines
    /// </summary>
    public void DeleteSelection()
    {
        while(selectedNodes.Count > 0)
        {
            Destroy(selectedNodes[selectedNodes.Count-1].gameObject);
            selectedNodes.Remove(selectedNodes[selectedNodes.Count-1]);
        }
    }

    /// <summary>
    /// Put the current selection in the copy buffer
    /// </summary>
    public void SelectionToCopyBuffer()
    {
        if (SelectedNodes.Count > 0)
        {
            nodesToCopy = new Nodes[SelectedNodes.Count];
            Array.Copy(SelectedNodes.ToArray(), nodesToCopy, SelectedNodes.Count);
        }
    }

    /// <summary>
    /// Paste the copy buffer
    /// </summary>
    /// <param name="robotScript">Where the copy buffer will be passed</param>
    public void PasteCopyBuffer(RobotScript robotScript)
    {
        if (nodesToCopy.Length > 0)
        {
            // get all the node inside other node recursively
            List<Nodes> nodesToCopyAll = new List<Nodes>();
            foreach (Nodes nodeToCopy in nodesToCopy)
            {
                nodesToCopyAll.AddRange(GetAllNodes(nodeToCopy));
            }
            nodesToCopyAll.AddRange(nodesToCopy);
            Dictionary<int, ScriptCloner.OldNodeId> matchOldNewIds;
            Nodes[] clones = ScriptCloner.CloneNodes(nodesToCopyAll.ToArray(), robotScript, out matchOldNewIds);

            // create the spline between nodes
            int i = 0;
            Transform nodeHolder = GameObject.FindGameObjectWithTag("NodeHolder").transform;
            foreach (Nodes node in clones)
            {
                for (int j = 0; j < nodesToCopyAll[i].currentSplines.Length; j++)
                {
                    if (nodesToCopyAll[i].currentSplines[j] != null)
                    {
                        SplineManager.SerializedSpline serializedSpline = nodesToCopyAll[i].currentSplines[j].SerializeSpline();

                        serializedSpline.idNodeStart = node.id;
                        if (j == 0)
                        {
                            if (node.nextNodeId > -1)
                            {
                                serializedSpline.idNodeEnd = node.nextNodeId;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (node.GetType() == typeof(NodeIf))
                            {
                                NodeIf nodeIf = (NodeIf)node;
                                if (nodeIf.nextNodeIdFalse > -1)
                                    serializedSpline.idNodeEnd = nodeIf.nextNodeIdFalse;
                                else
                                    continue;
                            }
                            else if (node.GetType() == typeof(NodeWhileLoop))
                            {
                                NodeWhileLoop nodeWhile = (NodeWhileLoop)node;
                                if (nodeWhile.nextNodeInside > -1)
                                    serializedSpline.idNodeEnd = nodeWhile.nextNodeInside;
                                else
                                    continue;
                            }
                            else if (node.GetType() == typeof(NodeForLoop))
                            {
                                NodeForLoop nodeFor = (NodeForLoop)node;
                                if (nodeFor.nextNodeInside > -1)
                                    serializedSpline.idNodeEnd = nodeFor.nextNodeInside;
                                else
                                    continue;
                            }
                        }
                        serializedSpline.robotScriptId = robotScript.id;

                        // create and set the right value for the spline
                        GameObject splineLinkInstance = Instantiate(SaveManager.instance.splineLink, new Vector3(0, 0, -899), Quaternion.identity, nodeHolder);
                        SplineManager splineManager = splineLinkInstance.GetComponent<SplineManager>();
                        splineManager.DeSerializeSpline(serializedSpline);
                        robotScript.splines.Add(splineLinkInstance);
                        node.currentSplines[j] = splineManager;
                        node.handleStartArray[j].GetComponent<ConnectHandle>().Hide();
                        node.handleStartArray[j].GetComponent<ConnectHandle>().canBeClicked = false;
                    }
                }
                i++;
            }

            List<GameObject> nodeClones = new List<GameObject>();

            ResetSelection();
            foreach (Nodes node in clones)
            {
                AddNodeToSelection(node, false);
                nodeClones.Add(node.gameObject);
            }
            robotScript.nodes.AddRange(nodeClones);
            // remove the confinement zone for the right nodes
            foreach (Nodes node in selectedNodes)
            {
                if (node.parentId > -1)
                {
                    Nodes.NodesDict[node.parentId].NodesInsideLoop.Remove(node);
                    node.parentId = -1;
                }
                node.ParentLoopArea = null;
            }

            foreach (Nodes selectedNode in SelectionManager.instance.SelectedNodes)
            {
                selectedNode.StartMove(true);
            }
        }
    }

    /// <summary>
    /// Get all nodes inside a node
    /// </summary>
    /// <param name="node">The node to get all the ones inside</param>
    /// <returns>A list of node</returns>
    private List<Nodes> GetAllNodes(Nodes node)
    {
        List<Nodes> nodes = new List<Nodes>();
        foreach (Nodes childNode in node.NodesInsideLoop)
        {
            nodes.Add(childNode);
            nodes.AddRange(GetAllNodes(childNode));
        }
        return nodes;
    }
}
