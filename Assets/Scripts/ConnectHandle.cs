using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectHandle : MonoBehaviour
{
    public Nodes node;
    public bool isInput = false;
    public Image inputImage;

    private void Start()
    {
        if (isInput)
            Manager.instance.OnSpline += ShowHide;
        if (inputImage != null)
            inputImage.enabled = false;
    }

    // when the handle is clicked, will ask the manager.
    public void Click()
    {
        Nodes nextNode = Manager.instance.ConnectNode(isInput, transform, node);
        if(nextNode != null)
        {
            node.nextNodeId = nextNode.id;
            node.nextGameObject = nextNode.gameObject;
            nextNode = null;
            Manager.instance.node = null;
        }
    }

    // show and hide the input image (red dot)
    public void ShowHide(object sender, Manager.OnSplineEventArgs e)
    {
        if (e.splineStarted)
            inputImage.enabled = true;
        else
            inputImage.enabled = false;
    }
}
