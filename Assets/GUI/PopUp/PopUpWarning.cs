// Copyright 2021 Jolan Aklin

//This file is part of Prog the robot.

//Prog the robot is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 3 of the License.

//Prog the robot is distributed in the hope that it will be useful,
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
using UnityEngine.UI;

public class PopUpWarning : MonoBehaviour
{
    private Action quitAction;
    private Action cancelAction;
    private Action saveAction;

    public TMP_Text warningText;
    public Button quitButton;
    public Button saveButton;

    public void Close()
    {
        Destroy(this.gameObject);
    }

    #region buttons action
    public void SetQuitAction(Action action)
    {
        quitAction = action;
    }
    public void SetCancelAction(Action action)
    {
        cancelAction = action;
    }
    public void SetSaveAction(Action action)
    {
        saveAction = action;
    }

    public void Quit()
    {
        quitAction();
    }
    public void Cancel()
    {
        cancelAction();
    }
    public void save()
    {
        saveAction();
    }
    #endregion
}
