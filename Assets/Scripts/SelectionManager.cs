using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// start tpi
/// <summary>
/// this class will handle the selection of nodes, copy and pasting of nodes
/// </summary>
public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance;

    private List<Nodes> selectedNodes = new List<Nodes>();
    public List<Nodes> SelectedNodes { get => selectedNodes; private set => selectedNodes = value; }

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
        // put all the nodes back to the right color
        foreach (Nodes node in selectedNodes)
        {
            if (node.NodeErrorCode == Nodes.ErrorCode.notConnected || node.NodeErrorCode == Nodes.ErrorCode.wrongInput)
                node.ChangeBorderColor(node.errorColor);
            else
                node.ChangeBorderColor(node.defaultColor);
        }
        SelectedNodes.Clear();
    }
}
// end tpi
