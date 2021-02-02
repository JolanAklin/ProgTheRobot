using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIRaycaster : MonoBehaviour
{
    public GameObject addNodeMenuInstance;
    private bool panelOpen;
    private GameObject addNodeMenu;

    // raycast stuff
    GraphicRaycaster graphciraycaster;
    PointerEventData pointerevent;
    EventSystem eventsystem;
    List<RaycastResult> rayCastResults = new List<RaycastResult>();

    void Start()
    {
        graphciraycaster = GetComponent<GraphicRaycaster>();
        eventsystem = GetComponent<EventSystem>();

        panelOpen = false;
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            pointerevent = new PointerEventData(eventsystem);
            pointerevent.position = Input.mousePosition;
            rayCastResults = new List<RaycastResult>();
            graphciraycaster.Raycast(pointerevent, rayCastResults);

            int i = 0;
            foreach (RaycastResult raycastResult in rayCastResults)
            {
                if(raycastResult.gameObject.tag == "ScriptPanel" && i == 0 && !panelOpen)
                {
                    addNodeMenu = Instantiate(addNodeMenuInstance, pointerevent.position, Quaternion.identity, transform);
                    addNodeMenu.tag = "MenuAddScript";
                    panelOpen = true;
                    break;
                }
                i++;
            }
        }
        if (Input.GetMouseButton(0))
        {
            bool foundMenuAddScript = true;
            pointerevent = new PointerEventData(eventsystem);
            pointerevent.position = Input.mousePosition;
            rayCastResults = new List<RaycastResult>();
            graphciraycaster.Raycast(pointerevent, rayCastResults);

            foreach (RaycastResult raycastResult in rayCastResults)
            {
                if (raycastResult.gameObject.tag == "MenuAddScript")
                {
                    foundMenuAddScript = false;
                }
            }
            if (foundMenuAddScript)
            {
                panelOpen = false;
                Destroy(addNodeMenu);
            }
        }
    }
}
