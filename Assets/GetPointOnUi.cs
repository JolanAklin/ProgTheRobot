using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//start tpi
public class GetPointOnUi : MonoBehaviour
{
    private static RectTransform rectTransform;
    public Camera uiCamera;
    private static Camera mainCamera;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCamera = uiCamera;
    }

    /// <summary>
    /// Return the mouse position as a point in the ui.
    /// <see cref="https://answers.unity.com/questions/799616/unity-46-beta-19-how-to-convert-from-world-space-t.html?_ga=2.179995431.2029764217.1619080466-1505705523.1618821093"/>
    /// </summary>
    /// <returns></returns>
    public static Vector2 GetMousePosOnUi()
    {
        Vector2 ViewportPosition = mainCamera.ScreenToViewportPoint(Input.mousePosition);
        Vector2 proportionalPosition = new Vector2(ViewportPosition.x * rectTransform.sizeDelta.x, ViewportPosition.y * rectTransform.sizeDelta.y);
        return proportionalPosition;
    }
}
//end tpi
