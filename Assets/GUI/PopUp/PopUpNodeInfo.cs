using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUpNodeInfo : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text text;
    public void init(string title, string info)
    {
        titleText.text = title;
        text.text = info;
    }

    public void Close()
    {
        Destroy(this.gameObject);
    }
}
