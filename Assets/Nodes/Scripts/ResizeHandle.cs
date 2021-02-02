using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeHandle : MonoBehaviour
{
    public Nodes node;

    public void NodeResize()
    {
        node.StartEndResize();
    }
}
