using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour
{

    private static List debugList;


    private void Awake()
    {
        debugList = GetComponent<List>();
    }

    public static void Log(string text)
    {
        debugList.AddChoice(new List.ListElement() { displayedText = text });
    }
    public static void LogError(string text)
    {
        debugList.AddChoice(new List.ListElement() { displayedText = text });
    }
}
