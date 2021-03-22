using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;

public class NodeMethod : Nodes
{
    public TMP_Dropdown tMP_Dropdown;
    private RobotScript nextScript;
    Dictionary<int, string> options = new Dictionary<int, string>();

    new private void Start()
    {
        base.Start();
        UpdateScriptList();
    }

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
        tMP_Dropdown.interactable = !e.started;
    }

    private bool ValidateInput()
    {
        //if(nextScript != null)
        //{
        //    Debug.Log("there");
        //    if (nextScript.nodeStart != null)
        //    {
        //        Debug.Log("there1");
        //        return true;
        //    }
        //}
        //return false;
        return true;
    }

    public void ChangeSelected()
    {
        // there is one more script in the list and one more in the dropdown. dropdown.value return a one based index, so -1
        nextScript = rs.robot.robotScripts[options.ElementAt(tMP_Dropdown.value-1).Key];
        if (!ValidateInput())
        {
            ChangeBorderColor(errorColor);
            Manager.instance.canExecute = false;
            return;
        }
        Manager.instance.canExecute = true;
        ChangeBorderColor(defaultColor);
    }

    private void UpdateScriptList()
    {
        for (int i = 1; i < rs.robot.robotScripts.Count; i++)
        {
            options.Add(rs.robot.robotScripts[i].id, rs.robot.robotScripts[i].name);
            tMP_Dropdown.options.Add(new TMP_Dropdown.OptionData() { text = options.ElementAt(i-1).Value, });
        }
    }


    public override void Execute()
    {
        // test if the robot has enough power to execute the node, if not he stop the code execution
        if (rs.robot.power <= nodeExecPower)
        {
            ExecManager.Instance.StopExec();
            rs.End();
            ChangeBorderColor(defaultColor);
            Debugger.Log($"Le robot {rs.robot.robotName} n'a plus assez d'énergie");
            return;
        }
        rs.robot.power -= nodeExecPower;

        if (!ExecManager.Instance.isRunning)
            return;
        ChangeBorderColor(currentExecutedNode);

        if (nextScript.nodeStart != null)
        {
            rs.endCallBack = () => { StartCoroutine("WaitBeforeCallingNextNode"); };
            nextScript.nodeStart.Execute();
        }
        else
        {
            Debugger.Log("Il n'y a pas de bloc de départ dans le script spécifié");
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
        SerializableNode serializableNode = new SerializableNode()
        {
            id = id,
            nextNodeId = nextNodeId,
            type = "subProgram",
            position = new float[] { transform.position.x, transform.position.y, transform.position.z },
            nodeSettings = new List<string>(),
            size = new float[] { canvasRect.sizeDelta.x, canvasRect.sizeDelta.y },

        };
        serializableNode.nodeSettings.Add(tMP_Dropdown.value.ToString());
        return serializableNode;
    }
    public override void DeSerializeNode(SerializableNode serializableNode)
    {
        id = serializableNode.id;
        nextNodeId = serializableNode.nextNodeId; //this is the next node in the execution order
        tMP_Dropdown.value = Convert.ToInt32(serializableNode.nodeSettings[0]);
        Resize(new Vector2(serializableNode.size[0], serializableNode.size[1]));
    }
    #endregion
}
