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

    void Start()
    {
        graphciraycaster = GetComponent<GraphicRaycaster>();
        eventsystem = GetComponent<EventSystem>();

        panelOpen = false;
    }

    void Update()
    {
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
    }
}
