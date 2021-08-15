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
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveLinkHandle : MonoBehaviour, IPointerDownHandler
{
    public GameObject SplineLink;

    private Image image;

    public void Start()
    {
        image = GetComponent<Image>();
        Manager.instance.OnSpline += ShowHide;
    }

    private void OnDestroy()
    {
        Manager.instance.OnSpline -= ShowHide;
    }

    public void ShowHide(object sender, Manager.OnSplineEventArgs e)
    {

        if (!e.splineStarted)
        {
            image.enabled = true;
        }
        else
        {
            image.enabled = false;
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            SplineLink.GetComponent<SplineManager>().MoveSpline();
        }
    }
}
