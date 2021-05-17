// Copyright 2021 Jolan Aklin

//This file is part of Prog The Robot.

//Prog The Robot is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 3 of the License.

//Prog The Robot is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Prog the robot.  If not, see<https://www.gnu.org/licenses/>.

using UnityEngine;
using Language;
using TMPro;
using System;


//from there http://www.demonixis.net/ajout-du-multilingue-dans-votre-jeux-avec-unity-3d/
[RequireComponent(typeof(TMP_Text))]
public class TranslateText : MonoBehaviour
{
    public string translationKey;
    public bool translateAtStart = true;


    private void Start()
    {
        Manager.instance.OnLanguageChanged += ChangeLanguage;
        if (translateAtStart)
            ChangeLanguage(this, EventArgs.Empty);
    }

    private void ChangeLanguage(object sender, EventArgs e)
    {
        TMP_Text text = GetComponent<TMP_Text>();
        text.text = Translation.Get(translationKey);
    }

    private void OnDestroy()
    {
        Manager.instance.OnLanguageChanged -= ChangeLanguage;
    }
}