using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowNodeInfo : MonoBehaviour
{
    // start tpi
    public Nodes.NodeTypes nodeType;
    private RectTransform rect;

    private NodeInfo.Info nodeInfo;


    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        nodeInfo = NodeInfo.nodesInfos.Find(x => x.nodeTypes == nodeType);
    }

    private void Update()
    {
        if(RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition))
        {
            NodeInfo.infoTitle.text = nodeInfo.infoTextTitle;
            NodeInfo.infoDesc.text = nodeInfo.infoText;
        }
    }
    //end tpi
}
