using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotScript
{
    public static int nextid = 0;
    public static Dictionary<int, RobotScript> robotScripts = new Dictionary<int, RobotScript>();
    public int id;
    public string name;

    public List<GameObject> nodes = new List<GameObject>();

    public RobotScript(string name)
    {
        this.name = name;
        Init();
    }

    private void Init()
    {
        // All robotscripts have a different id
        id = nextid;
        nextid++;
        robotScripts.Add(id, this);
    }

    public void SerializeScript()
    {

    }

    public void DeSerializeScript()
    {

    }

    public List.ListElement ConvertToListElement()
    {
        return new List.ListElement { isAddScript = false, displayedText = name, actionOnClick = () => { LoadNodes(); } };
    }

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
            Manager.instance.CreateNodeObject(node);
        }
        Debug.Log($"Loaded Script {id}");
    }
}
