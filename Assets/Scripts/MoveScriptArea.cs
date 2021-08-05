using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveScriptArea : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 mousePosBeginMove;
    private Vector3 cameraPosAtPanStart;
    private bool isCursorHover = true;
    [SerializeField] private float nodeAreaPanSensitivity;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Middle)
        {
            mousePosBeginMove = Input.mousePosition;
            cameraPosAtPanStart = NodeDisplay.instance.nodeCamera.transform.position;
            CursorManager.instance.ChangeCursor(CursorManager.CursorDef.CursorTypes.move, true);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Middle)
        {
            Vector3 currentMousePos = Input.mousePosition;
            Vector3 delta = (currentMousePos - mousePosBeginMove);
            Vector3 cameraPos = new Vector3(-delta.x / nodeAreaPanSensitivity, -delta.y / nodeAreaPanSensitivity, 0);
            NodeDisplay.instance.nodeCamera.transform.position = cameraPos + cameraPosAtPanStart;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        CursorManager.instance.UnLockCursorTexture();
        CursorManager.instance.ChangeCursor(CursorManager.CursorDef.CursorTypes.arrow);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isCursorHover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isCursorHover = false;
    }

    private void Update()
    {
        if(isCursorHover && Input.GetKey(KeyCode.LeftControl) && Input.mouseScrollDelta.y != 0)
        {
            float zoom = NodeDisplay.instance.nodeCamera.orthographicSize - Input.mouseScrollDelta.y;
            if (zoom < 1)
                zoom = 1f;
            NodeDisplay.instance.nodeCamera.orthographicSize = zoom;
        }
    }
}
