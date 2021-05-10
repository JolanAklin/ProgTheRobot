using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// start tpi
public class CompletionProposition : MonoBehaviour
{
    public TMP_Text completionText;
    public TMP_InputField toFill;
    public string completion;

    /// <summary>
    /// Complete the selected node
    /// </summary>
    public void Complete()
    {
        toFill.Select();
        toFill.text = completion;
    }
}
//end tpi
