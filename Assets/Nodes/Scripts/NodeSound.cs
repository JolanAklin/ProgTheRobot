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

    public override void LockUnlockAllInput(object sender, ExecManager.onChangeBeginEventArgs e)
    {
        LockUnlockAllInput(true);
    }

    public override void LockUnlockAllInput(bool isLocked)
    {
        inputField.enabled = !isLocked;
        IsInputLocked = isLocked;
        if (!isLocked)
            inputField.Select();
    }

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

    private bool ValidateInput()
    {
        return SoundManager.instance.HasAudio(input);
    }

    public void SyncCheckChanged()
    {
        playAsync = asyncToggle.value;
    }

    public override void Execute()
    {
        if (!ExecManager.Instance.isRunning)
            return;
        ChangeBorderColor(currentExecutedNode);

        if (!playAsync)
        {
            SoundManager.instance.Play(input);
            StartCoroutine("WaitBeforeCallingNextNode");
        }
        else
        {
            SoundManager.instance.PlaySync(input, () => { StartCoroutine("WaitBeforeCallingNextNode"); });
        }
    }

    // get called by the robotmanager when an action require more power than the robot has
    private void noPower()
    {
        ExecManager.Instance.StopExec();
        rs.End();
        ChangeBorderColor(defaultColor);
        Debugger.Log($"Le robot {rs.robot.robotName} n'a plus assez d'énergie");
        Debug.Log("there");
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
    public override void DeSerializeNode(SerializableNode serializableNode)
    {
        id = serializableNode.id;
        nextNodeId = serializableNode.nextNodeId; //this is the next node in the execution order
        parentId = serializableNode.parentId;
        input = serializableNode.nodeSettings[0];
        asyncToggle.value = Convert.ToBoolean(serializableNode.nodeSettings[1]);
        inputField.text = input;
        Resize(new Vector2(serializableNode.size[0], serializableNode.size[1]));
    }
    #endregion
}

// end tpi
