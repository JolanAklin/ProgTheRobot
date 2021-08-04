using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScriptAreaBackground : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // simple left click performed
            SelectionManager.instance.ResetSelection();
            MenuToolTip.instance.HideToolTip();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // simple right click performed
            MenuToolTip.instance.SetContent("Add node", new MenuToolTip.ButtonContent[] {
                new MenuToolTip.ButtonContent("Start node", () => { AddNodeScript.instance.AddNode(Nodes.NodeTypes.start);MenuToolTip.instance.HideToolTip(); }),
                new MenuToolTip.ButtonContent("End node", () => { AddNodeScript.instance.AddNode(Nodes.NodeTypes.end);MenuToolTip.instance.HideToolTip(); }),
                new MenuToolTip.ButtonContent("Action node", () => { AddNodeScript.instance.AddNode(Nodes.NodeTypes.execute);MenuToolTip.instance.HideToolTip(); }),
                new MenuToolTip.ButtonContent("Read/write node", () => { AddNodeScript.instance.AddNode(Nodes.NodeTypes.readWrite);MenuToolTip.instance.HideToolTip(); }),
                new MenuToolTip.ButtonContent("Test node", () => { AddNodeScript.instance.AddNode(Nodes.NodeTypes.test);MenuToolTip.instance.HideToolTip(); }),
                new MenuToolTip.ButtonContent("Sub program node", () => { AddNodeScript.instance.AddNode(Nodes.NodeTypes.subProgram);MenuToolTip.instance.HideToolTip(); }),
                new MenuToolTip.ButtonContent("Affectation node", () => { AddNodeScript.instance.AddNode(Nodes.NodeTypes.affectation);MenuToolTip.instance.HideToolTip(); }),
                new MenuToolTip.ButtonContent("While loop node", () => { AddNodeScript.instance.AddNode(Nodes.NodeTypes.whileLoop);MenuToolTip.instance.HideToolTip(); }),
                new MenuToolTip.ButtonContent("For loop node", () => { AddNodeScript.instance.AddNode(Nodes.NodeTypes.forLoop);MenuToolTip.instance.HideToolTip(); }),
                new MenuToolTip.ButtonContent("Sound node", () => { AddNodeScript.instance.AddNode(Nodes.NodeTypes.sound);MenuToolTip.instance.HideToolTip(); }),
            });
            MenuToolTip.instance.ShowToolTip();
        }
    }
}
