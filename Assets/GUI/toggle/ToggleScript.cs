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
using UnityEngine.UI;
using UnityEngine.Events;

public class ToggleScript : MonoBehaviour
{
    public GameObject handle;
    private RectTransform handleRectTransform;
    public GameObject fillArea;
    public Color fillAreaColor;

    public UnityEvent OnCheckChanged;

    private bool value;
    public bool Value { get => value; }

    private void Start()
    {
        handleRectTransform = handle.GetComponent<RectTransform>();
        fillArea.GetComponent<Image>().color = fillAreaColor;
        value = false;
    }

    public void CheckChanged()
    {
        value = !value;
        if (value)
        {
            fillArea.SetActive(true);
            handleRectTransform.localPosition = new Vector3(-5, 0, 0);
        }
        else
        {
            fillArea.SetActive(false);
            handleRectTransform.localPosition = new Vector3(-15, 0, 0);
        }
        OnCheckChanged?.Invoke();
    }

}
