using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CursorManager : MonoBehaviour
{
    public List<CursorDef> cursors = new List<CursorDef>();

    public static CursorManager instance;

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

    public void ChangeCursor(string curName)
    {
        CursorDef def = cursors.Find(x => x.curName == curName);
        Cursor.SetCursor(def.curTexture, def.hotSpot, CursorMode.Auto);
    }
}
