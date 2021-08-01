using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PopUpFillNodeDropDownModule : MonoBehaviour, PopUpFillNodeInputInterface
{
    public List<TMP_Dropdown> dropdowns = new List<TMP_Dropdown>();
    private List<object> inputDropDowns = new List<object>();
    public List<object> Inputs { get => inputDropDowns; }

    private void Awake()
    {
        for (int i = 0; i < dropdowns.Count; i++)
        {
            inputDropDowns.Add(dropdowns[i]);
        }
    }

    public bool Validate()
    {
        bool isValid = true;
        foreach (TMP_Dropdown dropdown in dropdowns)
        {
            if(dropdown.value == 0)
            {
                isValid = false;
                break;
            }
        }
        return isValid;
    }

    public void SetInputsContent(string[] content)
    {
        for (int i = 0; i < content.Length; i++)
        {
            DropDownItemDefiner dropDownItemDefiner = DropDownItemDefiner.FromJson(content[i]);
            List<string> options = new List<string>();
            options.Add(dropDownItemDefiner.placeHolder);
            options.AddRange(dropDownItemDefiner.items);
            dropdowns[i].AddOptions(options);
            dropdowns[i].value = dropDownItemDefiner.dropDownValue;
        }
    }

    [Serializable]
    public class DropDownItemDefiner
    {
        [SerializeField]
        public string placeHolder;

        [SerializeField]
        public string[] items;

        [SerializeField]
        public int dropDownValue;

        public DropDownItemDefiner(string placeHolder, string[] items, int dropDownValue)
        {
            this.placeHolder = placeHolder;
            this.items = items;
            this.dropDownValue = dropDownValue;
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public static DropDownItemDefiner FromJson(string json)
        {
            return JsonUtility.FromJson<DropDownItemDefiner>(json);
        }
    }
}
