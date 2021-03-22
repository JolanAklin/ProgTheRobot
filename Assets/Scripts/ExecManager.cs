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

    private int numberOfStopExecReceived = 0;
    

    private void Awake()
    {
        instance = this;
    }

    public void StartExec()
    {
        // this event is used to lock all the inputs of nodes
        onChangeBegin?.Invoke(this, new onChangeBeginEventArgs() { started = true });
        if(Manager.instance.canExecute)
        {
            if(!isRunning)
            {
                Debugger.ClearDebug();
                isRunning = true;
                // call the start node from all main scripts
                foreach (KeyValuePair<int,Robot> robot in Robot.robots)
                {
                    if(robot.Value.MainScript.nodeStart != null)
                    {
                        robot.Value.MainScript.nodeStart.Execute();
                    }
                }
            }
        }
        else
        {
            Debugger.Log("Pas tout les blocs ont une configuration valide");
        }
    }
    public void StopExec()
    {
        // test if all the robot have finished their program, if yes, inputs field can be modified again
        numberOfStopExecReceived++;
        if(numberOfStopExecReceived == Robot.robots.Count)
        {
            numberOfStopExecReceived = 0;

            Instance.isRunning = false;
            onChangeBegin?.Invoke(this, new onChangeBeginEventArgs() { started = false });
            buttonNextAction = null;
        }
    }


    // debugging stuff
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
