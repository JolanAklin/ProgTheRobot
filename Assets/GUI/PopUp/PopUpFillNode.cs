using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using F23.StringSimilarity;

public class PopUpFillNode : PopUp
{
    public TMP_Text validationTypeText;
    public TMP_InputField input;
    public Validator.ValidationType validationType;


    public CompletionMenu completionMenu;
    // must be filled by the new language manager
    public List<CompletionPossibilities> completionPossibilities = new List<CompletionPossibilities>();
    private CompletionPossibilities completionForRightType;

    [Serializable]
    public class CompletionPossibilities
    {
        public Validator.ValidationType typeOfValidation;
        public List<string> possibilities = new List<string>();
    }

    public class Completion
    {
        public double score;
        public string completion;
    }

    private Validator.ValidationReturn validationReturn;

    public Action cancelAction;
    public Action OkAction;

    /// <summary>
    /// Initialise the pop up
    /// </summary>
    /// <param name="validationType">What type of validation to use</param>
    public void Init(Validator.ValidationType validationType)
    {
        this.validationType = validationType;
        validationTypeText.text = validationType.ToString();
        foreach (var item in completionPossibilities)
        {
            if(item.typeOfValidation == validationType)
            {
                completionForRightType = item;
                break;
            }
        }
    }

    #region Validation

    /// <summary>
    /// Validate the input
    /// </summary>
    public void Validate()
    {
        string toValidate = input.text;
        toValidate = toValidate.Replace("<color=red><b>", "");
        toValidate = toValidate.Replace("</b></color>", "");
        toValidate = FormatInput(toValidate);
        input.text = toValidate;
        Display(Validator.Validate(validationType, toValidate));
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
        this.validationReturn = validationReturn;
        Dictionary<uint, string> tags = new Dictionary<uint, string>();
        string displayString = input.text;
        uint offset = 0;
        if(validationReturn.validationStatus == Validator.ValidationStatus.KO)
        {
            foreach (KeyValuePair<uint, Validator.ValidationReturn.Error> error in validationReturn.specificErrors)
            {
                if(!(tags.ContainsKey(error.Value.startPos) || tags.ContainsKey(error.Value.endPos)))
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
        if(validationReturn.specificErrors != null && validationReturn.generalErrors != null)
        {
            foreach (var errors in validationReturn.specificErrors)
            {
                if(index >= errors.Key && index < errors.Value.endPos)
                {
                    errorMessage = errors.Value.message;
                    Debug.Log(errorMessage);
                    break;
                }
            }
        }
    }

    #endregion

    #region Autocompletion

    private void Start()
    {
        MakeCompletionDecisionTree();
    }

    #region decision tree
    /// <summary>
    /// Does a tree with each key word. for example : there is Wall distance and Wall in front function. The algorithme does this :
    /// Wall
    ///    |- in front
    ///    |- distance
    /// </summary>
    private void MakeCompletionDecisionTree()
    {
        Dictionary<string, List<int>> keyWordPos = new Dictionary<string, List<int>>();

        DecisionTree tree = new DecisionTree();

        /* first level = 0
         * so       Wall in front
         * level      0   1   2
         */

        foreach (string possibility in completionForRightType.possibilities)
        {
            List<string> temp = new List<string>();
            temp.AddRange(possibility.Split(' '));

            for (int i = 0; i < temp.Count; i++)
            {

                TreeBranch treeBranch = new TreeBranch() { root = temp[i] };

                if(i > 0)
                {
                    TreeBranch previousBranch = previousBranch = tree.branches[temp[0]];
                    for (int j = 1; j < i; j++)
                    {
                        if(previousBranch.nextBranches.ContainsKey(temp[j]))
                        {
                            previousBranch = previousBranch.nextBranches[temp[j]];
                        }
                    }
                    if(previousBranch != null)
                    {
                        if (!previousBranch.nextBranches.ContainsKey(treeBranch.root))
                            previousBranch.nextBranches.Add(treeBranch.root, treeBranch);
                    }
                }else if(i == 0)
                {
                    if(!tree.branches.ContainsKey(treeBranch.root))
                        tree.branches.Add(temp[i], treeBranch);
                }
            }
        }
        Debug.Log(tree.ToString());
    }

    private class TreeBranch
    {
        public string root;
        public Dictionary<string, TreeBranch> nextBranches = new Dictionary<string, TreeBranch>();

        public override string ToString()
        {
            string message = "|- " + root + Environment.NewLine;
            message += ConvertToString(5, this);
            return message;
        }

        private string ConvertToString(int spacing, TreeBranch treeBranch)
        {
            string message = "";
            foreach (TreeBranch branch in treeBranch.nextBranches.Values)
            {
                for (int i = 0; i < spacing; i++)
                {
                    message += " ";
                }
                message += "|- " + branch.root + Environment.NewLine;
                message += ConvertToString(spacing * 2, branch);
            }
            return message;
        }
    }

    private class DecisionTree
    {
        public Dictionary<string, TreeBranch> branches = new Dictionary<string, TreeBranch>();

        public override string ToString()
        {
            string message = "";
            foreach (TreeBranch branch in branches.Values)
            {
                message += branch.ToString();
            }
            return message;
        }
    }
    #endregion

    public void PredictCompletion()
    {
        if (!input.isFocused)
            return;
        // store the score and the text to complete with
        List<Completion> proba = new List<Completion>();

        Jaccard jd = new Jaccard();
        double score;
        double heighestScore = 0;
        foreach (string possibility in completionForRightType.possibilities)
        {
            score = jd.Similarity(input.text, possibility);
            if (score > heighestScore)
                heighestScore = score;
            if(score > 0.15)
            {
                proba.Add(new Completion() { score = score, completion = possibility });
            }
        }
        Debug.Log(heighestScore);
        Completion[] completionProbabilitiesArray = proba.ToArray();
        QuickSortCompletionProbability(completionProbabilitiesArray, 0, proba.Count - 1);
        completionMenu.ShowCompletionProposition(completionProbabilitiesArray);
    }

    /// <summary>
    /// <see cref="https://www.geeksforgeeks.org/quick-sort/"/>
    /// Sort an array using quicksort algorithme
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

    public void Cancel()
    {
        cancelAction();
    }
    public void Ok()
    {
        OkAction();
    }
}