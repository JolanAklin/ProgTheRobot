using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System.Data;
using System;

public class NodeAffect : Nodes
{
    private string input;
    private string[] inputSplited;

    private VarsManager.Var var;

    public TMP_InputField inputField;

    new private void Awake()
    {
        base.Awake();
        ExecManager.onChangeBegin += LockAllInput;
    }

    public void OnDestroy()
    {
        ExecManager.onChangeBegin -= LockAllInput;
    }

    public void LockAllInput(object sender, ExecManager.onChangeBeginEventArgs e)
    {
        inputField.interactable = !e.started;
    }

    public void ChangeInput(TMP_InputField tMP_InputField)
    {
        input = tMP_InputField.text;
        if(!ValidateInput())
        {
            ChangeBorderColor(errorColor);
            Manager.instance.canExecute = false;
            return;
        }
        Manager.instance.canExecute = true;
        ChangeBorderColor(defaultColor);
    }

    private bool ValidateInput()
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
        return false;
    }

    public override void Execute()
    {
        if (!ExecManager.Instance.isRunning)
            return;
        ChangeBorderColor(currentExecutedNode);
        // calculate and set the var
        string[] inputVarReplaced = rs.robot.varsManager.ReplaceStringsByVar((string[])inputSplited.Clone());
        if (inputVarReplaced != null)
        {
            if (var == null)
            {
                if(VarsManager.CheckVarName(inputSplited[0]))
                {
                    var = rs.robot.varsManager.GetVar(inputSplited[0]);
                    if (var == null)
                    {
                        Debugger.LogError("Une erreur est survenue");
                        return;
                    }
                }
            }

            string expression = string.Join("", inputVarReplaced, 2, inputVarReplaced.Length - 2).Trim();
            var.Value = Convert.ToInt32(new DataTable().Compute(expression, null));
            var.Persist();
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
            nextNodeId = nextNodeId,
            type = "execute",
            position = new float[] { transform.position.x, transform.position.y, transform.position.z },
            nodeSettings = new List<string>() };
        serializableNode.nodeSettings.Add(input);
        return serializableNode;
    }
    public override void DeSerializeNode()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
