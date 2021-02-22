using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NodeEnd : Nodes
{
    public override void Execute()
    {
        if (!ExecManager.Instance.isRunning)
            return;
        ChangeBorderColor(currentExecutedNode);
        StartCoroutine("WaitBeforeCallingNextNode");
    }

    IEnumerator WaitBeforeCallingNextNode()
    {
        yield return new WaitForSeconds(executedColorTime / Manager.instance.execSpeed);
        ChangeBorderColor(defaultColor);
        CallNextNode();
    }

    public override void CallNextNode()
    {
        ExecManager.Instance.StopExec();
        rs.End();
        Debug.Log("its the end");
    }

    public override void PostExecutionCleanUp(object sender, EventArgs e)
    {
        ChangeBorderColor(defaultColor);
    }

    #region save stuff
    public class SerializedNodeEnd : SerializableNode
    {

    }
    public override SerializableNode SerializeNode()
    {
        SerializedNodeEnd serializedNodeEnd = new SerializedNodeEnd() { id = id, nextNodeId = nextNodeId };
        return serializedNodeEnd;
    }
    public override void DeSerializeNode()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
