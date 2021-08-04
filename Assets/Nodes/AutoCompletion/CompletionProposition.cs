// Copyright 2021 Jolan Aklin

//This file is part of Prog The Robot.

//Prog The Robot is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 3 of the License.

//Prog The Robot is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Prog the robot.  If not, see<https://www.gnu.org/licenses/>.

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
        toFill.ActivateInputField();
        toFill.Select();
        toFill.caretPosition = toFill.text.Length;
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
