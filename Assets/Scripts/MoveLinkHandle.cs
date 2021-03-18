using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLinkHandle : MonoBehaviour
{
    public GameObject SplineLink;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2d;

    public void Start()
    {
        boxCollider2d = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Manager.instance.OnSpline += ShowHide;
    }

    private void OnDestroy()
    {
        Manager.instance.OnSpline -= ShowHide;
    }

    public void ShowHide(object sender, Manager.OnSplineEventArgs e)
    {

        if (!e.splineStarted)
        {
            spriteRenderer.enabled = true;
            boxCollider2d.enabled = true;
        }
        else
        {
            spriteRenderer.enabled = false;
            boxCollider2d.enabled = false;
        }

    }
}
