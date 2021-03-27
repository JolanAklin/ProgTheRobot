// Copyright 2021 Jolan Aklin

//This file is part of Prog the robot.

//Prog the robot is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 3 of the License.

//Prog the robot is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Prog the robot.  If not, see<https://www.gnu.org/licenses/>.

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
    public Nodes nodeToMove;
    private Vector3 beginMoveMousePos;

    // move node camera
    private Vector3 mousePosBeginMove;
    private Vector3 cameraPosAtPanStart;
    private bool cameraCanBePanned;
    public float nodeAreaPanSensitivity;

    // info bar
    public GameObject infoBar;

    private Nodes selectedNode = null;

    public static UIRaycaster instance;


    void Start()
    {
        graphicraycaster = GetComponent<GraphicRaycaster>();
        eventsystem = GetComponent<EventSystem>();

        panelOpen = false;

        instance = this;
    }

    void Update()
    {
        // do only raycast on UI elements
        #region On ui element
        pointerevent = new PointerEventData(eventsystem);
        pointerevent.position = Input.mousePosition;
        rayCastResults = new List<RaycastResult>();
        graphicraycaster.Raycast(pointerevent, rayCastResults);

        if (Input.GetMouseButtonDown(1) && !panelOpen && rayCastResults.Count == 0 && nodeToMove == null)
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
        GameObject resizePanel = null;
        if (rayCastResults.Count > 0)
        {
            resizePanel = rayCastResults[0].gameObject;
            if(resizePanel != null)
            {
                if(resizePanel.tag != "ResizePanel")
                {
                    resizePanel = null;
                }
            }
        }

        if(resizePanel != null)
        {
            CursorManager.instance.ChangeCursor(resizePanel.GetComponent<ResizePanel>().cursorType);
        }else
        {
            CursorManager.instance.ChangeCursor("default");
        }

        if (Input.GetMouseButtonDown(0) && !panelOpen && resizePanel != null)
        {
            resizePanel.GetComponent<ResizePanel>().StartMove();
        }

        if(rayCastResults.Count == 0)
        {
            infoBar.GetComponent<InfoBar>().ChangeInfos("ScriptInfo");
        }else if(rayCastResults.Find(X => X.gameObject.tag == "RobotSmallWindow").gameObject != null)
        {
            infoBar.GetComponent<InfoBar>().ChangeInfos("RobotSmallWindow");
            CameraController.instance.SetCanMove(true);
        }else if (rayCastResults.Find(X => X.gameObject.tag == "RobotBigWindow").gameObject != null)
        {
            CameraController.instance.SetCanMove(true);
        }
        else
        {
            CameraController.instance.SetCanMove(false);

        }
        #endregion

        // only do raycast in the 3D/2D world
        #region On nodes
        if (Input.GetMouseButtonUp(0))
        {
            //end resize
            if(resizeHandle != null && resizeHandle.node.canResize)
            {
                //resizeHandle.node.gameObject.transform.position = new Vector3(resizeHandle.node.gameObject.transform.position.x, resizeHandle.node.transform.position.y, );
                resizeHandle.NodeResize();
                resizeHandle = null;
            }

            //end move
            if (nodeToMove != null && nodeToMove.canMove)
            {
                //nodeToMove.gameObject.transform.position = new Vector3(nodeToMove.gameObject.transform.position.x, nodeToMove.transform.position.y, 0);
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
                    if (hit.collider.gameObject.tag == "ResizeHandle")
                    {
                        resizeHandle = hit.collider.GetComponent<ResizeHandle>();
                        resizeHandle.NodeResize();
                    }
                }
            }

            //move node
            if (nodeToMove == null)
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
            if (rayCastResults.Count == 0)
            {
                RaycastHit2D hit;
                Ray ray = NodeDisplay.instance.nodeCamera.ScreenPointToRay(Input.mousePosition);
                if (hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity))
                {
                    if (hit.collider.gameObject.tag == "Node")
                    {
                        if (selectedNode != null)
                        {
                            selectedNode.ChangeBorderColor(selectedNode.defaultColor);
                        }
                        selectedNode = hit.collider.GetComponent<Nodes>();
                        Manager.instance.selectedNodeId = selectedNode.id;
                        selectedNode.ChangeBorderColor(selectedNode.selectedColor);
                    }
                    else
                    {
                        Manager.instance.selectedNodeId = -1;
                        if (selectedNode != null)
                        {
                            selectedNode.ChangeBorderColor(selectedNode.defaultColor);
                            selectedNode = null;
                        }
                    }
                }else
                {
                    Manager.instance.selectedNodeId = -1;
                    if (selectedNode != null)
                    {
                        selectedNode.ChangeBorderColor(selectedNode.defaultColor);
                        selectedNode = null;
                    }
                }
            }else
            {
                Manager.instance.selectedNodeId = -1;
                if(selectedNode != null)
                {
                    selectedNode.ChangeBorderColor(selectedNode.defaultColor);
                    selectedNode = null;
                }
            }
        }
        // only start moving a node if the cursor was moved more than 1 unit
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
                CursorManager.instance.ChangeCursor("move", true);
            }

            Vector3 currentMousePos = Input.mousePosition;
            Vector3 delta = (currentMousePos - mousePosBeginMove);
            Vector3 cameraPos = new Vector3(-delta.x/nodeAreaPanSensitivity, -delta.y/nodeAreaPanSensitivity, 0);
            NodeDisplay.instance.nodeCamera.transform.position = cameraPos + cameraPosAtPanStart;
        }
        else if(cameraCanBePanned == true)
        {
            cameraCanBePanned = false;
            CursorManager.instance.UnLockCursorTexture();
            CursorManager.instance.ChangeCursor("default");
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
