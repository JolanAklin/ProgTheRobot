using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// will handle the loading of nodes
public class RobotScript
{
    public static int nextid = 0;
    public static Dictionary<int, RobotScript> robotScripts = new Dictionary<int, RobotScript>();
    public int id;
    public string name;
    public Robot robot;

    public bool hasAStartNode = false;
    public Nodes nodeStart;

    public List<GameObject> nodes = new List<GameObject>();

    public RobotScript(string name, Robot robot)
    {
        this.name = name;
        Init(robot);
    }

    private void Init(Robot robot)
    {
        // All robotscripts have a different id
        id = nextid;
        nextid++;
        this.robot = robot;
        robotScripts.Add(id, this);
    }

    // convert this class to json
    public void SerializeScript()
    {

    }

    // convert json to this class
    public void DeSerializeScript()
    {

    }

    // convert node script to displayable element in list
    public List.ListElement ConvertToListElement()
    {
        return new List.ListElement { isAddScript = false, displayedText = name, actionOnClick = () => { LoadNodes(); } };
    }

    // show the node from json when finished
    public void LoadNodes()
    {
        // need to update when there is json
        //if(Manager.instance.currentlySelectedScript != -1)
        //{
        //    RobotScript oldSelectedScript = robotScripts[Manager.instance.currentlySelectedScript];
        //    oldSelectedScript.nodes.Clear();
        //    oldSelectedScript.nodes = Manager.instance.DeleteNodes();
        //}
        Manager.instance.DeleteNodes();
        Manager.instance.currentlySelectedScript = this.id;
        foreach (GameObject node in nodes)
        {
            Manager.instance.CreateNodeObject(node, this);
        }
        Debug.Log($"Loaded Script {id}");
    }
}
