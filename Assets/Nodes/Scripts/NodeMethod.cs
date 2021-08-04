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
using System;

public class NodeMethod : Nodes
{
    private string nodeExecutableString;

    new private void Awake()
    {
        base.Awake();
        nodeTypes = NodeTypes.subProgram;
        OnDoubleClick += ModifyNodeContent;
    }

    public void OnDestroy()
    {
        DestroyNode();
        OnDoubleClick -= ModifyNodeContent;
    }

    protected override void ModifyNodeContent(object sender, EventArgs e)
    {
        MakeSubProgramList();
        PopUpFillNode popUpFillNode = PopUpManager.ShowPopUp(PopUpManager.PopUpTypes.FillSubProgram).GetComponent<PopUpFillNode>();
        if (nodeExecutableString != null)
            popUpFillNode.inputModule.SetInputsContent(new string[] { new PopUpFillNodeDropDownModule.DropDownItemDefiner("Select a sub program", subProgramList.Values.ToArray(), Convert.ToInt32(nodeExecutableString + 1)).ToJson() });
        else
            popUpFillNode.inputModule.SetInputsContent(new string[] { new PopUpFillNodeDropDownModule.DropDownItemDefiner("Select a sub program", subProgramList.Values.ToArray(), 0).ToJson() });

        popUpFillNode.cancelAction = () =>
        {
            popUpFillNode.Close();
        };
        popUpFillNode.OkAction = () =>
        {
            TMP_Dropdown dropdown = popUpFillNode.inputModule.Inputs[0] as TMP_Dropdown;
            KeyValuePair<int, string> subProgram = subProgramList.ElementAt(dropdown.value - 1);
            nodeExecutableString = subProgram.Key.ToString();
            nodeContentDisplay.text = subProgram.Value;
            popUpFillNode.Close();
        };
    }

    private Dictionary<int, string> subProgramList = new Dictionary<int, string>();
    private void MakeSubProgramList()
    {
        subProgramList.Clear();
        for (int i = 1; i < rs.robot.robotScripts.Count; i++)
        {
            RobotScript robotScript = rs.robot.robotScripts[i];
            if (rs.id != robotScript.id)
            {
                subProgramList.Add(robotScript.id, robotScript.name);
            }
        }
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

        int nextScriptId = Convert.ToInt32(nodeExecutableString);
        if (RobotScript.robotScripts[nextScriptId].nodeStart != null)
        {
            RobotScript.robotScripts[nextScriptId].endCallBack = () => { CallNextNode(); };
            RobotScript.robotScripts[nextScriptId].nodeStart.Execute();
        }
        else
        {
            Debugger.Log("Il n'y a pas de bloc de départ dans le script spécifié");
        }
    }

    public override void CallNextNode()
    {

        if (!ExecManager.Instance.debugOn)
        {
            ChangeBorderColor(defaultColor);
            if (NodesDict.ContainsKey(nextNodeId))
                NodesDict[nextNodeId].Execute();
        }
        else
        {
            ExecManager.Instance.buttonNextAction = () => {
                if (NodesDict.ContainsKey(nextNodeId))
                    NodesDict[nextNodeId].Execute();
                ChangeBorderColor(defaultColor);
            };
            ExecManager.Instance.ShowVar();
        }
    }

    public override void PostExecutionCleanUp(object sender, EventArgs e)
    {
        ChangeBorderColor(defaultColor);
    }

    private int dropDownValue;

    #region save stuff
    public override SerializableNode SerializeNode()
    {
        SerializableNode serializableNode = new SerializableNode()
        {
            id = id,
            nextNodeId = nextNodeId,
            parentId = parentId,
            type = "subProgram",
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

        MakeSubProgramList();
        nodeExecutableString = serializableNode.nodeSettings[0];
        nodeContentDisplay.text = subProgramList[Convert.ToInt32(nodeExecutableString)];

        Resize(new Vector2(serializableNode.size[0], serializableNode.size[1]));
        NodesDict.Add(id, this);
    }
    #endregion
}
