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

public class CameraController : MonoBehaviour
{
    // from https://pastebin.com/YBbFGZzD, modified by me
    public float mouseSensitivity = 250;
    public float moveSpeed;

    private float verticalLookRotation;
    private float horizontalLookRotation;

    private bool inMove;

    public Transform defaultCamPosRot;

    private void Start()
    {
        SetDefaultPosRot();
    }

    private void Update()
    {
        if (Input.GetMouseButton(2))
        {
            if(Manager.instance.canTerrainCamMove || inMove)
            {
                inMove = true;
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    Vector3 moveDir = new Vector3(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"), 0).normalized;
                    Vector3 targetMoveAmount = moveDir * moveSpeed * Time.deltaTime;
                    Vector3 localMove = transform.TransformDirection(targetMoveAmount) * Time.fixedDeltaTime;
                    transform.Translate(transform.InverseTransformDirection(localMove));
                }
                else
                {
                    horizontalLookRotation += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

                    verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
                    verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90, 0);

                    transform.localEulerAngles = new Vector3(-1 * verticalLookRotation, 1 * horizontalLookRotation, 0);
                }
            }
        }else
        {
            inMove = false;
        }
        if(Input.mouseScrollDelta.y != 0 && Manager.instance.canTerrainCamMove && !inMove)
        {
            transform.Translate(Vector3.forward * Input.mouseScrollDelta.y * moveSpeed * Time.deltaTime / 4);
        }
    }

    // start tpi
    /// <summary>
    /// Set the default position and rotation
    /// </summary>
    public void SetDefaultPosRot()
    {
        transform.localPosition = defaultCamPosRot.localPosition;
        transform.rotation = defaultCamPosRot.rotation;
        verticalLookRotation = 0;
        horizontalLookRotation = 0;
    }
    // end tpi
}
