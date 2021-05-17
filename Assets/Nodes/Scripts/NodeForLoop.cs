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

public class NodeForLoop : Nodes
{
    [Tooltip("Put the input field in this order : varname, varstart, varend, varstep")]
    public TMP_InputField[] inputFields;
    public int nextNodeInside = -1;

    private VarsManager.Var varIncrement;
    private int varStart;
    private int varEnd;
    private int varStep;

    private string incrementVar = "";
    private string startVar = "";
    private string endVar = "";
    private string stepVar = "";

    private IEnumerator waitBeforeLockInput;

    public void ChangeInput()
    {
        if (!ValidateInput())
        {
            nodeErrorCode = ErrorCode.wrongInput;
            ChangeBorderColor(errorColor);
            Manager.instance.canExecute = false;
            return;
        }
        nodeErrorCode = ErrorCode.ok;
        ChangeBorderColor(defaultColor);
        Manager.instance.canExecute = true;
    }

    new private void Awake()
    {
        base.Awake();
        nodeTypes = NodeTypes.forLoop;
        ExecManager.onChangeBegin += LockUnlockAllInput;

        // start tpi
        if (handleEndArray.Length > 1)
            handleEndArray[1].loopArea = nodesLoopArea;
        if (handleStartArray.Length > 1)
            handleStartArray[1].loopArea = nodesLoopArea;

        waitBeforeLockInput = WaitBeforeLockingInputs();
        //end tpi
    }

    private void OnDestroy()
    {
        ExecManager.onChangeBegin -= LockUnlockAllInput;
        DestroyNode();
    }
    //start tpi

    /// <summary>
    /// Test if values inside the node are correct
    /// </summary>
    /// <returns>True if values are correct</returns>
    private bool ValidateInput()
    {
        if(startVar != "" && endVar != "" && stepVar != "" && incrementVar != "")
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Set the var that will be incremented through the for loop
    /// </summary>
    /// <param name="inputField">The input field for this setting</param>
    public void setIncrementVar(TMP_InputField inputField)
    {
        incrementVar = inputField.text;
        ChangeInput();
    }
    /// <summary>
    /// Set the start value of the for loop
    /// </summary>
    /// <param name="inputField">The input field for this setting</param>
    public void SetStart(TMP_InputField inputField)
    {
        startVar = inputField.text;
        ChangeInput();
    }
    /// <summary>
    /// Set the end value of the for loop
    /// </summary>
    /// <param name="inputField">The input field for this setting</param>
    public void SetEnd(TMP_InputField inputField)
    {
        endVar = inputField.text;
        ChangeInput();
    }
    /// <summary>
    /// Set the step at which the for loop is going
    /// </summary>
    /// <param name="inputField">The input field for this setting</param>
    public void SetStep(TMP_InputField inputField)
    {
        stepVar = inputField.text;
        ChangeInput();
    }
    //end tpi
    public override void LockUnlockAllInput(object sender, ExecManager.onChangeBeginEventArgs e)
    {
        LockUnlockAllInput(true);
    }
    // start tpi

    /// <summary>
    /// Lock all input fields of the node
    /// </summary>
    /// <param name="isLocked">If true, all input fields cannot be modified</param>
    public override void LockUnlockAllInput(bool isLocked)
    {
        IsInputLocked = isLocked;
        foreach (TMP_InputField inputField in inputFields)
        {
            inputField.enabled = !isLocked;
        }
        if (!isLocked)
            inputFields[0].Select();
    }

    /// <summary>
    /// Stop the coroutine that wait the end of two frame before locking inputs
    /// </summary>
    public void StopLocking()
    {
        StopCoroutine(waitBeforeLockInput);
    }
    
    /// <summary>
    /// Start the coroutine that wait the end of two frame before locking inputs
    /// </summary>
    public void LockAllInputWait()
    {
        StartCoroutine(waitBeforeLockInput);
    }

    /// <summary>
    /// Wait for the end of two frame and then lock inputs
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitBeforeLockingInputs()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        LockUnlockAllInput(true);
    }
    //end tpi

    public override void Execute()
    {
        // test if the robot has enough power to execute the node, if not he stop the code execution
        if (rs.robot.Power <= nodeExecPower)
        {
            ExecManager.Instance.StopExec();
            rs.End();
            ChangeBorderColor(defaultColor);
            Debugger.Log($"Le robot {rs.robot.robotName} n'a plus assez d'�nergie");
            return;
        }
        rs.robot.Power -= nodeExecPower;

        if (!ExecManager.Instance.isRunning)
            return;
        ChangeBorderColor(currentExecutedNode);

        if (varIncrement == null)
        {
            if(!int.TryParse(startVar, out varStart))
            {
                VarsManager.Var tempVar = rs.robot.varsManager.GetVar(startVar);
                if(tempVar != null)
                {
                    varStart = tempVar.Value;
                }
                else
                {
                    Debugger.LogError("Une erreur est survenue1");
                    return;
                }
            }
            varIncrement = rs.robot.varsManager.GetVar(incrementVar,varStart);
            if(varIncrement == null)
            {
                Debugger.LogError("Une erreur est survenue2");
                return;
            }
            if (!int.TryParse(endVar, out varEnd))
            {
                VarsManager.Var tempVar = rs.robot.varsManager.GetVar(endVar);
                if (tempVar != null)
                {
                    varEnd = tempVar.Value;
                }
                else
                {
                    Debugger.LogError("Une erreur est survenue3");
                    return;
                }
            }
            if (!int.TryParse(stepVar, out varStep))
            {
                VarsManager.Var tempVar = rs.robot.varsManager.GetVar(stepVar);
                if (tempVar != null)
                {
                    varStep = tempVar.Value;
                }
                else
                {
                    Debugger.LogError("Une erreur est survenue4");
                    return;
                }
            }
        }

        if(varIncrement.Value <= varEnd)
        {
            varIncrement.Value += varStep;
            varIncrement.Persist();

            // other code will go here
            IEnumerator coroutine = WaitBeforeCallingNextNode(nextNodeInside);
            StartCoroutine(coroutine);
        }
        else
        {
            varIncrement.Value = varStart;
            varIncrement.Persist();
            StartCoroutine("WaitBeforeCallingNextNode");
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
        varStep = 1;
        varEnd = 0;
        varIncrement = null;
        varStart = 0;
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
        serializableNode.nodeSettings.Add(incrementVar);
        serializableNode.nodeSettings.Add(startVar);
        serializableNode.nodeSettings.Add(endVar);
        serializableNode.nodeSettings.Add(stepVar);
        serializableNode.nodeSettings.Add(nextNodeInside.ToString());
        return serializableNode;
    }
    public override void DeSerializeNode(SerializableNode serializableNode)
    {
        id = serializableNode.id;
        nextNodeId = serializableNode.nextNodeId; //this is the next node in the execution order
        parentId = serializableNode.parentId;
        for (int i = 0; i < serializableNode.nodeSettings.Count-1; i++)
        {
            inputFields[i].text = serializableNode.nodeSettings[i];
        }
        incrementVar = serializableNode.nodeSettings[0];
        startVar = serializableNode.nodeSettings[1];
        endVar = serializableNode.nodeSettings[2];
        stepVar = serializableNode.nodeSettings[3];
        nextNodeInside = Convert.ToInt32(serializableNode.nodeSettings[4]);
        Resize(new Vector2(serializableNode.size[0], serializableNode.size[1]));
        NodesDict.Add(id, this);
    }
    #endregion
}
