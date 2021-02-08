using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIRaycaster : MonoBehaviour
{
    public GameObject addNodeMenuInstance;
    [HideInInspector]
    public bool panelOpen;
    private GameObject addNodeMenu;

    // raycast stuff
    GraphicRaycaster graphicraycaster;
    PointerEventData pointerevent;
    EventSystem eventsystem;
    List<RaycastResult> rayCastResults = new List<RaycastResult>();

    // resize node
    private ResizeHandle resizeHandle;
    //move node
    private Nodes nodeToMove;

    void Start()
    {
        graphicraycaster = GetComponent<GraphicRaycaster>();
        eventsystem = GetComponent<EventSystem>();

        panelOpen = false;
    }

    void Update()
    {
        #region On ui element
        pointerevent = new PointerEventData(eventsystem);
        pointerevent.position = Input.mousePosition;
        rayCastResults = new List<RaycastResult>();
        graphicraycaster.Raycast(pointerevent, rayCastResults);
        if (Input.GetMouseButtonDown(1) && !panelOpen && rayCastResults.Count == 0)
        {
            addNodeMenu = Instantiate(addNodeMenuInstance, Input.mousePosition, Quaternion.identity, transform);
            addNodeMenu.tag = "MenuAddScript";
            panelOpen = true;
        }
        if(Input.GetMouseButton(0) && panelOpen && rayCastResults.Find(X => X.gameObject.tag == "MenuAddScript").gameObject == null)
        {
            if(addNodeMenu != null)
            {
                Destroy(addNodeMenu);
                panelOpen = false;
            }
        }
        #endregion

        #region On nodes
        if(Input.GetMouseButtonUp(0))
        {
            //end resize
            if(resizeHandle != null && resizeHandle.node.canResize)
            {
                resizeHandle.node.gameObject.transform.position = new Vector3(resizeHandle.node.gameObject.transform.position.x, resizeHandle.node.transform.position.y, 0f);
                resizeHandle.NodeResize();
                resizeHandle = null;
            }

            //end move
            if (nodeToMove != null && nodeToMove.canMove)
            {
                nodeToMove.gameObject.transform.position = new Vector3(nodeToMove.gameObject.transform.position.x, nodeToMove.transform.position.y, 0f);
                nodeToMove.StartEndMove();
                nodeToMove = null;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            // start resize
            if (rayCastResults.Count == 0 && resizeHandle == null)
            {
                RaycastHit2D hit;
                Ray ray = NodeDisplay.instance.nodeCamera.ScreenPointToRay(Input.mousePosition);
                if (hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity))
                {
                    if(hit.collider.gameObject.tag == "ResizeHandle")
                    {
                        resizeHandle = hit.collider.GetComponent<ResizeHandle>();
                        resizeHandle.NodeResize();
                    }
                }
            }

            //move node
            if (rayCastResults.Count == 0 && nodeToMove == null)
            {
                RaycastHit2D hit;
                Ray ray = NodeDisplay.instance.nodeCamera.ScreenPointToRay(Input.mousePosition);
                if (hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity))
                {
                    if (hit.collider.gameObject.tag == "Node")
                    {
                        nodeToMove = hit.collider.GetComponent<Nodes>();
                        nodeToMove.StartEndMove();
                    }
                }
            }
        }

        if(Input.GetMouseButtonDown(0))
        {
            if (rayCastResults.Count == 0 && nodeToMove == null)
            {
                RaycastHit2D hit;
                Ray ray = NodeDisplay.instance.nodeCamera.ScreenPointToRay(Input.mousePosition);
                if (hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity))
                {
                    if (hit.collider.gameObject.tag == "ConnectHandle")
                    {
                        ConnectHandle connect = hit.collider.gameObject.GetComponent<ConnectHandle>();
                        connect.Click();
                    }
                }
            }
        }
        #endregion
    }
}
