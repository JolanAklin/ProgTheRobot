using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeElementNodeVisual : MonoBehaviour
{
    public Transform nodeRoot;
    public BoxCollider2D nodeCollider;
    public RectTransform canvas;
    public bool needResize = true;
    public bool verticalResize = false;
    public RectTransform leftSide;
    public RectTransform rightSide;
    public RectTransform middleSide;

    private void Start()
    {
        OnResize();
    }

    public void OnResize()
    {
        if(needResize)
        {
            if(verticalResize)
            {
                middleSide.sizeDelta = new Vector2(0, canvas.rect.height - (rightSide.rect.height + leftSide.rect.height));
            }
            else
            {
                middleSide.sizeDelta = new Vector2(canvas.rect.width - (rightSide.rect.width + leftSide.rect.width), 0);
            }
        }
        nodeCollider.size = new Vector2(canvas.rect.width*canvas.localScale.x, middleSide.rect.height*canvas.localScale.y);
        canvas.position = new Vector2(nodeRoot.position.x, nodeRoot.position.y);
    }
}
