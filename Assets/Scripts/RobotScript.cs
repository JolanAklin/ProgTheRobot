using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// will handle the loading of nodes
public class RobotScript
{
    public static int nextid = 0;
    public static Dictionary<int, RobotScript> robotScripts = new Dictionary<int, RobotScript>();
    public int id;
    public string name;
    public Robot robot;

    public Action endCallBack;

    public Nodes nodeStart;

    public event EventHandler onStop;

    public List<GameObject> nodes = new List<GameObject>();
    public List<GameObject> splines = new List<GameObject>();

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
        Manager.instance.HideNodes();
        Manager.instance.currentlySelectedScript = this.id;
        foreach (GameObject node in nodes)
        {
            node.transform.GetChild(0).gameObject.SetActive(true);
        }
        foreach (GameObject spline in splines)
        {
            spline.SetActive(true);
        }
        Debug.Log($"Loaded Script {id}");
    }

    public void End()
    {
        endCallBack?.Invoke();
        onStop?.Invoke(this, EventArgs.Empty);
    }

    ~RobotScript()
    {
        Debug.Log($"Script {name} was destroyed");
    }

    public void Clear()
    {
        robot = null;
        nodeStart = null;
        nodes.Clear();
        splines.Clear();
    }

    #region save stuff
    [Serializable]
    public class SerializedRobotScript
    {
        public int id;
        public string name;
        public int robotId;
        [SerializeField]
        public List<Nodes.SerializableNode> serializedNode = new List<NodeEnd.SerializableNode>();
    }

    // convert this class to json
    public SerializedRobotScript SerializeScript()
    {
        // add all the nodes inside of the script
        List<Nodes.SerializableNode> serializedNode = new List<Nodes.SerializableNode>();
        foreach (GameObject node in nodes)
        {
            Nodes nodeScript = node.GetComponent<Nodes>();
            serializedNode.Add(nodeScript.SerializeNode());
        }
        SerializedRobotScript serializedRobotScript = new SerializedRobotScript() { id = id, name = name, robotId = robot.id, serializedNode = serializedNode};
        return serializedRobotScript;
    }

    // convert json to this class
    public void DeSerializeScript()
    {

    }
    #endregion
}
