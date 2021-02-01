using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Robot
{
    // robot params
    public Color color;
    public string robotName;
    public uint power;

    public static int nextid = 0;
    public static Dictionary<int, Robot> robots = new Dictionary<int, Robot>();
    public static int idSelected;
    public int id;

    public List<RobotScript> robotScripts = new List<RobotScript>();


    public Robot(Color color, string name, uint power)
    {
        this.color = color;
        this.robotName = name;
        this.power = power;
        Init();
    }

    private void Init()
    {
        // All robotscripts have a different id
        id = nextid;
        nextid++;
        robots.Add(id, this);
    }

    public static void DeleteRobot(int id)
    {
        robots.Remove(id);
    }

    public List.ListElement AddScript(RobotScript robotScript)
    {
        robotScripts.Add(robotScript);
        return robotScript.ConvertToListElement();
    }

    public List.ListElement CreateScript(string name)
    {
       return AddScript(new RobotScript(name));
    }

    public List<List.ListElement> ScriptToList()
    {
        List<List.ListElement> list = new List<List.ListElement>();
        list.Add(new List.ListElement { isAddScript = true, actionOnClick = () => { Manager.instance.list.AddChoice(this.CreateScript("test")); } });
        foreach (RobotScript scripts in robotScripts)
        {
            list.Add(scripts.ConvertToListElement());
        }
        return list;
    }

    public ListRobot.ListElement ConvertToListElement()
    {
        return new ListRobot.ListElement() { isAddRobot = false, robotColor = color, actionOnClick = () => {
            idSelected = this.id;
            Manager.instance.list.ChangeList(ScriptToList(),1); } };
    }
}
