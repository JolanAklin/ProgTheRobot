using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectHandle : MonoBehaviour
{
    public Nodes node;
    public bool isInput = false;

    public void Click()
    {
        Nodes nextNode = Manager.instance.ConnectNode(isInput, transform, node);
        if(nextNode != null)
        {
            node.nextNodeId = nextNode.id;
            node.nextGameObject = nextNode.gameObject;
            nextNode = null;
            Manager.instance.node = null;
            Debug.Log(node.nextNodeId);
        }
    }
}
