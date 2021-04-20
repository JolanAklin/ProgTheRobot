// Copyright 2021 Jolan Aklin

//This file is part of Prog the robot.

//Prog the robot is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 3 of the License.

//Prog the robot is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Prog the robot.  If not, see<https://www.gnu.org/licenses/>.

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
    public EventHandler CheckNode;
    public bool canExecute = true;

    // update the list of all sub program nodes
    public EventHandler onScriptAdded;

    // the canvas with all the UI
    public GameObject canvas;
    public AspectRatioFitter smallRobotViewAspectRation;

    //List used to display script and robot
    public List list;
    public ListRobot listRobot;

    //change the resolution of the canvas when screen resolution changes
    private CanvasScaler canvasScaler;

    [HideInInspector]
    public int currentlySelectedScript = -1;
    public GameObject nodeHolder; // all the nodes and spline are children of this object

    public GameObject robotPrefab;

    // define the execution speed of the script
    public float execSpeed;

    public int selectedNodeId = -1;

    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        // load the translation
        Translation.Init();
        Translation.LoadData("fr");
        OnLanguageChanged?.Invoke(instance, EventArgs.Empty);

        canvasScaler = canvas.GetComponent<CanvasScaler>();

        // sub to the custom window resized event
        WindowResized.instance.onWindowResized += ResizeCanvas;

        // add the plus tab to the robot list
        Dictionary<int, ListRobot.ListElement> robotElements = new Dictionary<int, ListRobot.ListElement>();
        robotElements.Add(-1, new ListRobot.ListElement()
        {
            isAddRobot = true,
            actionOnClick = () =>
            {
                Robot robot = new Robot(Color.red, "Robot", 1000);
                listRobot.AddChoice(robot.id, robot.ConvertToListElement());
                listRobot.Select(robot.id);
                ChangeRobotSettings();
            }
        });
        listRobot.Init(robotElements, 0);
    }

    private void ResizeCanvas(object sender, WindowResized.WindowResizedEventArgs e)
    {
        // change the canvas resolution to match the screen size
        canvasScaler.referenceResolution = new Vector2(e.screenWidth, e.screenHeight);
        smallRobotViewAspectRation.aspectRatio = (float)e.screenWidth / (float)e.screenHeight;
    }

    private void Update()
    {
        Nodes nodeInfo;
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.I))
        {
            if(Nodes.NodesDict.TryGetValue(selectedNodeId, out nodeInfo))
            {
                ShowInfo(nodeInfo);
            }
        }
    }

    private PopUpNodeInfo ni;
    public void ShowInfo(Nodes node)
    {
        if(ni == null)
        {
            ni = WindowsManager.InstantiateWindow((int)Enum.Parse(typeof(WindowsManager.popUp), "nodeInfo"), instance.canvas.transform).GetComponent<PopUpNodeInfo>();
            ni.init(node.infoTextTitle, node.infoText);
        }else
        {
            ni.Close();
        }
    }

    // called when the speed slider is changed
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
            // instantiate a popup to modify the robot and set all actions of the popup
            PopUpRobot rm = WindowsManager.InstantiateWindow((int)Enum.Parse(typeof(WindowsManager.popUp), "robotModif"), canvas.transform).GetComponent<PopUpRobot>();
            rm.Init(robotToChange.Color, robotToChange.robotName, robotToChange.defaultPower);
            // apply the config to the robot
            rm.SetOkAction(() =>
            {
                robotToChange.Color = rm.robotColor;
                robotToChange.robotName = rm.robotName;
                robotToChange.defaultPower = rm.power;
                listRobot.UpdateButtonColor();
                listRobot.ChangeChoiceColor(robotToChange.id, rm.robotColor);
                rm.PopUpClose();
            });
            // close the popup
            rm.SetCancelAction(() =>
            {
                rm.PopUpClose();
            });
            // delete the robot
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
    [HideInInspector]
    public GameObject spline; // the current spline
    public EventHandler<OnSplineEventArgs> OnSpline; // is used to show the input of all the nodes
    private Action<int> actionWhenConnectionFinished;
    public class OnSplineEventArgs : EventArgs
    {
        public bool splineStarted;
    }
    public Nodes ConnectNode(bool isInput, Transform handleTransform, Nodes sender, Action<int> action, int handleId)
    {
        if(node == null && !isInput)
        {
            // output. Where the spline is created
            actionWhenConnectionFinished = action;
            OnSpline?.Invoke(this, new OnSplineEventArgs() { splineStarted = true });
            instance.spline = Instantiate(SplineObject, new Vector3(0,0,-899), Quaternion.identity, GameObject.FindGameObjectWithTag("NodeHolder").transform);
            instance.spline.GetComponent<SplineManager>().Init(handleTransform, sender, handleId);
            node = sender;
        }
        if (isInput)
        {
            // input. Where the spline ends

            //start tpi
            // check if the node contain the same handle as the one passed in the parameters
            bool canConnect = true;
            foreach (GameObject handle in node.handleEndArray)
            {
                if (handle == sender.handleEndArray[handleId])
                {
                    canConnect = false;
                    break;
                }
            }
            if(canConnect)
            {
                OnSpline?.Invoke(this, new OnSplineEventArgs() { splineStarted = false });
                SplineManager splineManager = instance.spline.GetComponent<SplineManager>();
                splineManager.EndSpline(handleTransform, sender, handleId);
                actionWhenConnectionFinished(sender.id);
                sender.numberOfInputConnection++;
                return node;
            }
            return null;
            //end tpi
        }
        return null;
    }

    public void HideNodes()
    {
        foreach (Transform transform in nodeHolder.transform)
        {
            if(transform.gameObject.tag == "Node")
            {
                transform.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                transform.GetComponent<BoxCollider2D>().enabled = false;
            }
            if (transform.gameObject.tag == "Spline")
                transform.gameObject.SetActive(false);
        }
    }

    public RobotManager CreateRobot(Color color, Robot robot, Vector3 position, Quaternion rotation)
    {
        RobotManager robotManager = Instantiate(robotPrefab, position, rotation).GetComponent<RobotManager>();
        robotManager.robot = robot;
        return robotManager;
    }
    public RobotManager CreateRobot(Color color, Robot robot)
    {
        RobotManager robotManager = Instantiate(robotPrefab, new Vector3(0, 0.5f, 0), Quaternion.identity).GetComponent<RobotManager>();
        robotManager.robot = robot;
        return robotManager;
    }
    public void DestroyObject(GameObject gameObject)
    {
        Destroy(gameObject);
    }


    private static bool quit = false;

    // used as described here https://docs.unity3d.com/ScriptReference/Application-wantsToQuit.html
    [RuntimeInitializeOnLoadMethod]
    static void RunOnStart()
    {
        Application.wantsToQuit += WantsToQuit;
    }
    static bool WantsToQuit()
    {
        if(!quit)
        {
            PopUpWarning sw = WindowsManager.InstantiateWindow((int)Enum.Parse(typeof(WindowsManager.popUp), "saveWarning"), instance.canvas.transform).GetComponent<PopUpWarning>();
            sw.warningText.text = "Des changements ne sont peut-être pas sauvegardés";
            sw.SetCancelAction(() =>
            {
                sw.Close();
                quit = false;
            });
            sw.SetSaveAction(() =>
            {
                SaveManager.instance.Save();
                quit = true;
                sw.Close();
                PopUpWait w = WindowsManager.InstantiateWindow((int)Enum.Parse(typeof(WindowsManager.popUp), "wait"), instance.canvas.transform).GetComponent<PopUpWait>();
                w.init("Quitting...", () =>
                {
                    Application.Quit();
                });
            });
            sw.SetQuitAction(() =>
            {
                quit = true;
                sw.Close();
                PopUpWait w = WindowsManager.InstantiateWindow((int)Enum.Parse(typeof(WindowsManager.popUp), "wait"), instance.canvas.transform).GetComponent<PopUpWait>();
                w.init("Quitting...", () =>
                {
                    Application.Quit();
                });
            });
            return false;
        }
        return true;
    }
}
