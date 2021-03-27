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
