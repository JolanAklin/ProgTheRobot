using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.Linq;
using System.Text.RegularExpressions;

[RequireComponent(typeof(CustomInputField))]
public class PopUpNodeInput : MonoBehaviour
{
    private CustomInputField input;
    public CompletionMenu completionMenu;

    public Validator.ValidationType validationType;

    public Validator.ValidationReturn validation { get; private set; }

    public PopUpFillNode popUpFillNode;

    public class Completion
    {
        public double score;
        public string completion;
        public LanguageManager.BranchType completionType;
    }

    public void OnSelect()
    {
        LanguageManager.instance.DTrees[validationType].GoToRoots();
    }

    public void OnDeselect()
    {
        completionMenu.Close();
    }

    private void Start()
    {
        input = GetComponent<CustomInputField>();

        
        completionMenu.popUpNodeInput = this;

        lastInputStringLength = input.text.Length;
        lastCaretPos = input.caretPosition;
    }

    #region Validation

    /// <summary>
    /// Validate the input
    /// </summary>
    public void Validate()
    {
        string toValidate = input.text;
        toValidate = RemoveRichTag(toValidate);
        toValidate = FormatInput(toValidate);
        input.text = toValidate;
        validation = Validator.Validate(validationType, toValidate);
        if (validation.validationStatus == Validator.ValidationStatus.KO)
            popUpFillNode.ShowInfo(true, "");
        else
            popUpFillNode.ShowInfo(false, "No error found");
        Display(validation);
    }

    /// <summary>
    /// Remove <color> and <b> tags
    /// </summary>
    /// <param name="richText"></param>
    /// <returns></returns>
    private string RemoveRichTag(string richText)
    {
        richText = richText.Replace("<color=red><b>", "");
        richText = richText.Replace("</b></color>", "");
        return richText;
    }

    /// <summary>
    /// Format the string
    /// </summary>
    /// <param name="input">the string to format</param>
    /// <returns>The formated string</returns>
    private string FormatInput(string input)
    {
        input = input.Replace("=", " = ");
        input = input.Replace("<", " < ");
        input = input.Replace(">", " > ");
        input = input.Replace("<=", " <= ");
        input = input.Replace(">=", " >= ");
        input = input.Replace("<>", " <> ");
        input = input.Replace("+", " + ");
        input = input.Replace("-", " - ");
        input = input.Replace("*", " * ");
        input = input.Replace("/", " / ");
        input = input.Replace("(", " ( ");
        input = input.Replace(")", " ) ");

        string pattern = @"\s+";
        input = Regex.Replace(input, pattern, " ");
        return input;
    }

    /// <summary>
    /// Display errors on the input field
    /// </summary>
    /// <param name="validationReturn">The ValidationReturn returned from the validator</param>
    public void Display(Validator.ValidationReturn validationReturn)
    {
        Dictionary<uint, string> tags = new Dictionary<uint, string>();
        string displayString = input.text;
        uint offset = 0;
        if (validationReturn.validationStatus == Validator.ValidationStatus.KO)
        {
            // adds tag in a list to put them correctly in the string
            foreach (KeyValuePair<uint, Validator.ValidationReturn.Error> error in validationReturn.specificErrors)
            {
                if (!(tags.ContainsKey(error.Value.startPos) || tags.ContainsKey(error.Value.endPos)))
                {
                    tags.Add(error.Value.startPos, "<color=red><b>");
                    tags.Add(error.Value.endPos, "</b></color>");
                }
            }
            Dictionary<uint, string> orderedTags = new Dictionary<uint, string>();
            foreach (KeyValuePair<uint, string> kvp in tags.OrderBy(key => key.Key))
            {
                orderedTags.Add(kvp.Key, kvp.Value);
            }
            foreach (KeyValuePair<uint, string> tag in orderedTags)
            {
                displayString = displayString.Insert((tag.Key + offset < displayString.Length) ? (int)(tag.Key + offset) : displayString.Length, tag.Value);
                offset += (uint)tag.Value.Length;
            }
            input.text = displayString;
        }
    }

    /// <summary>
    /// Show error messsage
    /// </summary>
    /// <param name="index">The index where the caret is</param>
    public void ShowError(int index)
    {
        string errorMessage = "";
        if (validation.specificErrors != null && validation.generalErrors != null)
        {
            foreach (var errors in validation.specificErrors)
            {
                if (index >= errors.Key && index <= errors.Value.endPos)
                {
                    errorMessage = errors.Value.message;
                    popUpFillNode.ShowInfo(true, errorMessage);
                    return;
                }
            }
            if (validation.validationStatus == Validator.ValidationStatus.KO)
                popUpFillNode.ShowInfo(true, "");
            else
                popUpFillNode.ShowInfo(false, "No error found");
        }
    }

