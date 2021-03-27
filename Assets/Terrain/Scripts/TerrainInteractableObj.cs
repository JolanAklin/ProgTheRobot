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

public class TerrainInteractableObj : MonoBehaviour
{
    public enum ObjectType
    {
        none = 0,
        PowerPlug,
        Marking,
        ball
    }

    public ObjectType type;

    private List<Material[]> defaultMat = new List<Material[]>();
    public Renderer[] objectRenderers;
    public Material placeHolderMat;

    public void Awake()
    {
        foreach (Renderer renderer in objectRenderers)
        {
            defaultMat.Add(renderer.materials);
        }
    }

    public void PlaceHolderMat()
    {
        foreach (Renderer renderer in objectRenderers)
        {
            Material[] mats = new Material[renderer.materials.Length];
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                mats[i] = placeHolderMat;
            }
            renderer.materials = mats;
        }
    }

    public void PlacedMat()
    {
        int i = 0;
        foreach (Renderer renderer in objectRenderers)
        {
            renderer.materials = defaultMat[i];
            i++;
        }
    }
}
