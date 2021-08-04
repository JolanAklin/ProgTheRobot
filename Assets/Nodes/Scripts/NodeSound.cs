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
using Language;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class NodeSound : Nodes
{
    public ToggleScript asyncToggle;
    private string nodeExecutableString;
    private bool playAsync = false;

    new private void Awake()
    {
        base.Awake();
        nodeTypes = NodeTypes.sound;
        OnDoubleClick += ModifyNodeContent;
    }

    private void OnDestroy()
    {
        OnDoubleClick -= ModifyNodeContent;
        DestroyNode();
    }

    protected override void ModifyNodeContent(object sender, EventArgs e)
    {
        PopUpFillNode popUpFillNode = PopUpManager.ShowPopUp(PopUpManager.PopUpTypes.FillSound).GetComponent<PopUpFillNode>();
        if (nodeExecutableString != null)
            popUpFillNode.inputModule.SetInputsContent(new string[] { new PopUpFillNodeDropDownModule.DropDownItemDefiner("Choose a sound", SoundManager.instance.AudioClipsName.ToArray(), SoundManager.instance.AudioClipsName.IndexOf(nodeExecutableString) + 1).ToJson() });
        else
            popUpFillNode.inputModule.SetInputsContent(new string[] { new PopUpFillNodeDropDownModule.DropDownItemDefiner("Choose a sound", SoundManager.instance.AudioClipsName.ToArray(), 0).ToJson() });

        popUpFillNode.cancelAction = () =>
        {
            popUpFillNode.Close();
        };
        popUpFillNode.OkAction = () =>
        {
            TMP_Dropdown dropDown = popUpFillNode.inputModule.Inputs[0] as TMP_Dropdown;
            nodeExecutableString = dropDown.options[dropDown.value].text;
            nodeContentDisplay.text = nodeExecutableString;
            popUpFillNode.Close();
        };
    }

    /// <summary>
    /// Called when the asyncToggle is changed
    /// </summary>
    public void SyncCheckChanged()
    {
        playAsync = asyncToggle.value;
    }

    /// <summary>
    /// Execute the node
    /// </summary>
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

        // play the sound
        if (!playAsync)
        {
            SoundManager.instance.Play(nodeExecutableString, rs.robot.robotManager.audioSource);
            StartCoroutine("WaitBeforeCallingNextNode");
        }
        else
        {
            SoundManager.instance.PlaySync(nodeExecutableString, rs.robot.robotManager.audioSource, () => { StartCoroutine("WaitBeforeCallingNextNode"); });
        }
    }

    /// <summary>
    /// Wait before calling the next node
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Call the next node immediately
    /// </summary>
    public override void CallNextNode()
    {
        if (NodesDict.ContainsKey(nextNodeId))
            NodesDict[nextNodeId].Execute();
    }

    /// <summary>
    /// Clean the node
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public override void PostExecutionCleanUp(object sender, EventArgs e)
    {
        ChangeBorderColor(defaultColor);
    }

    #region save stuff
    /// <summary>
    /// Convert the node to a saveable state
    /// </summary>
    /// <returns></returns>
    public override SerializableNode SerializeNode()
    {
        SerializableNode serializableNode = new SerializableNode() {
            id = id,
            nextNodeId = nextNodeId,
            parentId = parentId,
            type = "sound",
            position = new float[] { transform.position.x, transform.position.y, transform.position.z },
            nodeSettings = new List<string>(),
            size = new float[] { canvasRect.sizeDelta.x, canvasRect.sizeDelta.y },
        };
        serializableNode.nodeSettings.Add(nodeExecutableString);
        serializableNode.nodeSettings.Add(playAsync.ToString());
        return serializableNode;
    }
    /// <summary>
    /// Set the values from a saved node
    /// </summary>
    /// <param name="serializableNode"></param>
    public override void DeSerializeNode(SerializableNode serializableNode)
    {
        id = serializableNode.id;
        nextNodeId = serializableNode.nextNodeId; //this is the next node in the execution order
        parentId = serializableNode.parentId;
        nodeExecutableString = serializableNode.nodeSettings[0];
        asyncToggle.value = Convert.ToBoolean(serializableNode.nodeSettings[1]);
        nodeContentDisplay.text = nodeExecutableString;
        Resize(new Vector2(serializableNode.size[0], serializableNode.size[1]));
        NodesDict.Add(id, this);
    }
    #endregion
}
