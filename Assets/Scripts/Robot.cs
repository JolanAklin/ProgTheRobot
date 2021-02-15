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

    public RobotManager robotManager;

    public static int nextid = 0;
    // store all the robot in this dictionnary
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
        // All robots have a different id
        id = nextid;
        nextid++;
        robots.Add(id, this);
    }

    public static void DeleteRobot(int id)
    {
        robots.Remove(id);
    }

    // add a script to this robot
    public List.ListElement AddScript(RobotScript robotScript)
    {
        robotScripts.Add(robotScript);
        return robotScript.ConvertToListElement();
    }


    public List.ListElement CreateScript(string name)
    {
       return AddScript(new RobotScript(name));
    }

    // convert all scripts to a displayable list element
    public List<List.ListElement> ScriptToList()
    {
        List<List.ListElement> list = new List<List.ListElement>();
        // add a "add" button. The button will show a popup asking for the name of the script once clicked
        list.Add(new List.ListElement { isAddScript = true, actionOnClick = () => {
            PopUpAddScript pas = WindowsManager.InstantiateWindow((int)Enum.Parse(typeof(WindowsManager.popUp), "addScript"), Manager.instance.canvas.transform).GetComponent<PopUpAddScript>();
            pas.SetCancelAction(() =>
            {
                pas.PopUpClose();
            });
            pas.SetOkAction(() =>
            {
                List.ListElement element = this.CreateScript(pas.scriptName);
                Manager.instance.list.AddChoice(element);
                Manager.instance.list.SelectLast();
                pas.PopUpClose();
            });
        } });
        foreach (RobotScript scripts in robotScripts)
        {
            list.Add(scripts.ConvertToListElement());
        }
        return list;
    }

    // convert the robot to a displayable list element
    public ListRobot.ListElement ConvertToListElement()
    {
        return new ListRobot.ListElement() { isAddRobot = false, robotColor = color, actionOnClick = () => {
            idSelected = this.id;
            Manager.instance.list.ChangeList(ScriptToList(),1); } };
    }
}
