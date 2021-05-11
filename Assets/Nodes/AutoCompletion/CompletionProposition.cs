using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

// start tpi
public class CompletionProposition : MonoBehaviour
{
    public TMP_Text completionText;
    public TMP_InputField toFill;
    public string completion;
    public Action callBack; 

    /// <summary>
    /// Complete the selected node
    /// </summary>
    public void Complete()
    {
        toFill.Select();
        toFill.text = completion;
        callBack?.Invoke();
    }
}
//end tpi
