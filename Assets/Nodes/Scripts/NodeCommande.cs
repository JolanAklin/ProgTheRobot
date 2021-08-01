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

public class NodeCommande : Nodes
{
    /// <summary>
    /// This string is attended to be used only internally. This is not shown to the user.
    /// </summary>
    private string nodeExecutableString;
    public TMP_Text nodeContentDisplay;

    new private void Awake()
    {
        base.Awake();
        nodeTypes = NodeTypes.execute;
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
        PopUpFillNode popUpFillNode = PopUpManager.ShowPopUp(PopUpManager.PopUpTypes.FillAction).GetComponent<PopUpFillNode>();
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
        if (!ExecManager.Instance.isRunning)
            return;
        ChangeBorderColor(currentExecutedNode);

        switch (nodeExecutableString)
        {
            case "":
                break;
            case string test when test.Contains("acgf#"):
                rs.robot.robotManager.GoForward(() => { StartCoroutine("WaitBeforeCallingNextNode"); }, noPower);
                break;
            case string test when test.Contains("actr#"):
                rs.robot.robotManager.TurnRight(() => { StartCoroutine("WaitBeforeCallingNextNode"); }, noPower);
                break;
            case string test when test.Contains("actl#"):
                rs.robot.robotManager.TurnLeft(() => { StartCoroutine("WaitBeforeCallingNextNode"); }, noPower);
                break;
            case string test when test.Contains("acm#"):
                rs.robot.robotManager.Mark(() => { StartCoroutine("WaitBeforeCallingNextNode"); }, noPower);
                break;
            case string test when test.Contains("acum#"):
                rs.robot.robotManager.Unmark(() => { StartCoroutine("WaitBeforeCallingNextNode"); }, noPower);
                break;
            case string test when test.Contains("acr#"):
                rs.robot.robotManager.Charge(() => { StartCoroutine("WaitBeforeCallingNextNode"); });
                break;
            case string test when test.Contains("acpb#"):
                rs.robot.robotManager.PlaceBall(() => { StartCoroutine("WaitBeforeCallingNextNode"); }, noPower);
                break;
            case string test when test.Contains("actb#"):
                rs.robot.robotManager.TakeBall(() => { StartCoroutine("WaitBeforeCallingNextNode"); }, noPower);
                break;
            case string test when test.Contains("actwb#"):
                rs.robot.robotManager.ThrowBall(() => { StartCoroutine("WaitBeforeCallingNextNode"); }, noPower);
                break;

            default:
                Debugger.LogError("Commande inconnue");
                break;
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
            type = "execute",
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
