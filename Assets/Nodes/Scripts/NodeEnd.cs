using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeEnd : Nodes
{
    public override void SerializeNode()
    {
        throw new System.NotImplementedException();
    }
    public override void DeSerializeNode()
    {
        throw new System.NotImplementedException();
    }
    public override void Execute()
    {
        CallNextNode();
    }

    public override void CallNextNode()
    {
        Debug.Log("its the end");
    }

    public override void PostExecutionCleanUp()
    {
        throw new System.NotImplementedException();
    }
}
