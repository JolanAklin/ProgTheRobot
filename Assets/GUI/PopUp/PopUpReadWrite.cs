// Copyright 2021 Jolan Aklin

//This file is part of Prog the robot.

//Prog the robot is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 3 of the License.

//FileTeleporter is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Prog the robot.  If not, see<https://www.gnu.org/licenses/>.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class PopUpReadWrite : MonoBehaviour
{
    private Action okAction;

    public TMP_Text infoText;
    public TMP_InputField inputField;

    public void Init(string text)
    {
        infoText.text = text;
        inputField.interactable = true;
    }
    public void Init(string text, string number)
    {
        infoText.text = text;
        inputField.text = number;
        inputField.interactable = false;
    }

    public void DestroyPopup()
    {
        Destroy(this.gameObject);
    }

    public int value()
    {
        int value;
        if(int.TryParse(inputField.text, out value))
        {
            return value;
        }
        return 0;
    }

    public void CheckValue()
    {
        int temp;
        if(!int.TryParse(inputField.text, out temp))
        {
            infoText.text = "Entrer seulement des nombres";
        }
    }

    public void SetOkAction(Action action)
    {
        okAction = action;
    }

    public void Ok()
    {
        okAction();
    }
}
