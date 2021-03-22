using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptCollider : MonoBehaviour
{

    public BoxCollider2D boxCollider2d;
    public RectTransform rectTransform;
    public RectTransform canvas;
    public float resizeFactor = 100;

    public void Resize()
    {
        boxCollider2d.size = new Vector2(rectTransform.rect.width/resizeFactor, rectTransform.rect.height/resizeFactor);
        if(canvas != null)
            if (rectTransform.anchorMax.y == 1)
                boxCollider2d.offset = new Vector2(0, canvas.rect.height / (resizeFactor * 2) - rectTransform.rect.height / (resizeFactor * 2));
            else
                boxCollider2d.offset = new Vector2(0, -canvas.rect.height / (resizeFactor * 2) + rectTransform.rect.height / (resizeFactor * 2));
    }
}
