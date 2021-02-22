using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecManager : MonoBehaviour
{
    private static ExecManager instance;

    public static ExecManager Instance { get => instance; private set => instance = value; }

    [HideInInspector]
    public bool isRunning = false;

    

    

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
        if(!isRunning)
        {
            isRunning = true;
            foreach (KeyValuePair<int,Robot> robot in Robot.robots)
            {
                robot.Value.MainScript.nodeStart.Execute();
            }
        }
    }
}
