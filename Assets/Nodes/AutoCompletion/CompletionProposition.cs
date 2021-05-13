using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

// start tpi
public class CompletionProposition : MonoBehaviour
{
    /// <summary>
    /// The text that show the completion proposition
    /// </summary>
    public TMP_Text completionText;
    /// <summary>
    /// the inputField to fill
    /// </summary>
    public TMP_InputField toFill;
    public string completion;
    /// <summary>
    /// Action called when the complete method is triggered
    /// </summary>
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
