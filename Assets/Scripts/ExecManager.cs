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
using UnityEngine.UI;
using System;

public class ExecManager : MonoBehaviour
{
    private static ExecManager instance;

    public static ExecManager Instance { get => instance; private set => instance = value; }

    public Button startButton;
    public Button stopButton;
    public Button nextStepButton;
    public Button modifyTerrainButton;

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
        if(CheckNode())
        {
            if(!isRunning)
            {
                Debugger.ClearDebug();
                isRunning = true;
                startButton.interactable = false;
                stopButton.interactable = true;
                modifyTerrainButton.interactable = false;
                if (debugOn)
                    nextStepButton.interactable = true;
                // call the start node from all main scripts
                foreach (Robot robot in Robot.robots.Values)
                {
                    if(robot.MainScript.nodeStart != null)
                    {
                        robot.robotManager.ResetTerrainObj();
                        robot.Power = robot.defaultPower;
                        robot.robotManager.transform.position = robot.robotManager.robotStartPos;
                        robot.robotManager.transform.rotation = robot.robotManager.robotStartRot;
                        robot.varsManager.Clean();
                        robot.MainScript.nodeStart.Execute();
                    }
                }
            }
        }
        else
        {
            Debugger.Log("Pas tout les blocs ont une configuration valide");
        }
    }
    


    public bool CheckNode()
    {
        Manager.instance.CheckNode?.Invoke(this, EventArgs.Empty);
        foreach (Nodes node in Nodes.NodesDict.Values)
        {
            if(node.NodeErrorCode != Nodes.ErrorCode.ok)
            {
                return false;
            }
        }
        return true;
    }

    public void InitializeScript()
    {
        CheckNode();
    }

    public void StopExec()
    {
        // test if all the robot have finished their program, if yes, inputs field can be modified again
        numberOfStopExecReceived++;
        if(numberOfStopExecReceived == Robot.robots.Count)
        {
            startButton.interactable = true;
            stopButton.interactable = false;
            nextStepButton.interactable = false;
            modifyTerrainButton.interactable = true;

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
            Debugger.Log($"xRobot de {robot.Value.robotName} : " + robot.Value.varsManager.GetFunction("xRobot").ToString());
            Debugger.Log($"yRobot de {robot.Value.robotName} : " + robot.Value.varsManager.GetFunction("yRobot").ToString());
            Debugger.Log($"dxRobot de {robot.Value.robotName} : " + robot.Value.varsManager.GetFunction("dxRobot").ToString());
            Debugger.Log($"dyRobot de {robot.Value.robotName} : " + robot.Value.varsManager.GetFunction("dyRobot").ToString());
            Debugger.Log($"énergie de {robot.Value.robotName} : " + robot.Value.varsManager.GetFunction("Energie").ToString());
            Debugger.Log($"xBallon de {robot.Value.robotName} : " + robot.Value.varsManager.GetFunction("xBallon").ToString());
            Debugger.Log($"yBallon de {robot.Value.robotName} : " + robot.Value.varsManager.GetFunction("yBallon").ToString());
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
