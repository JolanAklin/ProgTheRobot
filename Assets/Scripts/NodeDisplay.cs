using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeDisplay : MonoBehaviour
{
    public static NodeDisplay instance;

    public RenderTexture rt;
    public Camera nodeCamera;
    private RectTransform scriptPanelParent;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
        scriptPanelParent = GameObject.FindGameObjectWithTag("ScriptPanelParent").GetComponent<RectTransform>();
    }

    private void FixedUpdate()
    {
        OnScriptPanelResize();
    }

    public void OnScriptPanelResize()
    {
        nodeCamera.targetTexture.Release();
        nodeCamera.enabled = false;
        rt.width = (int)scriptPanelParent.rect.width;
        rt.height = (int)scriptPanelParent.rect.height;
        nodeCamera.enabled = true;
        nodeCamera.targetTexture = rt;
    }
}
