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
    public override SerializableNode SerializeNode()
    {
        SerializableNode serializableNode = new SerializableNode()
        {
            id = id,
            nextNodeId = nextNodeId,
            type = "end",
            position = new float[] { transform.position.x, transform.position.y, transform.position.z },
            nodeSettings = new List<string>(),
            size = new float[] { canvas.sizeDelta.x, canvas.sizeDelta.y },

        };
        return serializableNode;
    }
    public override void DeSerializeNode(SerializableNode serializableNode)
    {
        id = serializableNode.id;
        nextNodeId = serializableNode.nextNodeId; //this is the next node in the execution order
        Resize(new Vector2(serializableNode.size[0], serializableNode.size[1]));
    }
    #endregion
}
