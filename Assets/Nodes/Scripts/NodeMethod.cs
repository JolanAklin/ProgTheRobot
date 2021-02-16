using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NodeMethod : Nodes
{
    private string input;
    public TMP_Dropdown tMP_Dropdown;
    private RobotScript nextScript;

    new private void Start()
    {
        base.Start();
        UpdateScriptList();
    }

    private void UpdateScriptList()
    {
        List<string> options = new List<string>();
        for (int i = 1; i < rs.robot.robotScripts.Count; i++)
        {
            options.Add(rs.robot.robotScripts[i].name);
        }
        tMP_Dropdown.AddOptions(options);
    }

    public void ChangeSelected()
    {
        nextScript = rs.robot.robotScripts[tMP_Dropdown.value + 1];
    }

    public override void SerializeNode()
    {
        throw new System.NotImplementedException();
    }
    public override void DeSerializeNode()
    {
        throw new System.NotImplementedException();
    }
    public override void Execute()
    {
        throw new System.NotImplementedException();
    }
    public override void PostExecutionCleanUp()
    {
        throw new System.NotImplementedException();
    }
}
