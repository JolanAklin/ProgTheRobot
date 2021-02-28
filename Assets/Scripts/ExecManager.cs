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

    public static EventHandler<onChangeBeginEventArgs> onChangeBegin; // triggered when the execution is started or stopped

    public class onChangeBeginEventArgs : EventArgs
    {
        public bool started;
    }


    // debugging stuff
    public Action buttonNextAction;
    [HideInInspector]
    public bool debugOn = false;
    

    private void Awake()
    {
        instance = this;
    }

    public void StartExec()
    {
        onChangeBegin?.Invoke(this, new onChangeBeginEventArgs() { started = true });
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
        onChangeBegin?.Invoke(this, new onChangeBeginEventArgs() { started = false });
        buttonNextAction = null;
    }


    // bugging stuff
    public void ActivateDebug(ToggleScript toggle)
    {
        debugOn = toggle.Value;
        Debug.Log($"debug {debugOn}");
    }
    // called when the next button is clicked when in debug mode
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
