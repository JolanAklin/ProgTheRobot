using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Language;

public class TranslationTestScript : MonoBehaviour
{
    private void Start()
    {
        Translation.Init();
        Translation.LoadData("fr");
        var hello = Translation.Get("welcome");
        Debug.Log(hello);
    }
}