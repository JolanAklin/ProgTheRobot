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

public class NodeReadWrite : Nodes
{
    /// <summary>
    /// This string is attended to be used only internally. This is not shown to the user.
    /// </summary>
    private string nodeExecutableString;
    public TMP_Text nodeContentDisplay;

    new private void Awake()
    {
        base.Awake();
        nodeTypes = NodeTypes.readWrite;
        ExecManager.onChangeBegin += LockUnlockAllInput;
        OnDoubleClick += ModifyNodeContent;
    }

    private void OnDestroy()
    {
        ExecManager.onChangeBegin -= LockUnlockAllInput;
        DestroyNode();
        OnDoubleClick -= ModifyNodeContent;
    }

    public void ModifyNodeContent(object sender, EventArgs e)
    {
        PopUpFillNode popUpFillNode = PopUpManager.ShowPopUp(PopUpManager.PopUpTypes.FillReadWrite).GetComponent<PopUpFillNode>();
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

    public override void LockUnlockAllInput(object sender, ExecManager.onChangeBeginEventArgs e)
    {
    }

    public override void LockUnlockAllInput(bool isLocked)
    {
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

        string[] delimiters = new string[] { " " };
        string[] inputSplited = nodeExecutableString.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        PopUpReadWrite rw = PopUpManager.ShowPopUp(PopUpManager.PopUpTypes.readWrite).GetComponent<PopUpReadWrite>();
        switch (inputSplited[0])
        {
            case "kwwrite#":
                rw.Init($"Affichage de {inputSplited[1]}", rs.robot.varsManager.GetVar(inputSplited[1]).Value.ToString());
                rw.SetOkAction(() => { 
                    rw.Close();
                    StartCoroutine("WaitBeforeCallingNextNode");
                });
                break;
            case "kwread#":
                rw.Init($"Lecture de {inputSplited[1]}");
                rw.SetOkAction(() => {
                    VarsManager.Var var = rs.robot.varsManager.GetVar(inputSplited[1],0);
                    var.Value = rw.value();
                    var.Persist();
                    rw.Close();
                    StartCoroutine("WaitBeforeCallingNextNode");
                });
                break;
        }
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

    public override void CallNextNode()
    {
        ChangeBorderColor(defaultColor);
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
            type = "readWrite",
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
        NodesDict.Add(id, this);
    }
    #endregion
}
