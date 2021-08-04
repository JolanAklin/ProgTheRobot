using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class MenuToolTip : ToolTip
{
    public static MenuToolTip instance { get; private set; }

    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private TMP_Text menuTitleText;

    private new void Awake()
    {
        base.Awake();
        instance = this;
        HideToolTip();
    }

    public void SetContent(string menuTitle, ButtonContent[] buttonsContent)
    {
        menuTitleText.SetText(menuTitle);
        menuTitleText.ForceMeshUpdate();
        for (int i = 1; i < bgRectTransform.transform.childCount; i++)
        {
            Destroy(bgRectTransform.transform.GetChild(i).gameObject);
        }
        foreach (ButtonContent content in buttonsContent)
        {
            GameObject instance = Instantiate(buttonPrefab, bgRectTransform);
            instance.transform.GetChild(0).GetComponent<TMP_Text>().SetText(content.content);
            instance.GetComponent<Button>().onClick.AddListener(() => { content.behaviour(); });
        }
    }

    public class ButtonContent
    {
        public string content { get; private set; }
        public Action behaviour { get; private set; }

        public ButtonContent(string content, Action behaviour)
        {
            this.content = content;
            this.behaviour = behaviour;
        }
    }
}
