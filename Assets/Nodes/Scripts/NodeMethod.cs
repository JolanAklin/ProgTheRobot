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
        ExecManager.onExecutionBegin += LockAllInput;
    }

    public void OnDestroy()
    {
        ExecManager.onExecutionBegin -= LockAllInput;
    }

    public void LockAllInput(object sender, ExecManager.onExecutionBeginEventArgs e)
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
    public class SerializedNodeMethod : SerializableNode
    {
        public int dropDownValue;
    }
    public override SerializableNode SerializeNode()
    {
        SerializedNodeMethod serializedNodeMethod = new SerializedNodeMethod() { id = id, nextNodeId = nextNodeId, dropDownValue = tMP_Dropdown.value };
        return serializedNodeMethod;
    }
    public override void DeSerializeNode()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
