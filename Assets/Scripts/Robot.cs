using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot
{
    // robot params
    private Color color;
    public Color Color { get => color; set => color = value; }
    private string name;
    public string Name { get => name; set => name = value; }
    private uint power;
    public uint Power { get => power; set => power = value; }

    public static int nextid = 0;
    public static Dictionary<int, Robot> robots = new Dictionary<int, Robot>();
    public int id;

    public List<RobotScript> robotScripts = new List<RobotScript>();


    public Robot(Color color, string name, uint power)
    {
        Color = color;
        Name = name;
        Power = power;
        Init();
    }

    private void Init()
    {
        // All robotscripts have a different id
        id = nextid;
        nextid++;
        robots.Add(id, this);
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
        foreach (RobotScript scripts in robotScripts)
        {
            list.Add(scripts.ConvertToListElement());
        }
        list.Add(new List.ListElement { isAddScript = true, actionOnClick = () => { Manager.instance.list.AddChoice(this.CreateScript("test")); } });
        return list;
    }

    public ListRobot.ListElement ConvertToListElement()
    {
        return new ListRobot.ListElement() { isAddRobot = false, robotColor = Color, actionOnClick = () => { Manager.instance.list.ChangeList(ScriptToList(),0); } };
    }
}
