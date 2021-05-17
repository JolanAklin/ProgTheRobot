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
    public string lettersToRemove;
    public Nodes completedNode;
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

        toFill.text = ReplaceLastOccurrence(toFill.text, lettersToRemove, completion);
        callBack?.Invoke();

        //completedNode.LockUnlockAllInput(false);
        //toFill.Select();
    }

    /// <summary>
    /// <see cref="https://stackoverflow.com/questions/14825949/replace-the-last-occurrence-of-a-word-in-a-string-c-sharp"/>
    /// </summary>
    /// <param name="Source">The source string</param>
    /// <param name="Find">The string to replace</param>
    /// <param name="Replace">The replacement</param>
    /// <returns>The original string with the find string replaced by the replace string</returns>
    private string ReplaceLastOccurrence(string Source, string Find, string Replace)
    {
        int place = Source.LastIndexOf(Find);

        if (place == -1)
            return Source;

        string result = Source.Remove(place, Find.Length).Insert(place, Replace);
        return result;
    }
}
//end tpi
