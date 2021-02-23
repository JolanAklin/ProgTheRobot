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
        // robot name, json
        foreach (KeyValuePair<int,Robot> robot in Robot.robots)
        {
            foreach (RobotScript rs in robot.Value.robotScripts)
            {
                string jsonScript = rs.SerializeScript();

                // check if the directory exist if not creates it
                if (!Directory.Exists(Application.persistentDataPath + "/Saves"))
                {
                    Directory.CreateDirectory(Application.persistentDataPath + "/Saves");
                }if (!Directory.Exists(Application.persistentDataPath + "/Saves/Scripts"))
                {
                    Directory.CreateDirectory(Application.persistentDataPath + "/Saves/Scripts");
                }

                string path = Application.persistentDataPath + "/Saves/Scripts/" + rs.name;
                var sr = File.CreateText(path);

                string dataToSave = jsonScript.Replace(@"\", "").Replace("\"{", "{").Replace("}\"", "}");
                sr.WriteLine(dataToSave);
                sr.Close();
            }
        }
    }
}
