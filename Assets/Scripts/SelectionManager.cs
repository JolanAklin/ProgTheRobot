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

    private Nodes[] nodeToCopy;

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
        if(selectedNodes.Count > 0)
        {
            nodeToCopy = new Nodes[SelectedNodes.Count];
            Array.Copy(selectedNodes.ToArray(), nodeToCopy, selectedNodes.Count);
        }
    }

    /// <summary>
    /// Paste the copy buffer
    /// </summary>
    /// <param name="robotScript">Where the copy buffer will be passed</param>
    public void PasteCopyBuffer(RobotScript robotScript)
    {
        if(nodeToCopy.Length > 0)
        {
            int idDelta;
            Nodes[] clones = ScriptCloner.CloneNodes(nodeToCopy, robotScript, out idDelta);

            List<GameObject> nodeClones = new List<GameObject>();


            ResetSelection();
            foreach (Nodes node in clones)
            {
                AddNodeToSelection(node, false);
                nodeClones.Add(node.gameObject);
            }
            robotScript.nodes = nodeClones;
            UIRaycaster.instance.MoveNode();
        }
    }
}
// end tpi
