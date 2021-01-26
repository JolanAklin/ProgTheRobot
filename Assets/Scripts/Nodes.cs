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

    public abstract void SerializeNode();

    public abstract void Execute();

    public int id;
    public static int nextid = 0;

    private void Awake()
    {
        id = nextid;
        nextid++;
    }

    public void Start()
    {
        
    }
}
