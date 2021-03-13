using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectHandle : MonoBehaviour
{
    public Nodes node; // which is his parent
    public bool isInput = false;
    private bool canBeClicked = true;
    private Image image;
    private BoxCollider2D boxCollider2d;

    // used when the node is a loop or a if node
    public bool ifFalse;
    public bool inLoopOut = false;

    public int handleNumber;
    
    private void Start()
    {
        canBeClicked = true;
        image = GetComponent<Image>();
        boxCollider2d = GetComponent<BoxCollider2D>();
        if (isInput)
        {
            Manager.instance.OnSpline += ShowHide; // this event while be triggered when a spline is created or ended
        }
        if (image != null && boxCollider2d != null && isInput)
        {
            image.enabled = false;
            boxCollider2d.enabled = false;
        }
    }

    private void OnDestroy()
    {
        if (isInput)
            Manager.instance.OnSpline -= ShowHide;
    }

    // when the handle is clicked, will ask the manager.
    public void Click()
    {
        Nodes nextNode = null;
        if(!isInput)
        {
            if(canBeClicked)
            {
                // call a function in the manager script and give a delegate to perform the right action to set the node id
                if (ifFalse)
                    nextNode = Manager.instance.ConnectNode(isInput, transform, node, (id) => { node.gameObject.GetComponent<NodeIf>().nextNodeIdFalse = id; }, handleNumber) ;
                else if(inLoopOut)
                {
                    NodeForLoop nodeForLoop;
                    if(node.gameObject.TryGetComponent(out nodeForLoop))
                        nextNode = Manager.instance.ConnectNode(isInput, transform, node, (id) => { nodeForLoop.nextNodeInside = id; }, handleNumber);
                    NodeWhileLoop nodeWhileLoop;
                    if (node.gameObject.TryGetComponent(out nodeWhileLoop))
                        nextNode = Manager.instance.ConnectNode(isInput, transform, node, (id) => { nodeWhileLoop.nextNodeInside = id; }, handleNumber);
                }
                else
                    nextNode = Manager.instance.ConnectNode(isInput, transform, node, (id) => { node.nextNodeId = id; }, handleNumber);

                canBeClicked = false;
            }
        }else
        {
            nextNode = Manager.instance.ConnectNode(isInput, transform, node, (id) => {}, handleNumber);
        }

        if (nextNode != null)
        {
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
        {
            image.enabled = true;
            boxCollider2d.enabled = true;
        }
        else
        {
            image.enabled = false;
            boxCollider2d.enabled = false;
        }
    }
}
