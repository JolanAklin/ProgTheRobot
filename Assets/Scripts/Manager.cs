using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Language;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public static Manager instance;

    public event EventHandler OnLanguageChanged;

    public GameObject canvas;

    //List used to display script and robot
    public List list;
    public ListRobot listRobot;

    //change the resolution of the canvas when screen resolution changes
    private CanvasScaler canvasScaler;
    private Resolution res;

    [HideInInspector]
    public int currentlySelectedScript = -1;
    public GameObject nodeHolder;

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

        canvasScaler = canvas.GetComponent<CanvasScaler>();
        res.height = Screen.height;
        res.width = Screen.width;
        canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
    }

    private void Update()
    {
        if(res.height != Screen.height || res.width != Screen.width)
        {
            canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
            res.height = Screen.height;
            res.width = Screen.width;

        }
    }

    private void Start()
    {
        Translation.Init();
        Translation.LoadData("fr");
        OnLanguageChanged?.Invoke(instance, EventArgs.Empty);

        // fill the list to test, while be removed
        List<Robot> robots = new List<Robot>();
        for (int i = 0; i < 3; i++)
        {
            Robot robot = new Robot(Color.blue, "Testbot", 100);
            robots.Add(robot);
            List<RobotScript> robotScripts = new List<RobotScript>();
            for (int j = 0; j < 10; j++)
            {
                robot.CreateScript($"Script {i}, {j}");
            }
        }
        Dictionary<int, ListRobot.ListElement> robotElements = new Dictionary<int, ListRobot.ListElement>();
        robotElements.Add(-1, new ListRobot.ListElement() { isAddRobot = true, actionOnClick = () => {
            Robot robot = new Robot(Color.red, "", 2000);
            listRobot.AddChoice(robot.id, robot.ConvertToListElement());
            listRobot.Select(robot.id);
            ChangeRobotSettings();
        } });
        foreach (Robot robot in robots)
        {
            robotElements.Add(robot.id, robot.ConvertToListElement());
        }
        listRobot.Init(robotElements, 1);
    }

    // robot customization
    public void ChangeRobotSettings()
    {
        if(Robot.robots.ContainsKey(Robot.idSelected))
        {
            Robot robotToChange = Robot.robots[Robot.idSelected];
            PopUpRobot rm = WindowsManager.InstantiateWindow((int)Enum.Parse(typeof(WindowsManager.popUp), "robotModif"), canvas.transform).GetComponent<PopUpRobot>();
            rm.Init(robotToChange.color, robotToChange.robotName, robotToChange.power);
            rm.SetOkAction(() =>
            {
                robotToChange.color = rm.robotColor;
                robotToChange.robotName = rm.robotName;
                robotToChange.power = rm.power;
                listRobot.UpdateButtonColor();
                rm.PopUpClose();
            });
            rm.SetCancelAction(() =>
            {
                rm.PopUpClose();
            });
            rm.SetDeleteAction(() =>
            {
                listRobot.RemoveRobot(robotToChange.id);
                Robot.DeleteRobot(robotToChange.id);
                rm.PopUpClose();
            });
        }
    }

    public void ChangeLanguage(ToggleScript toggle)
    {
        if(toggle.Value)
        {
            Translation.LoadData("eng");
            OnLanguageChanged?.Invoke(instance, EventArgs.Empty);
        }else
        {
            Translation.LoadData("fr");
            OnLanguageChanged?.Invoke(instance, EventArgs.Empty);
        }
    }

    // show and destroy nodes
    public void CreateNodeObject(GameObject node)
    {
        Instantiate(node, node.transform.position, Quaternion.identity,nodeHolder.transform);
    }
    public void DeleteNodes()
    {
        foreach (Transform transform in nodeHolder.transform)
        {
            Destroy(transform.gameObject);
        }
    }
}
