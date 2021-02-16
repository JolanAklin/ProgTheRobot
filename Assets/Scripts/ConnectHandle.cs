using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectHandle : MonoBehaviour
{
    public Nodes node;
    public bool isInput = false;
    private Image image;

    public bool ifFalse;

    private void Start()
    {
        image = GetComponent<Image>();
        if (isInput)
            Manager.instance.OnSpline += ShowHide;
        if (image != null && isInput)
            image.enabled = false;
    }

    private void OnDestroy()
    {
        if (isInput)
            Manager.instance.OnSpline -= ShowHide;
    }

    // when the handle is clicked, will ask the manager.
    public void Click()
    {
        Nodes nextNode;
        if (ifFalse)
            nextNode = Manager.instance.ConnectNode(isInput, transform, node, ref node.gameObject.GetComponent<NodeIf>().nextNodeIdFalse);
        else
            nextNode = Manager.instance.ConnectNode(isInput, transform, node, ref node.nextNodeId);

        if (nextNode != null)
        {
            //node.nextNodeId = nextNode.id;
            //node.nextGameObject = nextNode.gameObject;
            nextNode = null;
            Manager.instance.node = null;
        }
        if (!isInput)
        {
            image.enabled = false;
        }
    }

    // show and hide the input image (red dot)
    public void ShowHide(object sender, Manager.OnSplineEventArgs e)
    {
        if (e.splineStarted)
            image.enabled = true;
        else
            image.enabled = false;
    }
}
