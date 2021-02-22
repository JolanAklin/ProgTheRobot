using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WindowsManager : MonoBehaviour
{
    public enum popUp
    {
        menu = 0,
        robotModif,
        colorPicker,
        saveWarning,
        addScript,
        readWrite
    }

    [Serializable]
    public class popUpClass
    {
        public GameObject popUpObj;
        public string popUpType;
    }

    public popUpClass[] popUpWindows;
    [HideInInspector]
    public Dictionary<int, GameObject> popUpWindowsDict = new Dictionary<int, GameObject>();

    public static WindowsManager instance;

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

    private void Start()
    {
        foreach (popUpClass popUp in popUpWindows)
        {
            popUpWindowsDict.Add((int)((popUp)Enum.Parse(typeof(popUp), popUp.popUpType)), popUp.popUpObj);
        }
        
    }

    public static GameObject InstantiateWindow(int WindowType, Transform parentTransform)
    {
        return Instantiate(instance.popUpWindowsDict[WindowType], parentTransform);
    }
}
