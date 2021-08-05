// Copyright 2021 Jolan Aklin

//This file is part of Prog The Robot.

//Prog The Robot is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 3 of the License.

//Prog The Robot is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Prog the robot.  If not, see<https://www.gnu.org/licenses/>.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class InfoBar : MonoBehaviour
{
    public static InfoBar instance { get; private set; }
    [SerializeField]
    public List<infoObject> infos = new List<infoObject>();

    [Serializable]
    public class infoObject
    {
        public enum InfoType
        {
            scriptInfo,
            robotSmallWindow,
            robotBigWindow,
            none,
        }
        public InfoType infoType;
        public GameObject prefab;
    }
    public infoObject.InfoType currentInfo { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    public void ChangeInfos(infoObject.InfoType infoType)
    {
        if(currentInfo != infoType)
        {
            currentInfo = infoType;
            foreach (Transform child in this.transform)
            {
                Destroy(child.gameObject);
            }
            if(infoType != infoObject.InfoType.none)
            {
                GameObject infoPrefab = infos.Find(x => x.infoType == infoType).prefab;
                if(infoPrefab != null)
                    Instantiate(infoPrefab, this.transform);
            }     
        }
    }
}
