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
