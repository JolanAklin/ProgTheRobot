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

public class Ball : TerrainInteractableObj
{
    public GameObject parent;
    public Transform ballObjectDefaultPos;
    public GameObject ballObject;
    public ObjectPlacement DefaultObjectPlacement;
    public ObjectPlacement currentObjectPlacement;
    public bool ballTaken;
    public LayerMask objectLayer;

    

    public void GetObjectPlacement()
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), transform.TransformDirection(Vector3.down), out hit, 1, objectLayer))
        {
            hit.collider.TryGetComponent(out DefaultObjectPlacement);
            currentObjectPlacement = DefaultObjectPlacement;
        }
    }

    public void BallReset()
    {
        parent.transform.position = DefaultObjectPlacement.transform.position;
        parent.transform.rotation = DefaultObjectPlacement.transform.rotation;
        ballTaken = false;

        ballObject.transform.position = ballObjectDefaultPos.position;
        ballObject.transform.rotation = ballObjectDefaultPos.rotation;


        currentObjectPlacement.tag = "Untagged";
        currentObjectPlacement.terrainObject = null;

        DefaultObjectPlacement.tag = "PlacementOccupied";
        DefaultObjectPlacement.terrainObject = gameObject;
        ballObject.GetComponent<Rigidbody>().isKinematic = true;
    }
}
