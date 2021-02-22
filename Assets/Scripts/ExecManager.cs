using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ExecManager : MonoBehaviour
{
    private static ExecManager instance;

    public static ExecManager Instance { get => instance; private set => instance = value; }

    [HideInInspector]
    public bool isRunning = false;

    public static EventHandler<onExecutionBeginEventArgs> onExecutionBegin;

    public class onExecutionBeginEventArgs : EventArgs
    {
        public bool started;
    }


    //debugging stuff
    public Action buttonNextAction;
    [HideInInspector]
    public bool debugOn = false;
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void StartExec()
    {
        onExecutionBegin?.Invoke(this, new onExecutionBeginEventArgs() { started = true });
        if(!isRunning)
        {
            isRunning = true;
            foreach (KeyValuePair<int,Robot> robot in Robot.robots)
            {
                robot.Value.MainScript.nodeStart.Execute();
            }
        }
    }
    public void StopExec()
    {
        Instance.isRunning = false;
        onExecutionBegin?.Invoke(this, new onExecutionBeginEventArgs() { started = false });
        buttonNextAction = null;
    }


    // bugging stuff
    public void ActivateDebug(ToggleScript toggle)
    {
        debugOn = toggle.Value;
        Debug.Log($"debug {debugOn}");
    }
    public void NextInDebug()
    {
        if(debugOn)
        {
            ShowVar();
            buttonNextAction?.Invoke();
        }
    }
    public void ShowVar()
    {
        Debugger.ClearDebug();
        foreach (KeyValuePair<int,Robot> robot in Robot.robots)
        {
            foreach (string var in robot.Value.varsManager.GetAllVars())
            {
                Debugger.Log(var);
            }
        }
    }
    public void Stop()
    {
        StopExec();
        foreach (KeyValuePair<int, RobotScript> rs in RobotScript.robotScripts)
        {
            rs.Value.End();
        }
    }
}
