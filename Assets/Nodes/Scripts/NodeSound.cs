// Copyright 2021 Jolan Aklin

//This file is part of Prog the robot.

//Prog the robot is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 3 of the License.

//Prog the robot is distributed in the hope that it will be useful,
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

// start tpi

public class NodeSound : Nodes
{
    public TMP_InputField inputField;
    public ToggleScript asyncToggle;
    public AutoCompletion completion;
    private string input;
    private bool playAsync = false;

    new private void Awake()
    {
        base.Awake();
        nodeTypes = NodeTypes.sound;
        ExecManager.onChangeBegin += LockUnlockAllInput;
        completion.possibleWord = SoundManager.instance.GetAllAudioNames();
        completion.ChangeProbaWord(this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        ExecManager.onChangeBegin -= LockUnlockAllInput;
        DestroyNode();
    }

    /// <summary>
    /// Lock all input fields of the node
    /// </summary>
    /// <param name="isLocked">If true, all input fields cannot be modified</param>
    public override void LockUnlockAllInput(object sender, ExecManager.onChangeBeginEventArgs e)
    {
        LockUnlockAllInput(true);
    }
    /// <summary>
    /// Lock all input fields of the node
    /// </summary>
    /// <param name="isLocked">If true, all input fields cannot be modified</param>
    public override void LockUnlockAllInput(bool isLocked)
    {
        inputField.enabled = !isLocked;
        IsInputLocked = isLocked;
        if (!isLocked)
            inputField.Select();
    }

    /// <summary>
    /// Called when the node's input field is changed. It will check for error.
    /// </summary>
    /// <param name="tMP_InputField"></param>
    public void ChangeInput(TMP_InputField tMP_InputField)
    {
        inputField = tMP_InputField;
        input = tMP_InputField.text;
        if (!ValidateInput())
        {
            nodeErrorCode = ErrorCode.wrongInput;
            ChangeBorderColor(errorColor);
            Manager.instance.canExecute = false;
            return;
        }
        nodeErrorCode = ErrorCode.ok;
        Manager.instance.canExecute = true;
        ChangeBorderColor(defaultColor);
    }

    /// <summary>
    /// Test if the node has a correct value
    /// </summary>
    /// <returns>True if the node has a correct value</returns>
    private bool ValidateInput()
    {
        return SoundManager.instance.HasAudio(input);
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
            SoundManager.instance.Play(input, rs.robot.robotManager.audioSource);
            StartCoroutine("WaitBeforeCallingNextNode");
        }
        else
        {
            SoundManager.instance.PlaySync(input, rs.robot.robotManager.audioSource, () => { StartCoroutine("WaitBeforeCallingNextNode"); });
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
        serializableNode.nodeSettings.Add(input);
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
        input = serializableNode.nodeSettings[0];
        asyncToggle.value = Convert.ToBoolean(serializableNode.nodeSettings[1]);
        inputField.text = input;
        Resize(new Vector2(serializableNode.size[0], serializableNode.size[1]));
        NodesDict.Add(id, this);
    }
    #endregion
}

// end tpi
