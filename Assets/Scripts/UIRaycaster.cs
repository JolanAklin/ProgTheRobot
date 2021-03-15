using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// all raycast stuff goes in there
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
    private Vector3 beginMoveMousePos;

    // move node camera
    private Vector3 mousePosBeginMove;
    private Vector3 cameraPosAtPanStart;
    private bool cameraCanBePanned;
    public float nodeAreaPanSensitivity;

    void Start()
    {
        graphicraycaster = GetComponent<GraphicRaycaster>();
        eventsystem = GetComponent<EventSystem>();

        panelOpen = false;
    }

    void Update()
    {
        // do only raycast on UI elements
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

        // only do raycast in the 3D/2D world
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
                nodeToMove.EndMove();
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
                        beginMoveMousePos = NodeDisplay.instance.nodeCamera.ScreenToWorldPoint(Input.mousePosition);
                        nodeToMove = hit.collider.GetComponent<Nodes>();
                    }
                }
            }
        }
        // only start moving a node if the cursor was moved pas 1 unit
        if(nodeToMove != null)
        {
            if(!nodeToMove.isMoving)
            {
                if((NodeDisplay.instance.nodeCamera.ScreenToWorldPoint(Input.mousePosition) - beginMoveMousePos).sqrMagnitude > 1 && nodeToMove != null)
                {
                    nodeToMove.StartMove();
                }
            }
        }

        if(Input.GetMouseButtonDown(0))
        {
            // move spline
            if (rayCastResults.Count == 0 && nodeToMove == null)
            {
                RaycastHit2D hit;
                Ray ray = NodeDisplay.instance.nodeCamera.ScreenPointToRay(Input.mousePosition);
                if (hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity))
                {
                    if (hit.collider.gameObject.tag == "SplineMoveHandle")
                    {
                        hit.collider.GetComponent<MoveLinkHandle>().SplineLink.GetComponent<SplineManager>().MoveSpline();
                    }else if (hit.collider.gameObject.tag == "ConnectHandle")
                    {
                        ConnectHandle connect = hit.collider.gameObject.GetComponent<ConnectHandle>();
                        connect.Click();
                    }
                }
            }
        }


        // script panel panning
        if (Input.GetMouseButton(2) && rayCastResults.Count == 0)
        {
            if(!cameraCanBePanned)
            {
                mousePosBeginMove = Input.mousePosition;
                cameraCanBePanned = true;
                cameraPosAtPanStart = NodeDisplay.instance.nodeCamera.transform.position;
            }

            Vector3 currentMousePos = Input.mousePosition;
            Vector3 delta = (currentMousePos - mousePosBeginMove);
            Vector3 cameraPos = new Vector3(-delta.x/nodeAreaPanSensitivity, -delta.y/nodeAreaPanSensitivity, -10);
            NodeDisplay.instance.nodeCamera.transform.position = cameraPos + cameraPosAtPanStart;
        }
        else if(cameraCanBePanned == true)
        {
            cameraCanBePanned = false;
        }

        // script panel zoom
        if(Input.GetKey(KeyCode.LeftControl) && rayCastResults.Count == 0)
        {
            float zoom = NodeDisplay.instance.nodeCamera.orthographicSize - Input.mouseScrollDelta.y;
            if (zoom < 1)
                zoom = 1f;
            NodeDisplay.instance.nodeCamera.orthographicSize = zoom;
        }
        #endregion
    }
}
