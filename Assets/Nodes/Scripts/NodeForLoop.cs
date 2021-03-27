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
using System;
using Language;

public class NodeForLoop : Nodes
{
    private string input;
    public TMP_InputField inputField;
    private string[] inputSplited;

    public int nextNodeInside = -1;

    private VarsManager.Var varIncrement;
    private int varStart = 0;
    private int varEnd;
    private int varStep = 1;
    public void ChangeInput(TMP_InputField tMP_InputField)
    {
        input = tMP_InputField.text;
        inputField = tMP_InputField;
        inputSplited = input.Split(' ');
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
        Manager.instance.OnLanguageChanged += TranslateText;
        ExecManager.onChangeBegin += LockAllInput;
    }

    private void OnDestroy()
    {
        Manager.instance.OnLanguageChanged -= TranslateText;
        ExecManager.onChangeBegin -= LockAllInput;
    }

    public void LockAllInput(object sender, ExecManager.onChangeBeginEventArgs e)
    {
        inputField.interactable = !e.started;
    }

    public bool ValidateInput()
    {
        // need to add the var support
        // Pour i = 0 Jusque 2 Pas 1
        // For i = 0 UpTo 10 Step 2
        // Pour i = var1 Jusque var2 Step var3
        // Pour i = 0 Jusque 3
        if(input.Length > 0)
        {
            if(inputSplited.Length != 0)
                try
                {
                    if(inputSplited[0] == "Pour" || inputSplited[0] == "For")
                        if(VarsManager.CheckVarName(inputSplited[1]))
                            if (inputSplited[2] == "=")
                                if (inputSplited[4] == "Jusque" || inputSplited[4] == "UpTo")
                                { 
                                    varStep = 1;
                                    if(inputSplited.Length == 8)
                                    {
                                        if (!(inputSplited[6] == "Pas" || inputSplited[6] == "Step"))
                                        {
                                            return false;
                                        }
                                    }
                                    if (inputSplited.Length > 6 && inputSplited.Length != 8)
                                        return false;
                                    TranslateText(this, EventArgs.Empty);
                                    return true;
                                }
                        return false;

                }catch
                {
                    return false;
                }
        }
        return true;
    }

    // translate the text inside the node
    private void TranslateText(object sender, EventArgs e)
    {
        if(Translation.CurrentLanguage == "eng")
        {
            input = input.Replace("Pour", "For");
            input = input.Replace("Jusque", "UpTo");
            input = input.Replace("Pas", "Step");

        }
        if (Translation.CurrentLanguage == "fr")
        {
            input = input.Replace("For", "Pour");
            input = input.Replace("UpTo", "Jusque");
            input = input.Replace("Step", "Pas");
        }

        inputField.text = input;
    }

    public override void Execute()
    {
        // test if the robot has enough power to execute the node, if not he stop the code execution
        if (rs.robot.power <= nodeExecPower)
        {
            ExecManager.Instance.StopExec();
            rs.End();
            ChangeBorderColor(defaultColor);
            Debugger.Log($"Le robot {rs.robot.robotName} n'a plus assez d'�nergie");
            return;
        }
        rs.robot.power -= nodeExecPower;

        if (!ExecManager.Instance.isRunning)
            return;
        ChangeBorderColor(currentExecutedNode);

        if (varIncrement == null)
        {
            if(!int.TryParse(inputSplited[3], out varStart))
            {
                VarsManager.Var tempVar = rs.robot.varsManager.GetVar(inputSplited[3]);
                if(tempVar != null)
                {
                    varStart = tempVar.Value;
                }
                else
                {
                    Debugger.LogError("Une erreur est survenue");
                    return;
                }
            }
            varIncrement = rs.robot.varsManager.GetVar(inputSplited[1],varStart);
            if(varIncrement == null)
            {
                Debugger.LogError("Une erreur est survenue");
                return;
            }
            if (!int.TryParse(inputSplited[5], out varEnd))
            {
                VarsManager.Var tempVar = rs.robot.varsManager.GetVar(inputSplited[5]);
                if (tempVar != null)
                {
                    varEnd = tempVar.Value;
                }
                else
                {
                    Debugger.LogError("Une erreur est survenue");
                    return;
                }
            }
            if(inputSplited.Length > 6)
            {
                if (!int.TryParse(inputSplited[7], out varStep))
                {
                    VarsManager.Var tempVar = rs.robot.varsManager.GetVar(inputSplited[7]);
                    if (tempVar != null)
                    {
                        varStep = tempVar.Value;
                    }
                    else
                    {
                        Debugger.LogError("Une erreur est survenue");
                        return;
                    }
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
        serializableNode.nodeSettings.Add(input);
        serializableNode.nodeSettings.Add(nextNodeInside.ToString());
        return serializableNode;
    }
    public override void DeSerializeNode(SerializableNode serializableNode)
    {
        id = serializableNode.id;
        nextNodeId = serializableNode.nextNodeId; //this is the next node in the execution order
        parentId = serializableNode.parentId;
        input = serializableNode.nodeSettings[0];
        inputField.text = input;
        nextNodeInside = Convert.ToInt32(serializableNode.nodeSettings[1]);
        Resize(new Vector2(serializableNode.size[0], serializableNode.size[1]));
    }
    #endregion
}
