using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeContextMenuScript : MonoBehaviour
{
    [HideInInspector]
    public Nodes nodeToModify;

    /// <summary>
    /// Unlock all node's input
    /// </summary>
    public void Modify()
    {
        nodeToModify.LockUnlockAllInput(false);
        UIRaycaster.instance.nodeContextMenuOpen = false;
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Remove the ndoe targeted by the context menu
    /// </summary>
    public void Remove()
    {
        Destroy(nodeToModify.gameObject);
        Destroy(this.gameObject);
    }
}
