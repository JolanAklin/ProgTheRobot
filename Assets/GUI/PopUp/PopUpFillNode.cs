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
    public CustomInputField input;
    public Validator.ValidationType validationType;

    public CompletionMenu completionMenu;
    // must be filled by the new language manager
    public List<CompletionPossibilities> completionPossibilities = new List<CompletionPossibilities>();
    private CompletionPossibilities completionForRightType;

    private DecisionTree dTree;

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
        public string toReplace;
        public BranchType completionType;
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
        toValidate = RemoveRichTag(toValidate);
        toValidate = FormatInput(toValidate);
        input.text = toValidate;
        Display(Validator.Validate(validationType, toValidate));
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
        dTree = MakeCompletionDecisionTree();
        completionMenu.popUpFillNode = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
            lastKeyWasBackspace = true;
    }

    #region decision tree
    /// <summary>
    /// Does a tree with each key word. for example : there is "Wall distance" and "Wall in front" function. The algorithme does this :
    /// Wall
    ///    |- in
    ///        |- front
    ///    |- distance
    ///    
    /// When the autocompletion algorithm detect on of the first branches it will propose the rest of the branch.
    /// For example the user writes "Wall" the algorithme will propose the keyword "in" and "distance".
    /// </summary>
    private DecisionTree MakeCompletionDecisionTree()
    {
        DecisionTree tree = new DecisionTree();

        foreach (string possibility in completionForRightType.possibilities)
        {
            List<string> temp = new List<string>();
            temp.AddRange(possibility.Split(' ')); // [Wall, in, front, bool]

            bool breakNext = false;
            for (int i = 0; i < temp.Count; i++)
            {
                if (breakNext)
                    break;

                TreeBranch treeBranch = new TreeBranch() { root = temp[i] };

                if(i == temp.Count - 2)
                {
                    switch (temp[i + 1])
                    {
                        case "int":
                            treeBranch.type = BranchType.intLeaf;
                            breakNext = true;
                            break;
                        case "bool":
                            treeBranch.type = BranchType.boolLeaf;
                            breakNext = true;
                            break;
                        case "boolOp":
                            treeBranch.type = BranchType.boolOpLeaf;
                            breakNext = true;
                            break;
                    }
                }

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
                        {
                            treeBranch.parent = previousBranch;
                            previousBranch.nextBranches.Add(treeBranch.root, treeBranch);
                        }
                    }
                }else if(i == 0)
                {
                    if(!tree.branches.ContainsKey(treeBranch.root))
                        tree.branches.Add(temp[i], treeBranch);
                }
            }
        }
        Debug.Log(tree.ToString());
        return tree;
    }

    public enum BranchType
    {
        intLeaf,
        boolLeaf,
        boolOpLeaf,
        branch,
    }

    /// <summary>
    /// The branch of a tree. Composed of a name and a dictionnary with the next branches
    /// </summary>
    private class TreeBranch
    {
        public string root;
        public TreeBranch parent;
        public Dictionary<string, TreeBranch> nextBranches = new Dictionary<string, TreeBranch>();
        public BranchType type = BranchType.branch;

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

    /// <summary>
    /// A tree that will help the autocompletion to propose relevent keyword to the user.
    /// </summary>
    private class DecisionTree
    {
        public Dictionary<string, TreeBranch> branches = new Dictionary<string, TreeBranch>();

        public TreeBranch currentBranch { get; private set; }

        public DecisionTree()
        {
            currentBranch = null;
        }

        /// <summary>
        /// Show the next branches name
        /// </summary>
        /// <returns>The name of all next branches</returns>
        public TreeBranch[] getNextBranches()
        {
            TreeBranch[] nextBranches = null;
            if (currentBranch == null)
            {
                nextBranches = new TreeBranch[branches.Count];
                for (int i = 0; i < nextBranches.Length; i++)
                {
                    nextBranches[i] = branches.ElementAt(i).Value;
                }
            }else
            {
                nextBranches = new TreeBranch[currentBranch.nextBranches.Count];
                for (int i = 0; i < nextBranches.Length; i++)
                {
                    nextBranches[i] = currentBranch.nextBranches.ElementAt(i).Value;
                }
            }
            return nextBranches;
        }

        /// <summary>
        /// Select the new current branch
        /// </summary>
        /// <param name="nextBranch">The branch to select</param>
        public void SelectNextBranch(string nextBranch)
        {
            if(currentBranch == null)
            {
                if (branches.ContainsKey(nextBranch))
                    currentBranch = branches[nextBranch];
                else
                    throw new BranchNotFoundException(nextBranch);
            }else
            {
                if (currentBranch.nextBranches.ContainsKey(nextBranch))
                    currentBranch = currentBranch.nextBranches[nextBranch];
                else
                    throw new BranchNotFoundException(nextBranch);
            }
        }

        /// <summary>
        /// Go to the roots if the tree
        /// </summary>
        public void GoToRoots()
        {
            currentBranch = null;
        }

        /// <summary>
        /// Test if the current branch has another branch after
        /// </summary>
        /// <returns>True if the current branch has a branch after</returns>
        public bool CurrentBranchHasNext()
        {
            if(currentBranch != null)
            {
                if (currentBranch.nextBranches.Count > 0)
                    return true;
                else
                    return false;
            }
            else
            {
                if (branches.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        public override string ToString()
        {
            string message = "";
            foreach (TreeBranch branch in branches.Values)
            {
                message += branch.ToString();
            }
            return message;
        }

        [Serializable]
        public class BranchNotFoundException : Exception
        {
            public BranchNotFoundException() { }
            public BranchNotFoundException(string message) : base($"Asked Branch : \"{ message }\" not found.") { }
            public BranchNotFoundException(string message, Exception inner) : base(message, inner) { }
            protected BranchNotFoundException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }
    }
    #endregion

    public bool hasCompleted = false;
    private bool lastKeyWasBackspace;

    /// <summary>
    /// Predict what the user can write
    /// </summary>
    /// <param name="forceShow">Show the completion list even if the input is not focused</param>
    public void PredictCompletion(bool forceShow = false)
    {
        if(lastKeyWasBackspace)
        {
            lastKeyWasBackspace = false;
            return;
        }
        if (!input.isFocused && !forceShow)
            return;
        // store the score and the text to complete with
        List<Completion> proba = new List<Completion>();
        string toComplete = "";
        if (!hasCompleted)
            toComplete = FindWord(RemoveRichTag(input.text), input.caretPosition);
        else
        {
            toComplete = "";
            hasCompleted = false;
        }
        foreach (TreeBranch possibility in dTree.getNextBranches())
        {
            if (possibility.root.StartsWith(toComplete, StringComparison.OrdinalIgnoreCase))
            {
                proba.Add(new Completion() { score = (double)possibility.root.Length / toComplete.Length, completion = possibility.root, toReplace = toComplete, completionType = possibility.type });
                if (toComplete.ToLower() == possibility.root.ToLower())
                {
                    Complete(possibility.root, toComplete);
                    return;
                }
            }
        }
        if (proba.Count == 0)
            return;
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
        dTree.SelectNextBranch(completion);
        if (!dTree.CurrentBranchHasNext())
            dTree.GoToRoots();
        completion += " ";
        if (toReplace == "")
        {
            string completedString = input.text;
            completedString = completedString.Insert(input.stringPosition, completion);
            input.text = completedString;
            input.stringPosition += completion.Length;
        }
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
        // if the caret is after a space, it is considered has a new word so returning "" while show all proposition.
        if (pos > 0)
            if (text[pos - 1] == ' ')
                return "";

        string regexWord = @"[a-zA-Z]+";
        Regex findWord = new Regex(regexWord);

        MatchCollection matches = findWord.Matches(text);

        if(matches.Count == 0)
            return "";

        int nearestMatch = Mathf.Abs(matches[0].Index - pos);
        int nearestMatchPos = 0;

        // find the nearest match
        for (int i = 1; i < matches.Count; i++)
        {
            int matchDistance = Mathf.Abs(matches[i].Index - pos);
            if (matchDistance < nearestMatch)
            {
                nearestMatch = matchDistance;
                nearestMatchPos = i;
            }

            matchDistance = Mathf.Abs(matches[i].Index + matches[i].Length - pos);
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

    public void Cancel()
    {
        cancelAction();
    }
    public void Ok()
    {
        OkAction();
    }
}