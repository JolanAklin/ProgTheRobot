using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class InfoBar : MonoBehaviour
{
    [SerializeField]
    public List<infoObject> infos = new List<infoObject>();

    [Serializable]
    public class infoObject
    {
        public string infoName;
        public GameObject prefab;
    }
    private string currentInfo;

    public void ChangeInfos(string info)
    {
        if(currentInfo != info)
        {
            currentInfo = info;
            foreach (Transform child in this.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject infoPrefab = infos.Find(x => x.infoName == info).prefab;
            if(infoPrefab != null)
                Instantiate(infoPrefab, this.transform);
        }
    }
}
