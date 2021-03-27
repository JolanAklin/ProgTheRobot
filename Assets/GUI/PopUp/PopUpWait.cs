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
