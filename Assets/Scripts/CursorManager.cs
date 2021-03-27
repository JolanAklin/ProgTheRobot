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

public class CursorManager : MonoBehaviour
{
    public List<CursorDef> cursors = new List<CursorDef>();

    public static CursorManager instance;

    private bool lockCurTexture = false;
    private string currentCurName = "";

    [Serializable]
    public class CursorDef
    {
        public string curName;
        public Texture2D curTexture;
        public Vector2 hotSpot;
    }

    private void Awake()
    {
        instance = this;
        ChangeCursor("default");
    }

    public void ChangeCursor(string curName, bool lockCurTexture = false)
    {
        if(!this.lockCurTexture && currentCurName != curName)
        {
            this.lockCurTexture = lockCurTexture;
            currentCurName = curName;
            CursorDef def = cursors.Find(x => x.curName == curName);
            if(def != null)
                Cursor.SetCursor(def.curTexture, def.hotSpot, CursorMode.Auto);
        }
    }

    public void UnLockCursorTexture()
    {
        lockCurTexture = false;
    }
}
