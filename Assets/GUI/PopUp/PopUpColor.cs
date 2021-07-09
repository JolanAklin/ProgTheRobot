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
using System;

public class PopUpColor : PopUp
{

    public Image showColor;
    [HideInInspector]
    public Color color;
    public Slider[] colorSliders;
    public Slider sliderRed;
    public Slider sliderGreen;
    public Slider sliderBlue;
    public TMP_Text textRed;
    public TMP_Text textGreen;
    public TMP_Text textBlue;

    // delegate called when the button are clicked
    private Action cancelAction;
    private Action OkAction;

    // set the color and the slider and the text
    public void Init(Color color)
    {
        this.color = color;
        this.color.a = 1;
        textRed.text = Math.Round((255f * color.r), 0).ToString();
        textGreen.text = Math.Round((255f * color.g), 0).ToString();
        textBlue.text = Math.Round((255f * color.b), 0).ToString();
        sliderRed.value = color.r;
        sliderGreen.value = color.g;
        sliderBlue.value = color.b;
        ShowNewColor();
    }

    public void ChangeRed(Slider slider)
    {
        color.r = slider.value;
        textRed.text = Math.Round((255f * color.r), 0).ToString();
        ShowNewColor();
    }
    public void ChangeGreen(Slider slider)
    {
        color.g = slider.value;
        textGreen.text = Math.Round((255f * color.g), 0).ToString();
        ShowNewColor();
    }
    public void ChangeBlue(Slider slider)
    {
        color.b = slider.value;
        textBlue.text = Math.Round((255f * color.b), 0).ToString();
        ShowNewColor();
    }

    private void ShowNewColor()
    {
        showColor.color = color;
    }

    // set the delegate called when a button is clicked
    public void SetButtonOk(Action action)
    {
        OkAction = action;
    }
    public void SetButtonCancel(Action action)
    {
        cancelAction = action;
    }

    // when button is pressed;
    public void Cancel()
    {
        cancelAction();
    }
    public void Ok()
    {
        OkAction();
    }
}
