using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CompletionMenu : MonoBehaviour
{
    public GameObject completionProposition;
    public GameObject completionPanel;

    public PopUpFillNode popUpFillNode;

    private RectTransform completionPanelRectTransform;

    private int selectedItemIndex = 0;
    private List<CompletionItem> completionItems = new List<CompletionItem>();

    private void Start()
    {
        completionPanelRectTransform = completionPanel.GetComponent<RectTransform>();
    }

    public void ShowCompletionProposition(PopUpFillNode.Completion[] completions)
    {
        completionItems.Clear();
        foreach (Transform child in completionPanel.transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < completions.Length; i++)
        {
            CompletionItem completionItem = Instantiate(completionProposition, completionPanel.transform).GetComponent<CompletionItem>();
            completionItem.SetText(completions[i].completion);
            completionItem.completionMenu = this;
            completionItem.popUpFillNode = popUpFillNode;
            completionItem.toReplace = completions[i].toReplace;
            completionItem.type = completions[i].completionType;
            completionItems.Add(completionItem);
            if(i == 0)
            {
                SelectCompletionItem(completionItem);
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(completionPanelRectTransform);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveSelection(-1);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveSelection(1);
        }
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            completionItems[selectedItemIndex].Complete();
        }
    }

    public void SelectCompletionItem(CompletionItem completionItem)
    {
        if(completionItems.Count > selectedItemIndex)
            completionItems[selectedItemIndex].ChangeColor(false);
        selectedItemIndex = completionItems.IndexOf(completionItem);
        completionItem.ChangeColor(true);
    }

    public void MoveSelection(int dir)
    {
        if(dir == -1)
        {
            if(selectedItemIndex > 0)
            {
                if (completionItems.Count > selectedItemIndex)
                    completionItems[selectedItemIndex].ChangeColor(false);

                selectedItemIndex -= 1;
                completionItems[selectedItemIndex].ChangeColor(true);
            }
        }else if (dir == 1)
        {
            if (selectedItemIndex + 1 < completionItems.Count)
            {
                if (completionItems.Count > selectedItemIndex)
                    completionItems[selectedItemIndex].ChangeColor(false);

                selectedItemIndex += 1;
                completionItems[selectedItemIndex].ChangeColor(true);
            }
        }
    }
}