    #endregion

    #region Autocompletion

    private int lastInputStringLength = 0;
    private int lastCaretPos = 0;

    private void Update()
    {
        // test if the caret has been moved by the mouse
        if (lastInputStringLength == input.text.Length && lastCaretPos != input.caretPosition)
        {
            LanguageManager.instance.DTrees[validationType].GoToRoots();
            toReplace = "";
            List<Completion> proba = new List<Completion>();
            foreach (LanguageManager.TreeBranch possibility in LanguageManager.instance.DTrees[validationType].getNextBranches())
            {
                proba.Add(new Completion() { score = Mathf.Infinity, completion = possibility.root, completionType = possibility.type });
            }
            if (proba.Count == 0)
                return;
            // show completion proposition to the user
            Completion[] completionProbabilitiesArray = proba.ToArray();
            QuickSortCompletionProbability(completionProbabilitiesArray, 0, proba.Count - 1);
            completionMenu.ShowCompletionProposition(completionProbabilitiesArray);
        }
        lastInputStringLength = input.text.Length;
        lastCaretPos = input.caretPosition;
    }

    private void LateUpdate()
    {
        hasProcessedBackSpace = false;
    }

    private bool hasCompleted = false;
    private bool lastKeyWasBackspace = false;
    private bool hasProcessedBackSpace = false;
    public string toReplace { get; private set; }

    /// <summary>
    /// Predict what the user can write
    /// </summary>
    /// <param name="forceShow">Show the completion list even if the input is not focused</param>
    public void PredictCompletion(bool forceShow = false)
    {
        if (Input.GetKeyDown(KeyCode.Backspace) && !hasProcessedBackSpace)
        {
            lastKeyWasBackspace = true;
        }
        if (input.hasPasted)
        {
            return;
        }
        if (!input.isFocused && !forceShow)
            return;


        // store the score and the text to complete with
        List<Completion> proba = new List<Completion>();
        string toComplete = "";
        if (!hasCompleted)
        {
            toComplete = FindWord(RemoveRichTag(input.text), input.caretPosition);
        }
        else
        {
            toComplete = "";
            hasCompleted = false;
        }
        toReplace = toComplete;
        // find completion possibilities
        foreach (LanguageManager.TreeBranch possibility in LanguageManager.instance.DTrees[validationType].getNextBranches())
        {
            if (possibility.root.StartsWith(toComplete, StringComparison.OrdinalIgnoreCase))
            {
                proba.Add(new Completion() { score = (double)possibility.root.Length / toComplete.Length, completion = possibility.root, completionType = possibility.type });
                if (toComplete.ToLower() == possibility.root.ToLower())
                {
                    // rewrite the word if the case does not match
                    if (toComplete != possibility.root)
                        Complete(possibility.root, toComplete);
                    else
                    {

                        if (!lastKeyWasBackspace)
                        {
                            hasCompleted = true;
                            LanguageManager.instance.DTrees[validationType].SelectNextBranch(possibility.root);
                            if (!LanguageManager.instance.DTrees[validationType].CurrentBranchHasNext())
                                LanguageManager.instance.DTrees[validationType].GoToRoots();
                            string completedString = input.text;
                            completedString = completedString.Insert(input.stringPosition, " ");
                            input.text = completedString;
                            input.stringPosition += 1;
                        }
                        else
                        {
                            lastKeyWasBackspace = false;
                            hasProcessedBackSpace = true;

                            if (toComplete.Length == 0)
                            {
                                LanguageManager.instance.DTrees[validationType].GoToParent();
                                PredictCompletion();
                                return;
                            }
                        }
                    }
                    return;
                }
            }
        }
        if (lastKeyWasBackspace)
        {
            lastKeyWasBackspace = false;
            hasProcessedBackSpace = true;
            if (toComplete.Length == 0)
            {
                LanguageManager.instance.DTrees[validationType].GoToParent();
                PredictCompletion();
                return;
            }
        }
        Debug.Log(toReplace);
        if (proba.Count == 0)
            return;
        // show completion proposition to the user
        Completion[] completionProbabilitiesArray = proba.ToArray();
        QuickSortCompletionProbability(completionProbabilitiesArray, 0, proba.Count - 1);
        completionMenu.ShowCompletionProposition(completionProbabilitiesArray);
    }

