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

    [Header("Images")]
    public Sprite plusSign;
    public Sprite minusSign;

    [Header("Colors")]
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
    /// Hide and show it's content
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
