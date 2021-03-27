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

public class Debugger : MonoBehaviour
{

    private static List debugList;


    private void Awake()
    {
        debugList = GetComponent<List>();
    }

    public static void Log(string text)
    {
        debugList.AddChoice(new List.ListElement() { displayedText = text });
    }
    public static void LogError(string text)
    {
        debugList.AddChoice(new List.ListElement() { displayedText = text });
    }

    public static void ClearDebug()
    {
        debugList.Clear();
    }
}
