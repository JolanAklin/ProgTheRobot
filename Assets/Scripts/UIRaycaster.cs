// Copyright 2021 Jolan Aklin

//This file is part of Prog The Robot.

//Prog The Robot is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 3 of the License.

//Prog The Robot is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Prog the robot.  If not, see<https://www.gnu.org/licenses/>.

using System;
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
    public bool nodeContextMenuOpen = false;
    private GameObject addNodeMenu;
    public GameObject nodeContextMenu;

    // raycast stuff
    GraphicRaycaster graphicraycaster;
    PointerEventData pointerevent;
    EventSystem eventsystem;
    List<RaycastResult> rayCastResults = new List<RaycastResult>();

    // resize node
    private ResizeHandle resizeHandle;
    //move node
    //public Nodes nodeToMove;

    // move node camera
    private Vector3 mousePosBeginMove;
    private Vector3 cameraPosAtPanStart;
    private bool cameraCanBePanned;
    public float nodeAreaPanSensitivity;

    // info bar
    public GameObject infoBar;

    private GameObject nodeContextMenuInstance;


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

        if(rayCastResults.Count == 0)
        {
            infoBar.GetComponent<InfoBar>().ChangeInfos("ScriptInfo");
        }else if(rayCastResults.Find(X => X.gameObject.tag == "RobotSmallWindow").gameObject != null)
        {
            infoBar.GetComponent<InfoBar>().ChangeInfos("RobotSmallWindow");
            //start tpi
            Manager.instance.canTerrainCamMove = true;
            // end tpi
        }else if (rayCastResults.Find(X => X.gameObject.tag == "RobotBigWindow").gameObject != null)
        {
            //start tpi
            Manager.instance.canTerrainCamMove = true;
            // end tpi
        }
        else
        {
            //start tpi
            Manager.instance.canTerrainCamMove = false;
            // end tpi
        }
        #endregion

        // only do raycast in the 3D/2D world
        #region On nodes

        if(Input.GetMouseButtonDown(0))
        {
            // move spline
            if (rayCastResults.Count == 0 && SelectionManager.instance.SelectedNodes.Count == 0)
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
                CursorManager.instance.ChangeCursor(CursorManager.CursorDef.CursorTypes.move, true);
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
            CursorManager.instance.ChangeCursor(CursorManager.CursorDef.CursorTypes.arrow);
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

    public void MoveNode()
    {
        foreach (Nodes selectedNode in SelectionManager.instance.SelectedNodes)
        {
            if (selectedNode != null)
            {
                selectedNode.StartMove();
            }
        }
    }
}
