using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ToggleScript : MonoBehaviour
{
    public GameObject handle;
    private RectTransform handleRectTransform;
    public GameObject fillArea;
    public Color fillAreaColor;

    public UnityEvent OnCheckChanged;

    private bool value;
    public bool Value { get => value; }

    private void Start()
    {
        handleRectTransform = handle.GetComponent<RectTransform>();
        fillArea.GetComponent<Image>().color = fillAreaColor;
        value = false;
    }

    public void CheckChanged()
    {
        value = !value;
        if (value)
        {
            fillArea.SetActive(true);
            handleRectTransform.localPosition = new Vector3(0, 0, 0);
        }
        else
        {
            fillArea.SetActive(false);
            handleRectTransform.localPosition = new Vector3(-30, 0, 0);
        }
        OnCheckChanged?.Invoke();
    }

}
