using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FpsCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    private void Start()
    {
        if(Debug.isDebugBuild)
            StartCoroutine("CountFps");
    }

    IEnumerator CountFps()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            float fps = 1 / Time.unscaledDeltaTime;
            text.text = fps.ToString("f2") + " fps";
        }
    }
}
