// Copyright 2021 Jolan Aklin

//This file is part of Prog The Robot.

//Prog The Robot is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 3 of the License.

//Prog The Robot is distributed in the hope that it will be useful,
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
    public AspectRatioFitter bigRobotViewAspectRation;

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

    [Header("Settings")]
    public bool connectHandleAlwaysShown = true;

    private void Awake()
    {
        instance = this;
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
                // start tpi
                // test if there is room available on the terrain for a new robot
                TerrainManager terrainManager = GameObject.FindGameObjectWithTag("Terrain").GetComponent<TerrainManager>();
                if (terrainManager.TerrainSize[0] * terrainManager.TerrainSize[1] < Robot.robots.Count + 1)
                {
                    PopUpWarning w = PopUpManager.ShowPopUp(PopUpManager.PopUpTypes.saveWarning).GetComponent<PopUpWarning>();
                    w.warningText.text = "Il n'y a plus de place pour un autre robot";
                    w.quitButton.gameObject.SetActive(false);
                    w.saveButton.gameObject.SetActive(false);
                    w.SetCancelAction(() =>
                    {
                        w.Close();
                    });
                    return;
                }
                // end tpi
                Robot robot = new Robot(Color.red, "Robot", 1000);
                listRobot.AddChoice(robot.id, robot.ConvertToListElement());
                listRobot.Select(robot.id);
                ChangeRobotSettings();
            }
        });
        listRobot.Init(robotElements, 0);
    }

    // start tpi

    /// <summary>
    /// Change the behaviour of connect handles
    /// </summary>
    /// <param name="toggle">The toggle that changes this parameter</param>
    public void ChangeConnectHandleDisplayMethod(ToggleScript toggle)
    {
        instance.connectHandleAlwaysShown = toggle.Value;
    }
    // end tpi

    private void ResizeCanvas(object sender, WindowResized.WindowResizedEventArgs e)
    {
        // change the canvas resolution to match the screen size
        smallRobotViewAspectRation.aspectRatio = (float)e.screenWidth / (float)e.screenHeight;
        bigRobotViewAspectRation.aspectRatio = (float)e.screenWidth / (float)e.screenHeight;
    }

    private void Update()
    {
        if(SelectionManager.instance.SelectedNodes.Count > 0)
        {
            Nodes nodeInfo = SelectionManager.instance.SelectedNodes[0];
            if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.I))
            {
                ShowInfo(nodeInfo);
            }

            // start tpi

            // delete
            if(Input.GetKeyDown(KeyCode.Delete))
            {
                SelectionManager.instance.DeleteSelection();
            }

            // copy
            if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C))
            {
                SelectionManager.instance.SelectionToCopyBuffer();
            }

        }

        // paste
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V))
        {
            SelectionManager.instance.PasteCopyBuffer(RobotScript.robotScripts[currentlySelectedScript]);
        }
        // end tpi

    }

    private PopUpNodeInfo ni;
    public void ShowInfo(Nodes node)
    {
        if(ni == null)
        {
            ni = PopUpManager.ShowPopUp(PopUpManager.PopUpTypes.nodeInfo).GetComponent<PopUpNodeInfo>();
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
            PopUpRobot rm = PopUpManager.ShowPopUp(PopUpManager.PopUpTypes.robotModif).GetComponent<PopUpRobot>();
            rm.Init(robotToChange.Color, robotToChange.robotName, robotToChange.defaultPower);
            // apply the config to the robot
            rm.SetOkAction(() =>
            {
                robotToChange.Color = rm.robotColor;
                robotToChange.robotName = rm.robotName;
                robotToChange.defaultPower = rm.power;
                listRobot.UpdateButtonColor();
                listRobot.ChangeChoiceColor(robotToChange.id, rm.robotColor);
                rm.Close();
            });
            // close the popup
            rm.SetCancelAction(() =>
            {
                rm.Close();
            });
            // delete the robot
            rm.SetDeleteAction(() =>
            {
                listRobot.RemoveRobot(robotToChange.id);
                Robot.DeleteRobot(robotToChange.id);
                rm.Close();
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
    private ConnectHandle startSplineHandle;
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
            SplineManager splineManager = instance.spline.GetComponent<SplineManager>();
            splineManager.Init(handleTransform, sender, handleId);
            startSplineHandle = sender.handleStartArray[handleId];
            node = sender;
            sender.currentSplines[handleId] = splineManager;
        }
        if (isInput)
        {
            // input. Where the spline ends

            //start tpi
            // check if the node contain the same handle as the one passed in the parameters. Return null if the node can't connect
            bool canConnect = true;
            SplineManager splineManager = instance.spline.GetComponent<SplineManager>();
            foreach (ConnectHandle handle in splineManager.NodeStart.handleEndArray)
            {
                if (handle == sender.handleEndArray[handleId])
                {
                    canConnect = false;
                    break;
                }
            }
            if(canConnect)
            {
                if (startSplineHandle.loopArea != sender.handleEndArray[handleId].loopArea)
                    canConnect = false;
            }
            if(canConnect)
            {
                OnSpline?.Invoke(this, new OnSplineEventArgs() { splineStarted = false });
                splineManager.EndSpline(handleTransform, sender, handleId);
                actionWhenConnectionFinished(sender.id);
                sender.AddConnection();
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
        Vector3 spawnPos = GetNextPlaceForRobot();
        RobotManager robotManager = Instantiate(robotPrefab, spawnPos, Quaternion.identity).GetComponent<RobotManager>();
        robotManager.robot = robot;
        return robotManager;
    }

    // start tpi

    /// <summary>
    /// Get the next available place for a robot
    /// </summary>
    /// <returns>The pos on the grid without a robot on it</returns>
    public Vector3 GetNextPlaceForRobot()
    {
        TerrainManager terrain = GameObject.FindGameObjectWithTag("Terrain").GetComponent<TerrainManager>();
        Vector3 pos = Vector3.zero;
        bool foundPos = true;
        for (int j = 0; j < terrain.TerrainSize[0]; j++)
        {
            for (int i = 0; i < terrain.TerrainSize[1]; i++)
            {
                foundPos = true;
                pos = new Vector3(j, 0.5f, i);

                foreach (Robot robot in Robot.robots.Values)
                {
                    if (robot.robotManager.PosOnGridInt == new Vector3(pos.x, 0, pos.z))
                    {
                        foundPos = false;
                        break;
                    }
                }

                if (foundPos)
                    break;
            }
            if (foundPos)
                break;
        }
        return pos;
    }
    //end tpi

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
            PopUpWarning sw = PopUpManager.ShowPopUp(PopUpManager.PopUpTypes.saveWarning).GetComponent<PopUpWarning>();
            sw.warningText.text = "Des changements ne sont peut-?tre pas sauvegard?s";
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
                PopUpWait w = PopUpManager.ShowPopUp(PopUpManager.PopUpTypes.wait).GetComponent<PopUpWait>();
                w.init("Quitting...", () =>
                {
                    Application.Quit();
                });
            });
            sw.SetQuitAction(() =>
            {
                quit = true;
                sw.Close();
                PopUpWait w = PopUpManager.ShowPopUp(PopUpManager.PopUpTypes.wait).GetComponent<PopUpWait>();
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
