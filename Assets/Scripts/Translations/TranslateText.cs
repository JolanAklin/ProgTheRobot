using UnityEngine;
using Language;
using TMPro;
using System;


//from there http://www.demonixis.net/ajout-du-multilingue-dans-votre-jeux-avec-unity-3d/
[RequireComponent(typeof(TMP_Text))]
public class TranslateText : MonoBehaviour
{
    public string translationKey;

    private void Start()
    {
        Manager.instance.OnLanguageChanged += ChangeLanguage;
    }

    private void ChangeLanguage(object sender, EventArgs e)
    {
        TMP_Text text = GetComponent<TMP_Text>();
        text.text = Translation.Get(translationKey);
    }
}