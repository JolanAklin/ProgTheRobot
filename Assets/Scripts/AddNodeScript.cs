using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AddNodeScript : MonoBehaviour
{
    private Dictionary<int, Action> nodes = new Dictionary<int, Action>();
    private Transform scriptPanel;
    public List<nodeObject> nodeObjects = new List<nodeObject>();

    [Serializable]
    public class nodeObject
    {
        public string nodeType;
        public GameObject gameObject;
    }


    private void Start()
    {
        scriptPanel = GameObject.FindGameObjectWithTag("NodeHolder").transform;

        foreach (Nodes.NodeTypes nodeType in (Nodes.NodeTypes[])Enum.GetValues(typeof(Nodes.NodeTypes)))
        {
            AddAction(nodeType.ToString(), () =>
            {
                Vector3 spawnPos = Round(NodeDisplay.instance.nodeCamera.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono),1);
                spawnPos.z = 0;
                Instantiate(nodeObjects.Find(x => x.nodeType == nodeType.ToString()).gameObject, spawnPos, Quaternion.identity, scriptPanel);
                GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
                canvas.GetComponent<UIRaycaster>().panelOpen = false;
                Destroy(this.gameObject);
            });
        }
    }

    public void AddAction(string nodeTypeName, Action action)
    {
        int nodeType = (int)Enum.Parse(typeof(Nodes.NodeTypes), nodeTypeName);
        if (nodes.ContainsKey(nodeType))
        {
            nodes[nodeType] = action;
        }else
        {
            nodes.Add(nodeType, action);
        }
    }

    public void DoAction(string nodeType)
    {
        nodes[(int)Enum.Parse(typeof(Nodes.NodeTypes), nodeType)]();
    }

    private Vector3 Round(Vector3 vector3, int round)
    {
        return new Vector3((float)Math.Round(vector3.x, round), (float)Math.Round(vector3.y,round), (float)Math.Round(vector3.z,round));
    }
}
