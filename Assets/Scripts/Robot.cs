using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Robot
{
    // robot params
    private Color color;
    public Color Color { get => color; set {
            color = value;
            if (robotManager != null)
                robotManager.SetRobotColor(color);
        } }

    public string robotName;
    public uint defaultPower;
    public uint power;

    public RobotManager robotManager;
    public VarsManager varsManager;

    public static int nextid = 0;
    // store all the robot in this dictionnary
    public static Dictionary<int, Robot> robots = new Dictionary<int, Robot>();
    public static int idSelected;
    public int id;


    private RobotScript mainScript;
    public RobotScript MainScript { get => mainScript; set => mainScript = value; }

    public List<RobotScript> robotScripts = new List<RobotScript>();



    /// <summary>
    /// Robot constructor
    /// </summary>
    /// <param name="color">Color of the robot</param>
    /// <param name="name">Name of the robot</param>
    /// <param name="power">Power of the robot</param>
    public Robot(Color color, string name, uint power)
    {
        robotManager = Manager.instance.CreateRobot(color, this);
        this.Color = color;
        this.robotName = name;
        this.defaultPower = power;
        varsManager = new VarsManager(robotManager, this);
        Init();
    }

    public Robot(int id, uint power, float[] robotColor, string robotName, Vector3 position, Quaternion rotation, List<RobotScript.SerializedRobotScript> serializedRobotScripts, SaveManager saveManager)
    {
        robotManager = Manager.instance.CreateRobot(Color, this, position, rotation);
        this.id = id;
        this.defaultPower = power;
        this.Color = new Color(robotColor[0], robotColor[1], robotColor[2], robotColor[3]);
        this.robotName = robotName;
        varsManager = new VarsManager(robotManager, this);
        robots.Add(id, this);
        bool isMain = true;
        foreach (RobotScript.SerializedRobotScript serializedRobotScript in serializedRobotScripts)
        {
            RobotScript robotScript = new RobotScript(serializedRobotScript, saveManager, isMain);
            if (isMain)
                isMain = false;
        }
    }

    private void Init()
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
        Manager.instance.DestroyObject(robots[id].robotManager.gameObject);
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
        // add a "+" button. The button will show a popup asking for the name of the script once clicked
        list.Add(new List.ListElement { isAddScript = true, actionOnClick = () => {
            // create the add script popup
            PopUpAddScript pas = WindowsManager.InstantiateWindow((int)Enum.Parse(typeof(WindowsManager.popUp), "addScript"), Manager.instance.canvas.transform).GetComponent<PopUpAddScript>();
            // close the popup when the "annuler" button is pressed
            pas.SetCancelAction(() =>
            {
                pas.PopUpClose();
            });
            // create the script with the right name and close the window
            pas.SetOkAction(() =>
            {
                List.ListElement element = this.CreateScript(pas.scriptName);
                Manager.instance.list.AddChoice(element);
                Manager.instance.list.SelectLast();
                Manager.instance.onScriptAdded?.Invoke(this, EventArgs.Empty);
                pas.PopUpClose();
            });
        } });
        // show all the already created script on the list
        foreach (RobotScript scripts in robotScripts)
        {
            list.Add(scripts.ConvertToListElement());
        }
        return list;
    }

    // convert the robot to a displayable list element
    public ListRobot.ListElement ConvertToListElement()
    {
        return new ListRobot.ListElement() { isAddRobot = false, robotColor = Color, actionOnClick = () => {
                idSelected = this.id;
                Manager.instance.list.ChangeList(ScriptToList(),1); 
            } 
        };
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

    #region Save 
    [Serializable]
    public class SerializedRobot
    {
        public int id;
        public string robotName;
        public uint power;
        public float[] robotColor;

        public float[] position;
        public float[] rotation;

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
            power = defaultPower,
            robotColor = new float[4] { Color.r, Color.g, Color.b, Color.a },

            serializedRobotScripts = serializedRobotScripts,
        };
        serializedRobot.position = new float[3] { robotManager.robotStartPos.x, robotManager.robotStartPos.y, robotManager.robotStartPos.z };
        serializedRobot.rotation = new float[4] { robotManager.robotStartRot.x, robotManager.robotStartRot.y, robotManager.robotStartRot.z, robotManager.robotStartRot.w };
        return serializedRobot;
    }
    #endregion
}

