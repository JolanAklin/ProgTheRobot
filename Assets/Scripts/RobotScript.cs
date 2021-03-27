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

    public RobotScript(SerializedRobotScript serializedRobotScript, SaveManager saveManager, bool isMain)
    {
        DeSerializeScript(serializedRobotScript, saveManager, isMain);
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
        HideNodes();
        Manager.instance.currentlySelectedScript = this.id;
        foreach (GameObject node in nodes)
        {
            node.transform.GetChild(0).gameObject.SetActive(true);
            foreach (BoxCollider2D collider2D in node.transform.GetComponents<BoxCollider2D>())
            {
                collider2D.enabled = true;
            }
        }
        foreach (GameObject spline in splines)
        {
            if(spline != null)
            {
                spline.SetActive(true);
            }
        }
        Debug.Log($"Loaded Script {id}");
    }

    public void HideNodesForThisScript()
    {
        foreach (GameObject node in nodes)
        {
            node.transform.GetChild(0).gameObject.SetActive(false);
            foreach (BoxCollider2D collider2D in node.transform.GetComponents<BoxCollider2D>())
            {
                collider2D.enabled = false;
            }
        }
        foreach (GameObject spline in splines)
        {
            if (spline != null)
            {
                spline.SetActive(false);
            }
        }
    }

    public void HideNodes()
    {
        if(robotScripts.ContainsKey(Manager.instance.currentlySelectedScript))
        {
            foreach (GameObject node in robotScripts[Manager.instance.currentlySelectedScript].nodes)
            {
                node.transform.GetChild(0).gameObject.SetActive(false);
                foreach (BoxCollider2D collider2D in node.transform.GetComponents<BoxCollider2D>())
                {
                    collider2D.enabled = false;
                }
            }
            foreach (GameObject spline in robotScripts[Manager.instance.currentlySelectedScript].splines)
            {
                if(spline != null)
                {
                    spline.SetActive(false);
                }
            }
        }
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
    public void DeSerializeScript(SerializedRobotScript serializedRobotScript, SaveManager saveManager, bool isMain)
    {
        id = serializedRobotScript.id;
        name = serializedRobotScript.name;
        robot = Robot.robots[serializedRobotScript.robotId];
        robotScripts.Add(id, this);
        robot.robotScripts.Add(this);
        if (isMain)
            robot.MainScript = this;

        Transform nodeHolder = GameObject.FindGameObjectWithTag("NodeHolder").transform;
        foreach (Nodes.SerializableNode serializableNode in serializedRobotScript.serializedNode)
        {
            GameObject node = saveManager.nodeObjects.Find(x => x.nodeType == serializableNode.type.ToString()).gameObject; // find the correct gameobject to instantiate
            GameObject nodeInstance = saveManager.InstantiateSavedObj(node, new Vector3(serializableNode.position[0], serializableNode.position[1], serializableNode.position[2]), Quaternion.identity, nodeHolder);

            nodes.Add(nodeInstance);
            Nodes nodeInstanceScript = nodeInstance.GetComponent<Nodes>();
            nodeInstanceScript.rs = this;

            nodeInstanceScript.DeSerializeNode(serializableNode);

            if (serializableNode.type == "start")
            {
                nodeStart = nodeInstance.GetComponent<Nodes>();
            }
        }
    }
    #endregion
}
