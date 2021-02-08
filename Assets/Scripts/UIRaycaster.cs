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
    GraphicRaycaster graphciraycaster;
    PointerEventData pointerevent;
    EventSystem eventsystem;
    List<RaycastResult> rayCastResults = new List<RaycastResult>();

    // resize node
    private ResizeHandle resizeHandle;
    private bool endResize = false;

    void Start()
    {
        graphciraycaster = GetComponent<GraphicRaycaster>();
        eventsystem = GetComponent<EventSystem>();

        panelOpen = false;
    }

    void Update()
    {
        #region On ui element
        pointerevent = new PointerEventData(eventsystem);
        pointerevent.position = Input.mousePosition;
        rayCastResults = new List<RaycastResult>();
        graphciraycaster.Raycast(pointerevent, rayCastResults);
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
            if(resizeHandle != null && resizeHandle.node.canResize)
            {
                resizeHandle.NodeResize();
                resizeHandle = null;
                endResize = true;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (rayCastResults.Count == 0 && resizeHandle == null && !endResize)
            {
                if(resizeHandle == null)
                {
                    RaycastHit2D hit;
                    Ray ray = NodeDisplay.instance.nodeCamera.ScreenPointToRay(Input.mousePosition);
                    // Does the ray intersect any objects excluding the player layer
                    if (hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity))
                    {
                        if(hit.collider.gameObject.tag == "ResizeHandle")
                        {
                            resizeHandle = hit.collider.GetComponent<ResizeHandle>();
                            resizeHandle.NodeResize();
                        }
                    }
                }
            }
            endResize = false;

        }
        #endregion
    }
}
