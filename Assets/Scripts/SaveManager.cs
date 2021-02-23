using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text.RegularExpressions;

public class SaveManager : MonoBehaviour
{

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            Save();
        }
    }

    public void Save()
    {
        string basePath = Application.persistentDataPath + "/Saves/";
        Debug.Log(Application.persistentDataPath);
        StreamWriter sr;
        // save the robot with his script and their nodes
        foreach (KeyValuePair<int,Robot> robot in Robot.robots)
        {
            string jsonRobot = JsonUtility.ToJson(robot.Value.SerializeRobot());
            // check if the directory exist if not creates it
            if (!Directory.Exists(Application.persistentDataPath + "/Saves"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/Saves");
            }

            sr = File.CreateText(basePath + robot.Value.robotName);
            sr.WriteLine(jsonRobot);
            sr.Close();
        }

        // save next id
        sr = File.CreateText(basePath + "Ids");
        SaveId saveId = new SaveId()
        {
            robotNextId = Robot.nextid,
            robotScriptNextId = Robot.nextid,
            nodeNextId = Nodes.nextid,
        };
        sr.WriteLine(JsonUtility.ToJson(saveId));
        sr.Close();

        // save spline
        string splineJson = "";
        foreach (SplineManager splineManager in SplineManager.splineManagers)
        {
            splineJson += JsonUtility.ToJson(splineManager.SerializeSpline());
        }
        sr = File.CreateText(basePath + "Splines");
        sr.WriteLine(splineJson);
        sr.Close();

        // save the terrain

    }

    [Serializable]
    public class SaveId
    {
        public int robotNextId;
        public int robotScriptNextId;
        public int nodeNextId;
    }
}
