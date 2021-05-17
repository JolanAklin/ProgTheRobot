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
