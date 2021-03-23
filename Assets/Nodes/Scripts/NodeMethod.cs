using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;

public class NodeMethod : Nodes
{
    public TMP_Dropdown tMP_Dropdown;
    private int nextScriptId;
    //Dictionary<int, string> options = new Dictionary<int, string>();
    Dictionary<int, SubProgram> options = new Dictionary<int, SubProgram>();

    private class SubProgram
    {
        public int subProgramId;
        public string subProgramName;
    }

    new private void Start()
    {
        base.Start();
        UpdateScriptList();
    }

    new private void Awake()
    {
        base.Awake();
        ExecManager.onChangeBegin += LockAllInput;
        Manager.instance.onScriptAdded += UpdateScriptList;
    }

    public void OnDestroy()
    {
        ExecManager.onChangeBegin -= LockAllInput;
        Manager.instance.onScriptAdded -= UpdateScriptList;
    }

    public void LockAllInput(object sender, ExecManager.onChangeBeginEventArgs e)
    {
        tMP_Dropdown.interactable = !e.started;
    }

    private bool ValidateInput()
    {
        return true;
    }

    public void ChangeSelected()
    {
        nextScriptId = options[tMP_Dropdown.value].subProgramId;
    }

    private void UpdateScriptList(object sender, EventArgs e)
    {
        UpdateScriptList();
    }

    private void UpdateScriptList()
    {
        tMP_Dropdown.options.Clear();
        options.Clear();
        for (int i = 1; i < rs.robot.robotScripts.Count; i++)
        {
            RobotScript robotScript = rs.robot.robotScripts[i];
            options.Add(i-1, new SubProgram()
            {
                subProgramId = robotScript.id,
                subProgramName = robotScript.name,
            });
            if(rs.id != robotScript.id)
                tMP_Dropdown.options.Add(new TMP_Dropdown.OptionData() { text = robotScript.name });
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

        if (RobotScript.robotScripts[nextScriptId].nodeStart != null)
        {
            RobotScript.robotScripts[nextScriptId].endCallBack = () => { CallNextNode(); };
            RobotScript.robotScripts[nextScriptId].nodeStart.Execute();
        }
        else
        {
            Debugger.Log("Il n'y a pas de bloc de départ dans le script spécifié");
        }
    }

    public override void CallNextNode()
    {

        if (!ExecManager.Instance.debugOn)
        {
            ChangeBorderColor(defaultColor);
            if (NodesDict.ContainsKey(nextNodeId))
                NodesDict[nextNodeId].Execute();
        }
        else
        {
            ExecManager.Instance.buttonNextAction = () => {
                if (NodesDict.ContainsKey(nextNodeId))
                    NodesDict[nextNodeId].Execute();
                ChangeBorderColor(defaultColor);
            };

        }
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
            parentId = parentId,
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
        parentId = serializableNode.parentId;
        tMP_Dropdown.value = Convert.ToInt32(serializableNode.nodeSettings[0]);
        Resize(new Vector2(serializableNode.size[0], serializableNode.size[1]));
    }
    #endregion
}
