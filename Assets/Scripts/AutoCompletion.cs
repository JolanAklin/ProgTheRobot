using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Language;

// start tpi

/// <summary>
/// manage the auto completion for a text input
/// </summary>
public class AutoCompletion : MonoBehaviour
{
    // get the full word from language files
    public string[] possibleWordKey;
    private string[] possibleCompletion;

    // Start is called before the first frame update
    void Start()
    {
        possibleCompletion = new string[possibleWordKey.Length];
        for (int i = 0; i < possibleWordKey.Length; i++)
        {
            possibleCompletion[i] = Translation.Get(possibleWordKey[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

// end tpi