    /// <summary>
    /// Complete the input
    /// </summary>
    /// <param name="completion">The word to insert</param>
    /// <param name="toReplace">The word to replace</param>
    public void Complete(string completion, string toReplace)
    {
        hasCompleted = true;
        LanguageManager.instance.DTrees[validationType].SelectNextBranch(completion);
        if (!LanguageManager.instance.DTrees[validationType].CurrentBranchHasNext())
            LanguageManager.instance.DTrees[validationType].GoToRoots();
        completion += " ";
        // if there is nothing to override, it just insert it
        if (toReplace == "")
        {
            string completedString = input.text;
            completedString = completedString.Insert(input.stringPosition, completion);
            input.text = completedString;
            input.stringPosition += completion.Length;
        }
        // replace a given word
        else
        {
            string completedString = input.text;
            int[] toRemove = FindNearestWord(toReplace, completedString, input.stringPosition);
            completedString = completedString.Remove(toRemove[0], toRemove[1]);
            completedString = completedString.Insert(toRemove[0], completion);
            input.text = completedString;
            input.stringPosition = toRemove[0] + completion.Length;
        }
    }

    /// <summary>
    /// Find the nearest word ending from a position in a string
    /// </summary>
    /// <param name="wordToFind">The word to search for</param>
    /// <param name="text">The string to search in</param>
    /// <param name="pos">The position the word should be the nearest</param>
    /// <returns>An int array containing the index and the length of the word</returns>
    private int[] FindNearestWord(string wordToFind, string text, int pos)
    {
        Regex findWord = new Regex(wordToFind);

        MatchCollection matches = findWord.Matches(text);

        if (matches.Count == 0)
            return null;

        int nearestMatch = Mathf.Abs(matches[0].Index + matches[0].Length - pos);
        int nearestMatchPos = 0;

        for (int i = 1; i < matches.Count; i++)
        {
            int matchDistance = Mathf.Abs(matches[i].Index + matches[i].Length - pos);
            if (matchDistance < nearestMatch)
            {
                nearestMatch = matchDistance;
                nearestMatchPos = i;
            }
        }

        return new int[2] { matches[nearestMatchPos].Index, matches[nearestMatchPos].Length };
    }

    /// <summary>
    /// Find the nearest word from the specified pos
    /// </summary>
    /// <param name="text">The text to search the word</param>
    /// <param name="pos">The pos that the word needs to be near of</param>
    /// <returns>The value of the closest word from the given pos</returns>
    private string FindWord(string text, int pos)
    {
        // if the caret is after a space, it is considered has a new word so returning "" will show all proposition.
        if (pos > 0)
            if (text[pos - 1] == ' ')
                return "";

        string regexWord = @"[a-zA-Z]+";
        Regex findWord = new Regex(regexWord);

        MatchCollection matches = findWord.Matches(text);

        if (matches.Count == 0)
            return "";

        int nearestMatch = Mathf.Abs(matches[0].Index + matches[0].Length - pos);
        int nearestMatchPos = 0;

        // find the nearest match
        for (int i = 1; i < matches.Count; i++)
        {
            int matchDistance = Mathf.Abs(matches[i].Index + matches[i].Length - pos);
            if (matchDistance < nearestMatch)
            {
                nearestMatch = matchDistance;
                nearestMatchPos = i;
            }
        }

        return matches[nearestMatchPos].Value;
    }

    #region quick sort
    /// <summary>
    /// <see cref="https://www.geeksforgeeks.org/quick-sort/"/>
    /// Sort an array using quicksort algorithm
    /// </summary>
    /// <param name="arr">The array to sort</param>
    /// <param name="low">the start point</param>
    /// <param name="high">the end point</param>
    private void QuickSortCompletionProbability(Completion[] arr, int low, int high)
    {
        if (low < high)
        {
            //partioning index
            int pi = partition(arr, low, high);

            // Separately sort elements before partition and after partition
            QuickSortCompletionProbability(arr, low, pi - 1);
            QuickSortCompletionProbability(arr, pi + 1, high);
        }
    }

    /// <summary>
    /// Partition the array for quicksort
    /// </summary>
    /// <param name="arr">The array to sort</param>
    /// <param name="low">The first index to take into account</param>
    /// <param name="high">The last index to take into account</param>
    /// <returns></returns>
    private int partition(Completion[] arr, int low, int high)
    {
        Completion temp;
        Completion pivot = arr[high]; // pivot
        int i = (low - 1); // Index of smaller element and indicates the right position of pivot found so far

        for (int j = low; j <= high - 1; j++)
        {
            // If current element is smaller than the pivot
            if (arr[j].score < pivot.score)
            {
                i++; // increment index of smaller element
                temp = arr[i];
                arr[i] = arr[j];
                arr[j] = temp;
            }
        }
        temp = arr[i + 1];
        arr[i + 1] = arr[high];
        arr[high] = temp;
        return (i + 1);
    }
    #endregion

    #endregion
}
