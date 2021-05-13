using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class NodeInfo : MonoBehaviour
{
    // start tpi

    [Header("Node infos")]
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

    /// <summary>
    /// Infos for the specified node.
    /// </summary>
    [Serializable]
    public class Info
    {
        /// <summary>
        /// the type of node targeted by this info
        /// </summary>
        public Nodes.NodeTypes nodeTypes;
        /// <summary>
        /// Name of the node
        /// </summary>
        public string infoTextTitle;
        /// <summary>
        /// Info on the node
        /// </summary>
        [TextArea]
        public string infoText;
    }
    //end tpi
}
