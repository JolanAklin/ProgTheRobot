using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextToolTip : ToolTip
{
    public static TextToolTip instance { get; private set; }

    [SerializeField] private TMP_Text text;
    [SerializeField] private Vector2 margin;

    private new void Awake()
    {
        base.Awake();
        instance = this;
        HideToolTip();
    }

    private void SetText(string toolTipText)
    {
        text.SetText(toolTipText);
        text.ForceMeshUpdate();

        Vector2 txtSize = text.GetRenderedValues(false);

        bgRectTransform.sizeDelta = txtSize + margin;
    }

    private void Update()
    {
        MoveToolTip();
    }

    public void ShowToolTip(string toolTipText)
    {
        base.ShowToolTip();
        SetText(toolTipText);
    }
}
