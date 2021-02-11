using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// resize node's graphicals elements
public class ThreeElementNodeVisual : MonoBehaviour
{
    public Transform nodeRoot;
    public BoxCollider2D nodeCollider;
    public RectTransform canvas;
    public bool needResize = true;
    public bool verticalResize = false;

    public RectTransform leftSide; // or top
    public RectTransform rightSide; // or bottom
    public RectTransform middleSide; // middleSide is dumb but changing it will imply that I have to change all the nodes middle object reference in the inspector 

    private void Start()
    {
        Resize();
    }

    public void Resize()
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
        //canvas.position = new Vector2(nodeRoot.position.x, nodeRoot.position.y);
    }
}
