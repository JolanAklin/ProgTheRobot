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
using System.Linq;
using System;

public class NodeMethod : Nodes
{
    public TMP_Dropdown tMP_Dropdown;
    private int nextScriptId = 0;
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
        tMP_Dropdown.value = dropDownValue;
    }

    new private void Awake()
    {
        base.Awake();
        nodeTypes = NodeTypes.subProgram;
        ExecManager.onChangeBegin += LockUnlockAllInput;
        Manager.instance.onScriptAdded += UpdateScriptList;
    }

    public void OnDestroy()
    {
        ExecManager.onChangeBegin -= LockUnlockAllInput;
        Manager.instance.onScriptAdded -= UpdateScriptList;
    }

    public override void LockUnlockAllInput(object sender, ExecManager.onChangeBeginEventArgs e)
    {
        tMP_Dropdown.interactable = !e.started;
        IsInputLocked = e.started;
    }
    // start tpi
    public override void LockUnlockAllInput(bool isLocked)
    {
        tMP_Dropdown.enabled = !isLocked;
        IsInputLocked = isLocked;
        if (!isLocked)
            tMP_Dropdown.Show();
        else
            tMP_Dropdown.Hide();
    }
    public override void UpdateNextNodeId(int idDelta)
    {
        if (nextNodeId != -1)
            nextNodeId += idDelta;
    }
    //end tpi

    private bool ValidateInput()
    {
        return true;
    }

    private void UpdateScriptList(object sender, EventArgs e)
    {
        UpdateScriptList();
    }

    private void UpdateScriptList()
    {
        tMP_Dropdown.options.Clear();
        options.Clear();
        int x = 0;
        for (int i = 1; i < rs.robot.robotScripts.Count; i++)
        {
            RobotScript robotScript = rs.robot.robotScripts[i];
            if(rs.id != robotScript.id)
            {
                options.Add(x, new SubProgram()
                {
                    subProgramId = robotScript.id,
                    subProgramName = robotScript.name,
                });
                tMP_Dropdown.options.Add(new TMP_Dropdown.OptionData() { text = robotScript.name });
                x++;
            }
        }
    }


    public override void Execute()
    {
        nextScriptId = options[tMP_Dropdown.value].subProgramId;
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
            ExecManager.Instance.ShowVar();
        }
    }

    public override void PostExecutionCleanUp(object sender, EventArgs e)
    {
        ChangeBorderColor(defaultColor);
    }

    private int dropDownValue;

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
        dropDownValue = Convert.ToInt32(serializableNode.nodeSettings[0]);
        Resize(new Vector2(serializableNode.size[0], serializableNode.size[1]));
    }
    #endregion
}
