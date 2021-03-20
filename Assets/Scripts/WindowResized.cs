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
