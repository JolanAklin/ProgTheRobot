using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Language;
using UnityEngine.UI;

// manages the whole program
public class Manager : MonoBehaviour
{
    public static Manager instance;

    // will be triggered when the language is changed by the toggle
    public event EventHandler OnLanguageChanged;

    // trigger an event where every node will test if there is an error in their config
    public event EventHandler CheckNode;
    public bool canExecute = true;

    // the canvas with all the UI
    public GameObject canvas;

    //List used to display script and robot
    public List list;
    public ListRobot listRobot;

    //change the resolution of the canvas when screen resolution changes
    private CanvasScaler canvasScaler;
    private Resolution res;

    [HideInInspector]
    public int currentlySelectedScript = -1;
    public GameObject nodeHolder; // all the nodes and spline are children of this object

    public GameObject robotPrefab;

    // define the execution speed of the script
    public float execSpeed;

    private void Awake()
    {
        instance = this;

        // change the canvas resolution to match the screen size
        canvasScaler = canvas.GetComponent<CanvasScaler>();
        res.height = Screen.height;
        res.width = Screen.width;
        canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
    }
    private void Start()
    {
        // load the translation
        Translation.Init();
        Translation.LoadData("fr");
        OnLanguageChanged?.Invoke(instance, EventArgs.Empty);

        // fill robots and scripts lists to test, will be removed
        Dictionary<int, ListRobot.ListElement> robotElements = new Dictionary<int, ListRobot.ListElement>();
        robotElements.Add(-1, new ListRobot.ListElement()
        {
            isAddRobot = true,
            actionOnClick = () =>
            {
                Robot robot = new Robot(Color.red, "", 2000, true);
                listRobot.AddChoice(robot.id, robot.ConvertToListElement());
                listRobot.Select(robot.id);
                ChangeRobotSettings();
            }
        });
        //robotElements.Add(robot.id, robot.ConvertToListElement());
        listRobot.Init(robotElements, 1);
    }

    private void Update()
    {
        // change the canvas resolution to match the screen size
        if (res.height != Screen.height || res.width != Screen.width)
        {
            canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
            res.height = Screen.height;
            res.width = Screen.width;
        }
    }

    public void ChangeSpeedSettings(Slider slider)
    {
        execSpeed = slider.value;
    }


    // robot customization
    public void ChangeRobotSettings()
    {
        if(Robot.robots.ContainsKey(Robot.idSelected))
        {
            Robot robotToChange = Robot.robots[Robot.idSelected];
            // instantiate a popup to modify the robot. Set all actions of the popup
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
        }
        else
        {
            Translation.LoadData("fr");
            OnLanguageChanged?.Invoke(instance, EventArgs.Empty);
        }
    }

    // create a spline and to connect to two nodes
    [HideInInspector]
    public Nodes node = null; // the node where the spline starts
    public GameObject SplineObject; // spline prefab
    private GameObject spline; // the current spline
    public EventHandler<OnSplineEventArgs> OnSpline; // is used to show the input of all the nodes
    private Action<int> actionWhenConnectionFinished;
    public class OnSplineEventArgs : EventArgs
    {
        public bool splineStarted;
    }
    public Nodes ConnectNode(bool isInput, Transform handleTransform, Nodes sender, Action<int> action)
    {
        if(node == null && !isInput)
        {
            // output. Where the spline is created
            actionWhenConnectionFinished = action;
            OnSpline?.Invoke(this, new OnSplineEventArgs() { splineStarted = true });
            instance.spline = Instantiate(SplineObject, Vector3.zero, Quaternion.identity, GameObject.FindGameObjectWithTag("NodeHolder").transform);
            instance.spline.GetComponent<SplineManager>().Init(handleTransform, sender);
            node = sender;
        }
        if (isInput)
        {
            // input. Where the spline ends
            OnSpline?.Invoke(this, new OnSplineEventArgs() { splineStarted = false });
            SplineManager splineManager = instance.spline.GetComponent<SplineManager>();
            splineManager.EndSpline(handleTransform, sender);
            //node.nextNodeId = sender.id;
            actionWhenConnectionFinished(sender.id);
            splineManager = null;
            return node;
        }
        return null;
    }

    // show and destroy nodes
    public void HideNodes()
    {
        foreach (Transform transform in nodeHolder.transform)
        {
            if(transform.gameObject.tag == "Node")
                transform.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            if (transform.gameObject.tag == "Spline")
                transform.gameObject.SetActive(false);
        }
    }

    public RobotManager CreateRobot(Color color)
    {
        return Instantiate(robotPrefab, new Vector3(0, 0.5f, 0), Quaternion.identity, transform.root).GetComponent<RobotManager>();
    }
}
