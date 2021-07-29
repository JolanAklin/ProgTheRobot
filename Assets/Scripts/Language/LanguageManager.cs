using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    /// <summary>
    /// Singleton representing the current language manager
    /// </summary>
    public static LanguageManager instance;

    /* one string in clear language and one string with function name abreviated
     * displayed string :   Wall distance = 10 And Wall right
     * abreviated string :  iwd = 10 And bwr
     * i = integer function and b = boolean function
     */
    // should be loaded dynamicly with a language file in the future
    private static Dictionary<string, string> abrevToFullName = new Dictionary<string, string>();
    private static Dictionary<string, string> fullNameToAbrev = new Dictionary<string, string>()
    {
        // boolean func
        {"Wall in front","bwf#"},
        {"Wall right","bwr#"},
        {"Wall left","bwl#"},
        {"Out","bo#"},
        {"Robot on an outlet","boao#"},
        {"Tile marked","btm#"},
        {"Ball on ground","bbg#"},
        {"True", "btrue#" },
        {"False", "bfalse#" },
        // int func
        {"Wall distance","iwd#"},
        {"Power","ip#"},
        {"Robot position x","irpx#"},
        {"Robot position y","irpy#"},
        {"Robot direction x","irdx#"},
        {"Robot direction y","irdy#"},
        {"Ball position x","ibpx#"},
        {"Ball position y","ibpy#"},
        // boolean operators
        {"And", "bopand#" },
        {"Or", "bopor#" },
        {"Not", "bopnot#" },
        // robot action
        { "Go forward", "acgf#" },
        { "Turn right", "actr#" },
        { "Turn left", "actl#" },
        { "Mark", "acm#" },
        { "Unmark", "acum#" },
        { "Recharge", "acr#" },
        { "Place ball", "acpb#" },
        { "Take ball", "actb#" },
        { "Throw ball", "actwb#" },
    };

    public Dictionary<string,string> FullNameToAbrevDict { get => fullNameToAbrev; }

    private Dictionary<Validator.ValidationType, List<string>> reservedKeywords = new Dictionary<Validator.ValidationType, List<string>>();
    public Dictionary<Validator.ValidationType, List<string>> ReservedKeywords { get => reservedKeywords; }

    private void Awake()
    {
        InverseKV();
        instance = this;
    }

    private void Start()
    {
        PrepareTrees();
        MakeReservedKeywordList();
    }

    private void MakeReservedKeywordList() // needs to be uptaded. this version cannot put some keywords in more than one validation type
    {
        reservedKeywords.Add(Validator.ValidationType.test, new List<string>());
        reservedKeywords.Add(Validator.ValidationType.action, new List<string>());
        char[] splitChars = new char[] { ' ' };
        foreach (KeyValuePair<string, string> function in fullNameToAbrev)
        {
            string[] temp = function.Key.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            Validator.ValidationType validationType = Validator.ValidationType.test;
            switch (function.Value)
            {
                case string func when func.StartsWith("i"):
                    validationType = Validator.ValidationType.test;
                    break;
                case string func when func.StartsWith("b"):
                    validationType = Validator.ValidationType.test;
                    break;
                case string func when func.StartsWith("bop"):
                    validationType = Validator.ValidationType.test;
                    break;
                case string func when func.StartsWith("ac"):
                    validationType = Validator.ValidationType.action;
                    break;
            }
            foreach (string word in temp)
            {
                if (!reservedKeywords[validationType].Contains(word))
                    reservedKeywords[validationType].Add(word);
            }
        }
    }


    /// <summary>
    /// Get the fullname of a function by using it's abreviation
    /// </summary>
    /// <param name="abrev">The abreviation used to return the fullname</param>
    /// <returns>The fullname of an abreviation</returns>
    public string getFullnameFromAbrev(string abrev)
    {
        if(abrevToFullName.ContainsKey(abrev))
        {
            return abrevToFullName[abrev];
        }
        return null;
    }

    /// <summary>
    /// Test if a key is present in the abreviation to fullname dictionnary
    /// </summary>
    /// <param name="key">The key to test for</param>
    /// <returns>True if the key is present</returns>
    public bool AbrevToFullNameContainsKey(string key)
    {
        return abrevToFullName.ContainsKey(key);
    }

    /// <summary>
    /// Get the abreviation of a function by using it's fullname
    /// </summary>
    /// <param name="fullname">The fullname used to return the abreviation</param>
    /// <returns>The abreviation of an fullname</returns>
    public string getAbrevFromFullname(string fullname)
    {
        if (fullNameToAbrev.ContainsKey(fullname))
        {
            return fullNameToAbrev[fullname];
        }
        return null;
    }

    /// <summary>
    /// Test if a key is present in the fullname to abreviation dictionnary
    /// </summary>
    /// <param name="key">The key to test for</param>
    /// <returns>True if the key is present</returns>
    public bool FullNameToAbrevContainsKey(string key)
    {
        return fullNameToAbrev.ContainsKey(key);
    }

    /// <summary>
    /// inverse the key and the value from the fullname to abreviation dictionary
    /// </summary>
    private static void InverseKV()
    {
        foreach (KeyValuePair<string, string> item in fullNameToAbrev)
        {
            abrevToFullName.Add(item.Value, item.Key);
        }
    }

    /// <summary>
    /// Convert all fullname to abreviation present in a string
    /// </summary>
    /// <param name="toConvert">String to convert</param>
    /// <returns>The converted string</returns>
    public string FullNameToAbrev(string toConvert)
    {
        foreach (KeyValuePair<string, string> item in fullNameToAbrev)
        {
            toConvert = toConvert.Replace(item.Key, item.Value);
        }
        return toConvert;
    }

    /// <summary>
    /// Convert all abreviation to fullname present in a string
    /// </summary>
    /// <param name="toConvert">String to convert</param>
    /// <returns>The converted string</returns>
    public string AbrevToFullName(string toConvert)
    {
        foreach (KeyValuePair<string,string> item in abrevToFullName)
        {
            toConvert = toConvert.Replace(item.Key, item.Value);
        }
        return toConvert;
    }


    #region decision tree

    public List<CompletionPossibilities> completionPossibilities = new List<CompletionPossibilities>();
    /// <summary>
    /// Same from above but easier to use in the code
    /// </summary>
    private Dictionary<Validator.ValidationType, CompletionPossibilities> completionPossibilitiesDict = new Dictionary<Validator.ValidationType, CompletionPossibilities>();

    /// <summary>
    /// Store a tree for every validation type
    /// </summary>
    private Dictionary<Validator.ValidationType, DecisionTree> dTrees = new Dictionary<Validator.ValidationType, DecisionTree>();
    public Dictionary<Validator.ValidationType, DecisionTree> DTrees { get => dTrees; private set => dTrees = value; }

    [Serializable]
    public class CompletionPossibilities
    {
        public Validator.ValidationType typeOfValidation;
        public List<string> possibilities = new List<string>();
    }
    /// <summary>
    /// Create all trees for every validation type given in completionPossibilities list
    /// </summary>
    private void PrepareTrees()
    {
        foreach (var item in completionPossibilities)
        {
            if (completionPossibilitiesDict.ContainsKey(item.typeOfValidation))
            {
                Debug.Log($"Found duplicate of \"{item.typeOfValidation}\"");
            }
            else
            {
                completionPossibilitiesDict.Add(item.typeOfValidation, item);
                dTrees.Add(item.typeOfValidation, MakeCompletionDecisionTree(item.typeOfValidation));
            }
        }
    }

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
    private DecisionTree MakeCompletionDecisionTree(Validator.ValidationType validationType)
    {
        DecisionTree tree = new DecisionTree();

        foreach (string possibility in completionPossibilitiesDict[validationType].possibilities)
        {
            List<string> temp = new List<string>();
            temp.AddRange(possibility.Split(' ')); // [Wall, in, front, bool]

            bool breakNext = false;
            for (int i = 0; i < temp.Count; i++)
            {
                if (breakNext)
                    break;

                TreeBranch treeBranch = new TreeBranch() { root = temp[i] };

                // set the branch type. defined by the last word of the function name, for example : Wall in front bool.
                if (i == temp.Count - 2)
                {
                    switch (temp[i + 1])
                    {
                        case "@int":
                            treeBranch.type = BranchType.@int;
                            breakNext = true;
                            break;
                        case "@bool":
                            treeBranch.type = BranchType.@bool;
                            breakNext = true;
                            break;
                        case "@boolOp":
                            treeBranch.type = BranchType.boolOp;
                            breakNext = true;
                            break;
                        case "@action":
                            treeBranch.type = BranchType.action;
                            breakNext = true;
                            break;
                    }
                }

                // link the branch to its parent or the base of the tree
                if (i > 0)
                {
                    TreeBranch parentBranch = parentBranch = tree.branches[temp[0]];
                    for (int j = 1; j < i; j++)
                    {
                        if (parentBranch.nextBranches.ContainsKey(temp[j]))
                        {
                            parentBranch = parentBranch.nextBranches[temp[j]];
                        }
                    }
                    if (parentBranch != null)
                    {
                        if (!parentBranch.nextBranches.ContainsKey(treeBranch.root))
                        {
                            treeBranch.parent = parentBranch;
                            parentBranch.nextBranches.Add(treeBranch.root, treeBranch);
                        }
                    }
                }
                else if (i == 0)
                {
                    if (!tree.branches.ContainsKey(treeBranch.root))
                        tree.branches.Add(temp[i], treeBranch);
                }
            }
        }
        Debug.Log(tree.ToString());
        return tree;
    }

    public enum BranchType
    {
        @int,
        @bool,
        boolOp,
        branch,
        action,
    }

    /// <summary>
    /// The branch of a tree. Composed of a name and a dictionnary with the next branches
    /// </summary>
    public class TreeBranch
    {
        public string root;
        public TreeBranch parent = null;
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
    public class DecisionTree
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
            }
            else
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
            if (currentBranch == null)
            {
                if (branches.ContainsKey(nextBranch))
                    currentBranch = branches[nextBranch];
                else
                    throw new BranchNotFoundException(nextBranch);
            }
            else
            {
                if (currentBranch.nextBranches.ContainsKey(nextBranch))
                    currentBranch = currentBranch.nextBranches[nextBranch];
                else
                    throw new BranchNotFoundException(nextBranch);
            }
        }

        public void GoToParent()
        {
            if (currentBranch != null)
            {
                if (currentBranch.parent != null)
                    currentBranch = currentBranch.parent;
                else
                    GoToRoots();
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
            if (currentBranch != null)
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
}
