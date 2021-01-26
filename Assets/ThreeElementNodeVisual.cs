using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeElementNodeVisual : MonoBehaviour
{
    public RectTransform canvas;
    public RectTransform leftSide;
    public RectTransform rightSide;
    public RectTransform middleSide;

    private void FixedUpdate()
    {
        OnResize();
    }

    public void OnResize()
    {
        middleSide.sizeDelta = new Vector2(canvas.rect.width - (rightSide.rect.width + leftSide.rect.width), middleSide.rect.height);
    }
}
