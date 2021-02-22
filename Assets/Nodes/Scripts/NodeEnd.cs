using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
        //ExecManager.Instance.isRunning = false;
        ExecManager.Instance.isRunning = false;
        rs.End();
        Debug.Log("its the end");
    }

    public override void PostExecutionCleanUp(object sender, EventArgs e)
    {
    }
}
