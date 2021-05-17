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
    private string input;
    private string[] inputSplited;

    private VarsManager.Var var;

    public TMP_InputField inputField;

    new private void Awake()
    {
        base.Awake();
        nodeTypes = NodeTypes.affectation;
        ExecManager.onChangeBegin += LockUnlockAllInput;
    }

    public void OnDestroy()
    {
        ExecManager.onChangeBegin -= LockUnlockAllInput;
        DestroyNode();
    }

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
        inputField.enabled = !isLocked;
        IsInputLocked = isLocked;
        if (!isLocked)
            inputField.Select();
    }
    //end tpi

    public void ChangeInput(TMP_InputField tMP_InputField)
    {
        input = tMP_InputField.text;
        input = FormatInput(input);
        inputField.text = input;
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

    // start tpi
    /// <summary>
    /// Format the string
    /// </summary>
    /// <param name="input">the string to format</param>
    /// <returns>The formated string</returns>
    private string FormatInput(string input)
    {
        input = input.Replace("=", " = ");
        input = input.Replace("<", " < ");
        input = input.Replace(">", " > ");
        input = input.Replace("<=", " <= ");
        input = input.Replace(">=", " >= ");
        input = input.Replace("<>", " <> ");
        input = input.Replace("+", " + ");
        input = input.Replace("-", " - ");
        input = input.Replace("*", " * ");
        input = input.Replace("/", " / ");
        input = input.Replace("(", " ( ");
        input = input.Replace(")", " ) ");

        string pattern = @"\s+";
        input = Regex.Replace(input, pattern, " ");
        return input;
    }
    // end tpi

    private bool ValidateInput()
    {
        if(input.Length > 0)
        {
            string[] delimiters = new string[] { " " };
            inputSplited = input.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            if (inputSplited.Length <= 2)
                return false;
            if (!inputSplited[0].Any(char.IsDigit))
            {
                try
                {
                    if (inputSplited[1] == "=")
                    {
                        for (int i = 2; i < inputSplited.Length; i++)
                        {
                            if (!(inputSplited[i].Any(Char.IsDigit) || inputSplited[i].Any(Char.IsLetter)))
                            {
                                //return VarsManager.CheckVarName(inputSplited[i]);
                                switch (inputSplited[i])
                                {
                                    case "+":
                                    case "-":
                                    case "*":
                                    case "/":
                                    case "(":
                                    case ")":
                                        break;
                                    default:
                                        return false;
                                }
                            }
                        }
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }
        else
        {
            return true;
        }
        return false;
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
        string[] inputVarReplaced = rs.robot.varsManager.ReplaceFunctionByValue(input).Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        if (inputVarReplaced != null)
        {
            if (var == null)
            {
                if(VarsManager.CheckVarName(inputSplited[0]))
                {
                    // start tpi
                    string expression = string.Join("", inputVarReplaced, 2, inputVarReplaced.Length - 2).Trim();
                    var = rs.robot.varsManager.GetVar(inputSplited[0], Convert.ToInt32(new DataTable().Compute(expression, null)));
                    // end tpi
                    if (var == null)
                    {
                        Debugger.LogError("Une erreur est survenue");
                        return;
                    }
                }
            }

        }
        else
        {
            // stop execution
            //Debugger.LogError("La variable spécifiée n'est pas connue");
            //ChangeBorderColor(errorColor);
            rs.robot.varsManager.GetVar(inputSplited[0], 0);
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
        serializableNode.nodeSettings.Add(input);
        return serializableNode;
    }
    public override void DeSerializeNode(SerializableNode serializableNode)
    {
        id = serializableNode.id;
        nextNodeId = serializableNode.nextNodeId; //this is the next node in the execution order
        parentId = serializableNode.parentId;
        input = serializableNode.nodeSettings[0];
        inputField.text = input;
        Resize(new Vector2(serializableNode.size[0], serializableNode.size[1]));
        ValidateInput();
        NodesDict.Add(id, this);
    }
    #endregion
}
