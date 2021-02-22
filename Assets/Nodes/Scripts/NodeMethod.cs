using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;

public class NodeMethod : Nodes
{
    private string input;
    public TMP_Dropdown tMP_Dropdown;
    private RobotScript nextScript;
    Dictionary<int, string> options = new Dictionary<int, string>();

    new private void Start()
    {
        base.Start();
        UpdateScriptList();
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
        yield return new WaitForSeconds(executedColorTime / Manager.instance.execSpeed);
        ChangeBorderColor(defaultColor);
        CallNextNode();
    }

    public override void CallNextNode()
    {
        if (NodesDict.ContainsKey(nextNodeId))
            NodesDict[nextNodeId].Execute();
    }

    public override void PostExecutionCleanUp(object sender, EventArgs e)
    {
        Debug.Log("node methode clean up do nothing");
    }
}
