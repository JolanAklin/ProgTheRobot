using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class PopUpWait : MonoBehaviour
{
    public TMP_Text text;
    private Action finishLoadAction;
    public void init(string info, Action finishLoadAction)
    {
        text.text = info;
        this.finishLoadAction = finishLoadAction;
    }

    public void Close()
    {
        Destroy(this.gameObject);
    }

    private void Start()
    {
        finishLoadAction?.Invoke();
    }

    public void SetFinishedLoadAction(Action action)
    {
        finishLoadAction = action;
    }
}
