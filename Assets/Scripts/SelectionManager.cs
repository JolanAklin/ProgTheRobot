using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// start tpi
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
        // reset the selection if not all the script are in the same script
        if (selectedNodesInScript == -1)
            selectedNodesInScript = selectedNode.rs.id;
        if (selectedNodes.Count >= 1)
            if (SelectedNodes[0].parentId != selectedNode.parentId)
                ResetSelection();

        if(selectedNode.rs.id != selectedNodesInScript)
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
    /// Put the current selection in the copy buffer
    /// </summary>
    public void SelectionToCopyBuffer()
    {
        if(SelectedNodes.Count > 0)
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
        if(nodesToCopy.Length > 0)
        {
            int idDelta;
            List<Nodes> nodesToCopyAll = new List<Nodes>();
            foreach (Nodes nodeToCopy in nodesToCopy)
            {
                nodesToCopyAll.AddRange(GetAllNodes(nodeToCopy));
            }
            nodesToCopyAll.AddRange(nodesToCopy);
            Nodes[] clones = ScriptCloner.CloneNodes(nodesToCopyAll.ToArray(), robotScript, out idDelta);

            List<GameObject> nodeClones = new List<GameObject>();


            ResetSelection();
            foreach (Nodes node in clones)
            {
                AddNodeToSelection(node, false);
                nodeClones.Add(node.gameObject);
            }
            robotScript.nodes.AddRange(nodeClones);
            foreach (Nodes node in selectedNodes)
            {
                if(node.parentId > -1)
                {
                    Nodes.NodesDict[node.parentId].NodesInsideLoop.Remove(node);
                    node.parentId = -1;
                }
                node.ParentLoopArea = null;
            }
            UIRaycaster.instance.MoveNode();
        }
    }

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

    //private void CreateSpline()
    //{
    //    Transform nodeHolder = GameObject.FindGameObjectWithTag("NodeHolder").transform;
    //    GameObject[] clonedSplines = new GameObject[splinesToClone.Length];
    //    int i = 0;
    //    foreach (GameObject splineToClone in splinesToClone)
    //    {
    //        SplineManager.SerializedSpline serializedSpline = splineToClone.GetComponent<SplineManager>().SerializeSpline();

    //        // change the node and script to the new cloned ones
    //        serializedSpline.idNodeStart += idDelta;
    //        serializedSpline.idNodeEnd += idDelta;
    //        serializedSpline.robotScriptId = robotScript.id;

    //        // create and set the right value for the spline
    //        GameObject splineLinkInstance = Instantiate(SaveManager.instance.splineLink, new Vector3(0, 0, -899), Quaternion.identity, nodeHolder);
    //        SplineManager splineManager = splineLinkInstance.GetComponent<SplineManager>();
    //        splineManager.DeSerializeSpline(serializedSpline);
    //        clonedSplines[i] = splineLinkInstance;
    //        i++;
    //    }
    //    return clonedSplines;
    //}
}
// end tpi
