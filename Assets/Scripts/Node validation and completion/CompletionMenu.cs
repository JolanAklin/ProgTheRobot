using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompletionMenu : MonoBehaviour
{
    public GameObject completionProposition;
    public GameObject completionPanel;

    private RectTransform completionPanelRectTransform;

    private void Start()
    {
        completionPanelRectTransform = completionPanel.GetComponent<RectTransform>();
    }

    public void ShowCompletionProposition(PopUpFillNode.Completion[] completions)
    {
        foreach (Transform child in completionPanel.transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = completions.Length - 1; i >= 0; i--)
        {
            CompletionItem completionItem = Instantiate(completionProposition, completionPanel.transform).GetComponent<CompletionItem>();
            completionItem.SetText(completions[i].completion);
        }
    }
}