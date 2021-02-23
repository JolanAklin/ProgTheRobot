using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NodeStart : Nodes
{
    public override void Execute()
    {
        if (!ExecManager.Instance.isRunning)
            return;
        ChangeBorderColor(currentExecutedNode);

        rs.robot.robotManager.transform.position = rs.robot.robotManager.robotStartPos;
        rs.robot.robotManager.transform.rotation = rs.robot.robotManager.robotStartRot;
        rs.robot.varsManager.Clean();
        StartCoroutine("WaitBeforeCallingNextNode");
    }

    IEnumerator WaitBeforeCallingNextNode()
    {
        if (!ExecManager.Instance.debugOn)
        {
            yield return new WaitForSeconds(executedColorTime / Manager.instance.execSpeed);
            ChangeBorderColor(defaultColor);
            CallNextNode();
        }
        else
        {
            ExecManager.Instance.buttonNextAction = () => {
                CallNextNode();
                ChangeBorderColor(defaultColor);
            };

        }
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
        ChangeBorderColor(defaultColor);
    }

    #region save stuff
    public class SerializedStart : SerializableNode
    {

    }

    public override SerializableNode SerializeNode()
    {
        SerializedStart serializedStart = new SerializedStart() {id = id, nextNodeId = nextNodeId };
        return serializedStart;
    }
    public override void DeSerializeNode()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
