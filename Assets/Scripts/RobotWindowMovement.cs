using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RobotWindowMovement : MonoBehaviour, IDragHandler, IBeginDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private bool pointerHover;
    [SerializeField] private CameraController cameraController;

    public void OnBeginDrag(PointerEventData eventData)
    {
        cameraController = RobotScript.robotScripts[Manager.instance.currentlySelectedScript].robot.robotManager.cameraPoint.GetComponent<CameraController>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Middle)
        {
            cameraController.Move();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cameraController = RobotScript.robotScripts[Manager.instance.currentlySelectedScript].robot.robotManager.cameraPoint.GetComponent<CameraController>();
        pointerHover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerHover = false;
    }

    private void Update()
    {
        if (Input.mouseScrollDelta.y != 0 && pointerHover && cameraController != null)
        {
            cameraController.Zoom();
        }
    }
}
