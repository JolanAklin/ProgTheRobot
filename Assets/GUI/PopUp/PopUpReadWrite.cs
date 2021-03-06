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
using TMPro;
using System;
using UnityEngine.UI;

public class PopUpReadWrite : PopUp
{
    private Action okAction;

    public TMP_Text infoText;
    public TMP_InputField inputField;

    public void Init(string text, string number, bool isReading)
    {
        int iNumber;
        if(int.TryParse(number, out iNumber))
        {
            inputField.text = iNumber.ToString();
        }
        else
        {
            inputField.text = "Undefined";
        }
        infoText.text = text;
        if(isReading)
        {
            inputField.interactable = true;
            inputField.Select();
        }
        else
        {
            inputField.interactable = false;
        }

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
