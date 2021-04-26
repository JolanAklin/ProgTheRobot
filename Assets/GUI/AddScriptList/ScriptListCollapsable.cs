using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScriptListCollapsable : MonoBehaviour
{
    // start tpi
    private Image panelImage;
    public TMP_Text nbScripts;
    public Image buttonImage;
    public Sprite plusSign;
    public Sprite minusSign;

    public Color collapsedColor;
    public Color normalColor;

    public List<GameObject> objectToHide;

    private bool isCollapsed = false;

    private void Awake()
    {
        panelImage = GetComponent<Image>();
    }

    private void Start()
    {
        nbScripts.text = "Nb organigrammes : " + objectToHide.Count;
    }

    /// <summary>
    /// Hide its content
    /// </summary>
    public void Collapse()
    {
        isCollapsed = !isCollapsed;
        if (isCollapsed)
        {
            buttonImage.sprite = plusSign;
            panelImage.color = collapsedColor;
        }
        else
        {
            buttonImage.sprite = minusSign;
            panelImage.color = normalColor;
        }
        foreach (GameObject gameObject in objectToHide)
        {
            gameObject.SetActive(!isCollapsed);
        }
    }

    // end tpi
}
