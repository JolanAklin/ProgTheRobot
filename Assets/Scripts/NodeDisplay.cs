using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// hold the reference of the camera showing the nodes editing zone
public class NodeDisplay : MonoBehaviour
{
    public static NodeDisplay instance;
    public Camera nodeCamera;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }
}
