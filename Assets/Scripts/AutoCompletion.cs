using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Language;
using F23.StringSimilarity;
using System;
using TMPro;
using UnityEngine.UI;

// start tpi

/// <summary>
/// manage the auto completion for a text input
/// </summary>
public class AutoCompletion : MonoBehaviour
{
    // get the full word from language files
    [Tooltip("If true, the key to the text in the language file must be used")]
    public bool useLanguageFiles = true;
    public string[] possibleWord;

    public TMP_InputField toComplete;
    public Nodes completedNode;

    public GameObject completionPropositionPrefab;

    private RectTransform rectTransform;
    private string[] possibleCompletion;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (useLanguageFiles)
            Manager.instance.OnLanguageChanged += ChangeProbaWord;
        ChangeProbaWord(this, EventArgs.Empty);
    }
    private void OnDestroy()
    {
        if (useLanguageFiles)
            Manager.instance.OnLanguageChanged += ChangeProbaWord;
    }

    private void ChangeProbaWord(object sender, EventArgs e)
    {
        if(useLanguageFiles)
        {
            possibleCompletion = new string[possibleWord.Length];
            for (int i = 0; i < possibleWord.Length; i++)
            {
                possibleCompletion[i] = Translation.Get(possibleWord[i]);
            }
        }
        else
        {
            possibleCompletion = possibleWord;
        }
    }

    public void ShowCompletion(TMP_InputField inputField)
    {
        CompletionProbability[] sortedProba = GetCompletion(inputField.text);
        HideCompletion();
        foreach (CompletionProbability completion in sortedProba)
        {
            CompletionProposition completionProposition = Instantiate(completionPropositionPrefab, Vector3.zero, Quaternion.identity, this.transform).GetComponent<CompletionProposition>();
            completionProposition.completionText.text = completion.completion;
            completionProposition.toFill = toComplete;
            completionProposition.completion = completion.completion;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    public void HideCompletion()
    {
        foreach (Transform child in this.transform)
        {
            Destroy(child.transform.gameObject);
        }
    }

    public void HideCompletionWait()
    {
        StartCoroutine("HideCompletionOnNextFrame");
    }

    IEnumerator HideCompletionOnNextFrame()
    {
        yield return new WaitForSeconds(0.1f);
        foreach (Transform child in this.transform)
        {
            Destroy(child.transform.gameObject);
        }
    }

    /// <summary>
    /// Get the possible completion in the propability order
    /// </summary>
    /// <param name="text">The text to complete</param>
    /// <returns>A string array with all the possible completions in order of the most propable</returns>
    public CompletionProbability[] GetCompletion(string text)
    {
        NormalizedLevenshtein nl = new NormalizedLevenshtein();
        Debug.Log(nl.Distance("anticonstitutionnellement", "xyz"));
        List<CompletionProbability> completionProbabilities = new List<CompletionProbability>();
        foreach (string completion in possibleCompletion)
        {
            double dist = nl.Distance(completion, text);
            // 0 = exactly the same, 1 = nothing in common
            if (dist < 0.9f)
                completionProbabilities.Add(new CompletionProbability() { completion = completion, dist = dist });
        }
        CompletionProbability[] completionProbabilitiesArray = completionProbabilities.ToArray();
        QuickSortCompletionProbability(completionProbabilitiesArray, 0, completionProbabilities.Count - 1);
        return completionProbabilitiesArray;
    }

    public class CompletionProbability
    {
        public string completion;
        public double dist;
    }


    /// <summary>
    /// <see cref="https://www.geeksforgeeks.org/quick-sort/"/>
    /// Sort an array using quicksort algorithme
    /// </summary>
    /// <param name="arr">The array to sort</param>
    /// <param name="low">the start point</param>
    /// <param name="high">the end point</param>
    private void QuickSortCompletionProbability(CompletionProbability[] arr, int low, int high)
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

    private int partition(CompletionProbability[] arr, int low, int high)
    {
        CompletionProbability temp;
        CompletionProbability pivot = arr[high]; // pivot
        int i = (low - 1); // Index of smaller element and indicates the right position of pivot found so far

        for (int j = low; j <= high - 1; j++)
        {
            // If current element is smaller than the pivot
            if (arr[j].dist < pivot.dist)
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
}

// end tpi