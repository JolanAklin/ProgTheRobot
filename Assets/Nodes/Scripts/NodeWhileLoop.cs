using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Language;
using System.Data;

public class NodeWhileLoop : Nodes
{
    private string input;
    private string[] inputSplited;
    public TMP_InputField inputField;

    public int nextNodeInside = -1;

    public void ChangeInput(TMP_InputField tMP_InputField)
    {
        input = tMP_InputField.text;
        inputField = tMP_InputField;
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

    private bool ValidateInput()
    {
        try
        {
            string[] delimiters = new string[] { "=", "<", ">", ">=", "<=", "<>" };
            inputSplited = input.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            if (!(inputSplited[0].IndexOf("TantQue") == 0 || inputSplited[0].IndexOf("While") == 0))
                return false;
            if (inputSplited.Length > 2 || inputSplited.Length == 1)
                return false;
            delimiters = new string[] { "While", "TantQue" };
            string[] inputSplitedFirstPart = inputSplited[0].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            if (inputSplitedFirstPart.Length > 1 || inputSplitedFirstPart[0] == "TantQue" || inputSplitedFirstPart[0] == "While")
                return false;
            TranslateText(this, EventArgs.Empty);
            return true;
        }catch(Exception e)
        {
            return false;
        }
    }

    private void TranslateText(object sender, EventArgs e)
    {
        if (Translation.CurrentLanguage == "eng")
        {
            input = input.Replace("TantQue", "While");
        }
        if (Translation.CurrentLanguage == "fr")
        {
            input = input.Replace("While", "TantQue");
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

        string[] delimiters = new string[] { " ", "While", "TantQue" };
        string[] expressionSplited1 = inputSplited[0].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        string expr = string.Join("", rs.robot.varsManager.ReplaceStringsByVar(expressionSplited1));
        if (expr == null)
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


        IEnumerator coroutine = WaitBeforeCallingNextNode(nextNodeInside);
        if (input.Contains("="))
        {
            if (value1 == value2)
            {
                StartCoroutine(coroutine);
            }
            else
            {
                StartCoroutine("WaitBeforeCallingNextNode");
            }

        }
        else if (input.Contains("<"))
        {
            if (value1 < value2)
            {
                StartCoroutine(coroutine);
            }
            else
            {
                StartCoroutine("WaitBeforeCallingNextNode");
            }
        }
        else if (input.Contains(">"))
        {
            if (value1 > value2)
            {
                StartCoroutine(coroutine);
            }
            else
            {
                StartCoroutine("WaitBeforeCallingNextNode");
            }
        }
        else if (input.Contains(">="))
        {
            if (value1 >= value2)
            {
                StartCoroutine(coroutine);
            }
            else
            {
                StartCoroutine("WaitBeforeCallingNextNode");
            }
        }
        else if (input.Contains("<="))
        {
            if (value1 <= value2)
            {
                StartCoroutine(coroutine);
            }
            else
            {
                StartCoroutine("WaitBeforeCallingNextNode");
            }
        }
        else if (input.Contains("<>"))
        {
            if (value1 != value2)
            {
                StartCoroutine(coroutine);
            }
            else
            {
                StartCoroutine("WaitBeforeCallingNextNode");
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
            type = "whileLoop",
            position = new float[] { transform.position.x, transform.position.y, transform.position.z },
            nodeSettings = new List<string>(),
            size = new float[] { canvas.sizeDelta.x, canvas.sizeDelta.y },

        };
        serializableNode.nodeSettings.Add(input);
        serializableNode.nodeSettings.Add(nextNodeInside.ToString());
        return serializableNode;
    }
    public override void DeSerializeNode(SerializableNode serializableNode)
    {
        id = serializableNode.id;
        nextNodeId = serializableNode.nextNodeId; //this is the next node in the execution order
        input = serializableNode.nodeSettings[0];
        inputField.text = input;
        nextNodeInside = Convert.ToInt32(serializableNode.nodeSettings[1]);
        Resize(new Vector2(serializableNode.size[0], serializableNode.size[1]));
        ValidateInput();
    }
    #endregion
}
