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
using TMPro;
using System;
using Language;
using System.Text.RegularExpressions;

public class NodeWhileLoop : Nodes
{
    private string nodeExecutableString = "";

    public int nextNodeInside = -1;
    new private void Awake()
    {
        base.Awake();
        nodeTypes = NodeTypes.whileLoop;

        // click management
        OnDoubleClick += ModifyNodeContent;

        // set in wich looparea is the inside connectHandle
        if (handleEndArray.Length > 1)
            handleEndArray[1].loopArea = nodesLoopArea;
        if (handleStartArray.Length > 1)
            handleStartArray[1].loopArea = nodesLoopArea;
    }

    protected override void ModifyNodeContent(object sender, EventArgs e)
    {
        PopUpFillNode popUpFillNode = PopUpManager.ShowPopUp(PopUpManager.PopUpTypes.FillWhile).GetComponent<PopUpFillNode>();
        if (nodeExecutableString != null)
            popUpFillNode.SetContent(new string[] { nodeExecutableString });
        popUpFillNode.cancelAction = () =>
        {
            popUpFillNode.Close();
        };
        popUpFillNode.OkAction = () =>
        {
            PopUpNodeInput input = popUpFillNode.inputModule.Inputs[0] as PopUpNodeInput;
            nodeExecutableString = input.executableFunction;
            nodeContentDisplay.text = LanguageManager.instance.AbrevToFullName(nodeExecutableString);
            popUpFillNode.Close();
        };
    }


    private void OnDestroy()
    {
        // click management
        OnDoubleClick -= ModifyNodeContent;

        DestroyNode();
    }

    public override void Execute()
    {
        // test if the robot has enough power to execute the node, if not he stop the code execution
        if (rs.robot.Power <= nodeExecPower)
        {
            ExecManager.Instance.StopExec();
            rs.End();
            ChangeBorderColor(defaultColor);
            Debugger.Log($"Le robot {rs.robot.robotName} n'a plus assez d'énergie");
            return;
        }
        rs.robot.Power -= nodeExecPower;

        if (!ExecManager.Instance.isRunning)
            return;
        ChangeBorderColor(currentExecutedNode);

        IEnumerator coroutine = WaitBeforeCallingNextNode(nextNodeInside);

        VarsManager.Evaluation eval = rs.robot.varsManager.Evaluate(nodeExecutableString);

        if (!eval.error)
            if (eval.result)
            {
                StartCoroutine(coroutine);
            }
            else
            {
                StartCoroutine("WaitBeforeCallingNextNode");
            }
        else
            Debugger.LogError("Une erreur est seurvenue durant l'évaluation de l'expression");
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
            ExecManager.Instance.ShowVar();
        }
    }

    IEnumerator WaitBeforeCallingNextNode(int nodeId)
    {
        if (!ExecManager.Instance.debugOn)
        {
            yield return new WaitForSeconds(executedColorTime / Manager.instance.execSpeed);
            ChangeBorderColor(defaultColor);
            NodesDict[nodeId].Execute();
        }
        else
        {
            ExecManager.Instance.buttonNextAction = () => {
                NodesDict[nodeId].Execute();
                ChangeBorderColor(defaultColor);
            };
        }
    }

    public override void CallNextNode()
    {
        if (NodesDict.ContainsKey(nextNodeId))
            NodesDict[nextNodeId].Execute();
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
            type = "whileLoop",
            position = new float[] { transform.position.x, transform.position.y, transform.position.z },
            nodeSettings = new List<string>(),
            size = new float[] { canvasRect.sizeDelta.x, canvasRect.sizeDelta.y },

        };
        serializableNode.nodeSettings.Add(nodeExecutableString);
        serializableNode.nodeSettings.Add(nextNodeInside.ToString());
        return serializableNode;
    }
    public override void DeSerializeNode(SerializableNode serializableNode)
    {
        id = serializableNode.id;
        nextNodeId = serializableNode.nextNodeId; //this is the next node in the execution order
        parentId = serializableNode.parentId;
        nodeExecutableString = serializableNode.nodeSettings[0];
        nodeContentDisplay.text = LanguageManager.instance.AbrevToFullName(nodeExecutableString);
        nextNodeInside = Convert.ToInt32(serializableNode.nodeSettings[1]);
        Resize(new Vector2(serializableNode.size[0], serializableNode.size[1]));
        NodesDict.Add(id, this);
    }
    #endregion
}
