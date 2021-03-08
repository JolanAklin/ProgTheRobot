using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;
using System.Data;

public class NodeIf : Nodes
{
    private string input;
    private string[] inputSplited;
    public int nextNodeIdFalse;

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
        if (!ValidateInput())
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
        string[] delimiters = new string[] { "=", "<", ">", ">=", "<=", "<>" };
        inputSplited = input.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        if (inputSplited.Length > 2 || inputSplited.Length == 1)
            return false;
        return true;
    }

    public override void Execute()
    {
        if (!ExecManager.Instance.isRunning)
            return;
        ChangeBorderColor(currentExecutedNode);

        //string expression = inputSplited[0].Replace(" ", string.Empty).Trim();
        string[] delimiters = new string[] { " " };
        string[] expressionSplited1 = inputSplited[0].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        string expr = string.Join("", rs.robot.varsManager.ReplaceStringsByVar(expressionSplited1));
        if(expr == null)
        {
            Debugger.Log("Variable inconnue");
            ChangeBorderColor(errorColor);
            return;
        }
        int value1 = Convert.ToInt32(new DataTable().Compute(expr, null));
        //expression = inputSplited[1].Replace(" ", string.Empty).Trim();
        string[] expressionSplited2 = inputSplited[1].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        expr = string.Join("", rs.robot.varsManager.ReplaceStringsByVar(expressionSplited2));
        if (expr == null)
        {
            Debugger.Log("Variable inconnue");
            ChangeBorderColor(errorColor);
            return;
        }
        int value2 = Convert.ToInt32(new DataTable().Compute(expr, null));

        IEnumerator coroutine = WaitBeforeCallingNextNode(nextNodeIdFalse);
        if (input.Contains("="))
        {
            if (value1 == value2)
            {
                StartCoroutine("WaitBeforeCallingNextNode");
            }
            else
            {
                StartCoroutine(coroutine);
            }

        }else if (input.Contains("<"))
        {
            if (value1 < value2)
            {
                StartCoroutine("WaitBeforeCallingNextNode");
            }
            else
            {
                StartCoroutine(coroutine);
            }
        }
        else if(input.Contains(">"))
        {
            if (value1 > value2)
            {
                StartCoroutine("WaitBeforeCallingNextNode");
            }
            else
            {
                StartCoroutine(coroutine);
            }
        }
        else if(input.Contains(">="))
        {
            if (value1 >= value2)
            {
                StartCoroutine("WaitBeforeCallingNextNode");
            }
            else
            {
                StartCoroutine(coroutine);
            }
        }
        else if(input.Contains("<="))
        {
            if (value1 <= value2)
            {
                StartCoroutine("WaitBeforeCallingNextNode");
            }
            else
            {
                StartCoroutine(coroutine);
            }
        }
        else if(input.Contains("<>"))
        {
            if (value1 != value2)
            {
                StartCoroutine("WaitBeforeCallingNextNode");
            }
            else
            {
                StartCoroutine(coroutine);
            }
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
        //unused
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
            type = "test",
            position = new float[] { transform.position.x, transform.position.y, transform.position.z },
            nodeSettings = new List<string>(),
            size = new float[] { canvas.sizeDelta.x, canvas.sizeDelta.y },

        };
        serializableNode.nodeSettings.Add(input);
        serializableNode.nodeSettings.Add(nextNodeIdFalse.ToString());
        return serializableNode;
    }
    public override void DeSerializeNode(SerializableNode serializableNode)
    {
        id = serializableNode.id;
        nextNodeId = serializableNode.nextNodeId; //this is the next node in the execution order
        input = serializableNode.nodeSettings[0];
        inputField.text = input;
        nextNodeIdFalse = Convert.ToInt32(serializableNode.nodeSettings[1]);
        Resize(new Vector2(serializableNode.size[0], serializableNode.size[1]));
    }
    #endregion
}
