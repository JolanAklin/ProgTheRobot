using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NodeStart : Nodes
{
    public override void Execute()
    {
        // remove all marks on the terrain
        rs.robot.robotManager.RemoveMarks();
        rs.robot.robotManager.ResetBalls();
        rs.robot.robotManager.GetBallOnTerrain();

        if (!ExecManager.Instance.isRunning)
            return;
        ChangeBorderColor(currentExecutedNode);

        rs.robot.power = rs.robot.defaultPower;
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
    public override SerializableNode SerializeNode()
    {
        SerializableNode serializableNode = new SerializableNode()
        {
            id = id,
            nextNodeId = nextNodeId,
            parentId = parentId,
            type = "start",
            position = new float[] { transform.position.x, transform.position.y, transform.position.z },
            nodeSettings = new List<string>(),
            size = new float[] { canvasRect.sizeDelta.x, canvasRect.sizeDelta.y },

        };
        return serializableNode;
    }
    public override void DeSerializeNode(SerializableNode serializableNode)
    {
        id = serializableNode.id;
        nextNodeId = serializableNode.nextNodeId; //this is the next node in the execution order
        parentId = serializableNode.parentId;
        Resize(new Vector2(serializableNode.size[0], serializableNode.size[1]));
    }
    #endregion
}
