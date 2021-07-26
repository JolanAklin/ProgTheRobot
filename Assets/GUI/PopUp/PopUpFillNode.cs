using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using F23.StringSimilarity;

public class PopUpFillNode : PopUp
{
    public Action cancelAction;
    public Action OkAction;

    public void Cancel()
    {
        cancelAction();
    }
    public void Ok()
    {
        OkAction();
    }
}