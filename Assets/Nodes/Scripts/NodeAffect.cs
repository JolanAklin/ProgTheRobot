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
using System.Linq;
using System.Data;
using System;
using System.Text.RegularExpressions;

public class NodeAffect : Nodes
{
    private string nodeExecutableString;

    private VarsManager.Var var;


    new private void Awake()
    {
        base.Awake();
        nodeTypes = NodeTypes.affectation;
        OnDoubleClick += ModifyNodeContent;
    }

    public void OnDestroy()
    {
        OnDoubleClick -= ModifyNodeContent;
        DestroyNode();
    }

    protected override void ModifyNodeContent(object sender, EventArgs e)
    {
        PopUpFillNode popUpFillNode = PopUpManager.ShowPopUp(PopUpManager.PopUpTypes.FillAffectation).GetComponent<PopUpFillNode>();
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


    public override void Execute()
    {
        // test if the robot has enough power to execute the node, if not he stop the code execution
        if(rs.robot.Power <= nodeExecPower)
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
        // calculate and set the var
        string[] delimiters = new string[] { " " };
        string[] inputVarReplaced = rs.robot.varsManager.ReplaceFunctionByValue(nodeExecutableString).Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        if (inputVarReplaced != null)
        {
            if (var == null)
            {
                string expression = string.Join("", inputVarReplaced, 2, inputVarReplaced.Length - 2).Trim();
                var = rs.robot.varsManager.GetVar(inputVarReplaced[0], Convert.ToInt32(new DataTable().Compute(expression, null)));
                if (var == null)
                {
                    Debugger.LogError("Une erreur est survenue");
                    return;
                }
            }

        }
        else
        {
            // stop execution
            //Debugger.LogError("La variable spécifiée n'est pas connue");
            //ChangeBorderColor(errorColor);
            rs.robot.varsManager.GetVar(inputVarReplaced[0], 0);
            Execute();
        }
        StartCoroutine("WaitBeforeCallingNextNode");
    }

    IEnumerator WaitBeforeCallingNextNode()
    {
        if(!ExecManager.Instance.debugOn)
        {
            // wait before calling the next node
            yield return new WaitForSeconds(executedColorTime / Manager.instance.execSpeed);
            ChangeBorderColor(defaultColor);
            CallNextNode();
        }
        else
        {
            // register a lambda to the action of the "next" button when the debuger is on
            ExecManager.Instance.buttonNextAction = () => {
                CallNextNode();
                ChangeBorderColor(defaultColor);
            };
            ExecManager.Instance.ShowVar();
            
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
        var = null;
    }

    #region save stuff
    public override SerializableNode SerializeNode()
    {
        SerializableNode serializableNode = new SerializableNode() {
            id = id,
            nextNodeId = nextNodeId, //this is the next node in the execution order
            parentId = parentId,
            type = "affectation",
            position = new float[] { transform.position.x, transform.position.y, transform.position.z },
            nodeSettings = new List<string>(),
            size = new float[] { canvasRect.sizeDelta.x, canvasRect.sizeDelta.y },
        };
        serializableNode.nodeSettings.Add(nodeExecutableString);
        return serializableNode;
    }
    public override void DeSerializeNode(SerializableNode serializableNode)
    {
        id = serializableNode.id;
        nextNodeId = serializableNode.nextNodeId; //this is the next node in the execution order
        parentId = serializableNode.parentId;
        nodeExecutableString = serializableNode.nodeSettings[0];
        nodeContentDisplay.text = LanguageManager.instance.AbrevToFullName(nodeExecutableString);
        Resize(new Vector2(serializableNode.size[0], serializableNode.size[1]));
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
