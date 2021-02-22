using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NodeStart : Nodes
{

    public static event EventHandler OnStart;

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
        ChangeBorderColor(currentExecutedNode);

        rs.robot.robotManager.transform.position = rs.robot.robotManager.robotStartPos;
        rs.robot.robotManager.transform.rotation = rs.robot.robotManager.robotStartRot;
        rs.robot.varsManager.Clean();
        OnStart?.Invoke(this, EventArgs.Empty);
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
        if (NodesDict.ContainsKey(nextNodeId))
        {
            NodesDict[nextNodeId].Execute();
        }
    }

    public override void PostExecutionCleanUp(object sender, EventArgs e)
    {
        Debug.Log("node start cleanup do nothing");
    }
}
