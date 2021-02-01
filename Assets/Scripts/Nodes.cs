using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Nodes : MonoBehaviour
{
    public enum NodeTypes
    {
        start = 0,
        end,
        execute,
        loop,
        test,
        methodCaller,
    }

    /// <summary>
    /// Will convert the node to json
    /// </summary>
    public abstract void SerializeNode();

    /// <summary>
    /// Execute the node
    /// </summary>
    public abstract void Execute();

    public int id;
    public static int nextid = 0;

    private void Awake()
    {
        // All nodes have a different id
        id = nextid;
        nextid++;
    }

    public void Start()
    {
        
    }

    public void Move()
    {

    }

    public void Resize()
    {

    }
}
