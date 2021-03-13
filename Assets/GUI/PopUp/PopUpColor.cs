using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PopUpColor : MonoBehaviour
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
