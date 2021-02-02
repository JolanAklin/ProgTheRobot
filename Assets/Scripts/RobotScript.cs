using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotScript
{

    public static int nextid = 0;
    public static Dictionary<int, RobotScript> robotScripts = new Dictionary<int, RobotScript>();
    public int id;
    public string name;

    public List<Nodes> nodes = new List<Nodes>();

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
        Debug.Log($"Loaded Script {id}");
    }
}
