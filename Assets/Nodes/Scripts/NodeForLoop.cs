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
using System.Data;

public class NodeForLoop : Nodes
{
    public int nextNodeInside = -1;

    //private VarsManager.Var varIncrement;
    //private int varStart;
    //private int varEnd;
    //private int varStep;

    //private string incrementVar = "";
    //private string startVar = "";
    //private string endVar = "";
    //private string stepVar = "";

    private bool isForLoopRunning = false;
    private string varName = "";
    private string varStartValue = "";
    private string untilExpression = "";
    private string incrementExpression = "";

    private int startValue; // the value at the very begin of the for loop
    private int incrementValue;
    private int endValue;

    private VarsManager.Var iterationVar;

    private IEnumerator waitBeforeLockInput;

    new private void Awake()
    {
        base.Awake();
        nodeTypes = NodeTypes.forLoop;

        if (handleEndArray.Length > 1)
            handleEndArray[1].loopArea = nodesLoopArea;
        if (handleStartArray.Length > 1)
            handleStartArray[1].loopArea = nodesLoopArea;

        OnDoubleClick += ModifyNodeContent;
    }

    private void OnDestroy()
    {
        OnDoubleClick -= ModifyNodeContent;
        DestroyNode();
    }

    protected override void ModifyNodeContent(object sender, EventArgs e)
    {
        PopUpFillNode popUpFillNode = PopUpManager.ShowPopUp(PopUpManager.PopUpTypes.FillForLoop).GetComponent<PopUpFillNode>();
        popUpFillNode.SetContent(new string[] { varName, varStartValue, untilExpression, incrementExpression });
        popUpFillNode.cancelAction = () =>
        {
            popUpFillNode.Close();
        };
        popUpFillNode.OkAction = () =>
        {
            PopUpNodeInput varInput = popUpFillNode.inputModule.Inputs[0] as PopUpNodeInput;
            PopUpNodeInput varValueInput = popUpFillNode.inputModule.Inputs[1] as PopUpNodeInput;
            PopUpNodeInput untilInput = popUpFillNode.inputModule.Inputs[2] as PopUpNodeInput;
            PopUpNodeInput incrementInput = popUpFillNode.inputModule.Inputs[3] as PopUpNodeInput;

            SetForLoop(varInput.executableFunction.Trim(), varValueInput.executableFunction.Trim(), untilInput.executableFunction.Trim(), incrementInput.executableFunction.Trim());

            popUpFillNode.Close();
        };
    }

    private void SetForLoop(string varName, string varStartValue, string untilExpression, string incrementExpression)
    {
        if (varName == "" || varStartValue == "" || untilExpression == "" || incrementExpression == "")
            return;
        this.varName = varName;
        this.varStartValue = varStartValue;
        this.untilExpression = untilExpression;
        this.incrementExpression = incrementExpression;

        try { startValue = Convert.ToInt32(new DataTable().Compute(rs.robot.varsManager.ReplaceFunctionByValue(varStartValue), null)); } catch (Exception) { startValue = 0; }
        try { incrementValue = Convert.ToInt32(new DataTable().Compute(rs.robot.varsManager.ReplaceFunctionByValue(incrementExpression), null)); } catch (Exception) { incrementValue = 0; }
        try { endValue = Convert.ToInt32(new DataTable().Compute(rs.robot.varsManager.ReplaceFunctionByValue(untilExpression), null)); } catch (Exception) { endValue = 0; }

        nodeContentDisplay.text = LanguageManager.instance.AbrevToFullName("For " + varName + " from " + varStartValue + " to " + untilExpression + " by increments of " + incrementExpression);
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

        if(!isForLoopRunning)
        {
            iterationVar = rs.robot.varsManager.GetVar(varName, startValue - incrementValue);
            isForLoopRunning = true;
        }

        if(startValue <= endValue)
        {
            if(iterationVar.Value >= endValue)
            {
                StartCoroutine("WaitBeforeCallingNextNode");
                isForLoopRunning = false;
            }
            else
            {
                IEnumerator coroutine = WaitBeforeCallingNextNode(nextNodeInside);
                StartCoroutine(coroutine);
            }
        }
        else
        {
            if (iterationVar.Value <= endValue)
            {
                StartCoroutine("WaitBeforeCallingNextNode");
                isForLoopRunning = false;
            }
            else
            {
                IEnumerator coroutine = WaitBeforeCallingNextNode(nextNodeInside);
                StartCoroutine(coroutine);
            }
        }

        iterationVar.Value += incrementValue;
        iterationVar.Persist();
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
        //varStep = 1;
        //varEnd = 0;
        //varIncrement = null;
        //varStart = 0;
    }

    #region save stuff
 
    public override SerializableNode SerializeNode()
    {
        SerializableNode serializableNode = new SerializableNode()
        {
            id = id,
            nextNodeId = nextNodeId,
            parentId = parentId,
            type = "forLoop",
            position = new float[] { transform.position.x, transform.position.y, transform.position.z },
            nodeSettings = new List<string>(),
            size = new float[] { canvasRect.sizeDelta.x, canvasRect.sizeDelta.y },

        };
        serializableNode.nodeSettings.Add(varName);
        serializableNode.nodeSettings.Add(varStartValue);
        serializableNode.nodeSettings.Add(untilExpression);
        serializableNode.nodeSettings.Add(incrementExpression);
        serializableNode.nodeSettings.Add(nextNodeInside.ToString());
        return serializableNode;
    }
    public override void DeSerializeNode(SerializableNode serializableNode)
    {
        id = serializableNode.id;
        nextNodeId = serializableNode.nextNodeId; //this is the next node in the execution order
        parentId = serializableNode.parentId;

        SetForLoop(serializableNode.nodeSettings[0], serializableNode.nodeSettings[1], serializableNode.nodeSettings[2], serializableNode.nodeSettings[3]);
        nextNodeInside = Convert.ToInt32(serializableNode.nodeSettings[4]);
        Resize(new Vector2(serializableNode.size[0], serializableNode.size[1]));
        if(!NodesDict.ContainsKey(id))
        {
            NodesDict.Add(id, this);
        }
        else
        {
            if(NodesDict[id] != this)
            {
                Debug.LogError("Tried to replace a node by another one");
            }
        }
    }
    #endregion
}
