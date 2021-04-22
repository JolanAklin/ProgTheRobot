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
}
