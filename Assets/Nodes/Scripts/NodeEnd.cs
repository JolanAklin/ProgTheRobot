// Copyright 2021 Jolan Aklin

//This file is part of Prog The Robot.

//Prog The Robot is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 3 of the License.

//Prog The Robot is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Prog the robot.  If not, see<https://www.gnu.org/licenses/>.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NodeEnd : Nodes
{
    new private void Awake()
    {
        base.Awake();
        nodeTypes = NodeTypes.end;
    }

    private void OnDestroy()
    {
        DestroyNode();
    }

    public override void Execute()
    {
        if (!ExecManager.Instance.isRunning)
            return;
        if (rs.endCallBack == null)
        {
            ChangeBorderColor(currentExecutedNode);
            StartCoroutine("WaitBeforeCallingNextNode");
        }
        else
        {
            rs.endCallBack?.Invoke();
            ChangeBorderColor(defaultColor);
        }
    }
    protected override void ModifyNodeContent(object sender, EventArgs e)
    {}

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
            parentId = parentId,
            type = "end",
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
        NodesDict.Remove(id);
        if (!NodesDict.ContainsKey(id))
        {
            NodesDict.Add(id, this);
        }
        else
        {
            if (NodesDict[id] != this)
            {
                Debug.LogError("Tried to replace a node by another one");
            }
        }
    }

    #endregion
}
