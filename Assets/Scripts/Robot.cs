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
    public VarsManager varsManager;

    public static int nextid = 0;
    // store all the robot in this dictionnary
    public static Dictionary<int, Robot> robots = new Dictionary<int, Robot>();
    public static int idSelected;
    public int id;


    private RobotScript mainScript;
    public RobotScript MainScript { get => mainScript; private set => mainScript = value; }

    public List<RobotScript> robotScripts = new List<RobotScript>();



    /// <summary>
    /// Robot constructor
    /// </summary>
    /// <param name="color">Color of the robot</param>
    /// <param name="name">Name of the robot</param>
    /// <param name="power">Power of the robot</param>
    /// <param name="justCreated">If justCreated is at true, a Principal script will be created.</param>
    public Robot(Color color, string name, uint power, bool justCreated = false)
    {
        this.color = color;
        this.robotName = name;
        this.power = power;
        varsManager = new VarsManager();
        robotManager = Manager.instance.CreateRobot(color);
        Init(justCreated);
    }

    private void Init(bool justCreated = false)
    {
        // All robots have a different id
        id = nextid;
        nextid++;
        robots.Add(id, this);
        MainScript = new RobotScript("Principal", this);
        AddScript(MainScript);
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
       return AddScript(new RobotScript(name, this));
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

    public void ClearRobot()
    {
        robotScripts.Clear();
        mainScript = null;
        robotManager.DestroyRobotManager();
        varsManager.Clean();
        foreach (RobotScript rs in robotScripts)
        {
            rs.Clear();
        }
        robotScripts.Clear();
    }

    ~Robot()
    {
        Debug.Log($"{robotName} was destroyed");
    }

    #region Save 
    [Serializable]
    public class SerializedRobot
    {
        public int id;
        public string robotName;
        public uint power;
        public float[] robotColor;

        [SerializeField]
        public List<RobotScript.SerializedRobotScript> serializedRobotScripts;
    }

    public SerializedRobot SerializeRobot()
    {
        List<RobotScript.SerializedRobotScript> serializedRobotScripts = new List<RobotScript.SerializedRobotScript>();
        foreach (RobotScript rs in robotScripts)
        {
            serializedRobotScripts.Add(rs.SerializeScript());
        }
        SerializedRobot serializedRobot = new SerializedRobot()
        {
            id = id,
            robotName = robotName,
            power = power,
            robotColor = new float[4] { color.r, color.g, color.b, color.a },

            serializedRobotScripts = serializedRobotScripts,
        };
        return serializedRobot;
    }
    #endregion
}

