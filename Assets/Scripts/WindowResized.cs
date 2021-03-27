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
using System;

public class WindowResized : MonoBehaviour
{
    public static WindowResized instance;

    public event EventHandler<WindowResizedEventArgs> onWindowResized;

    public class WindowResizedEventArgs : EventArgs
    {
        public int screenHeight;
        public int screenWidth;
    }

    private int screenHeight;
    private int screenWidth;

    private void Awake()
    {
        instance = this;
    }

    public void FixedUpdate()
    {
        if (screenHeight != Screen.height || screenWidth != Screen.width)
        {
            screenHeight = Screen.height;
            screenWidth = Screen.width;

            onWindowResized?.Invoke(this, new WindowResizedEventArgs() { screenHeight = Screen.height, screenWidth = Screen.width});
        }
    }
}
