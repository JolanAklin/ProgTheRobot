using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PopUpManager : MonoBehaviour
{
    public enum PopUpTypes
    {
        robotModif,
        colorPicker,
        saveWarning,
        addScript,
        readWrite,
        nodeInfo,
        wait,
        menu,
        FillTest,
        FillWhile,
        FillAction,
    }

    [Serializable]
    public class PopUp
    {
        public PopUpTypes type;
        public GameObject obj;
    }

    public GameObject popUpBase;
    private static GameObject statPopUpBase;
    public List<PopUp> popUps = new List<PopUp>();
    private static Dictionary<PopUpTypes, GameObject> popUpsDict = new Dictionary<PopUpTypes, GameObject>();

    private void Awake()
    {
        foreach (var popUp in popUps)
        {
            popUpsDict.Add(popUp.type, popUp.obj);
        }
        statPopUpBase = popUpBase;
    }

    /// <summary>
    /// Open main menu pop up
    /// </summary>
    public void OpenMenu()
    {
        GameObject basePopUp = Instantiate(statPopUpBase, Vector3.zero, Quaternion.identity);
        Instantiate(popUpsDict[PopUpTypes.menu], basePopUp.transform.GetChild(0).GetChild(1));
    }

    /// <summary>
    /// Open a pop up
    /// </summary>
    /// <param name="type">The type of popUp to open</param>
    /// <returns>Return the opened pop up object</returns>
    public static GameObject ShowPopUp(PopUpTypes type)
    {
        GameObject basePopUp = Instantiate(statPopUpBase, Vector3.zero, Quaternion.identity);
        return Instantiate(popUpsDict[type], basePopUp.transform.GetChild(0).GetChild(1));
    }
}
