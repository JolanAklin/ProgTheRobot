using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class NodeInfo : MonoBehaviour
{
    // start tpi
    public List<Info> infos = new List<Info>();
    public static List<Info> nodesInfos = new List<Info>();

    [Header("Text boxes")]
    public TMP_Text infotitle;
    public TMP_Text infodesc;


    public static TMP_Text infoTitle;
    public static TMP_Text infoDesc;


    private void Awake()
    {
        nodesInfos = infos;
        infoTitle = infotitle;
        infoDesc = infodesc;
    }

    [Serializable]
    public class Info
    {
        public Nodes.NodeTypes nodeTypes;
        public string infoTextTitle;
        [TextArea]
        public string infoText;
    }
    //end tpi
}
