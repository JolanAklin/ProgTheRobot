using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RobotManager : MonoBehaviour
{
    private Action actionOnUpdate; // hold the action (like turning), it while be executed on Update
    private Action callBack; // call the delegate when the movement (like turning) stops

    private Vector3 startMovementPos; // the pos of the robot a the start of the movement
    private Quaternion startMovementRot; // the rotation of the robot at the start of the rotation

    public Renderer[] renderers;

    // hold the data to reset the robot to its original position
    public Vector3 robotStartPos;
    public Quaternion robotStartRot;

    public LayerMask wallLayer;
    public LayerMask objectLayer;


    private float t; // interpolation factor

    private void Start()
    {
        SetDefaultPos(this.transform);
    }

    public void SetDefaultPos(Transform transform)
    {
        robotStartPos = transform.position;
        robotStartRot = transform.rotation;
    }

    public void SetRobotColor(Color color)
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = color;
        }
    }

    public void SetDefaultPos(Vector3 position)
    {
        robotStartPos = position;
    }

    public void SetDefaultPos(Quaternion rotation)
    {
        robotStartRot = rotation;
    }

    public void GoForward(Action callBack)
    {
        this.callBack = callBack;
        GoForward();
    }

    private void GoForward()
    {
        if(!WallInFront())
        {
            if (actionOnUpdate == null)
            {
                actionOnUpdate = GoForward;
                startMovementPos = transform.position;
                t = 0f;
            }
            t += 1 * Time.deltaTime * Manager.instance.execSpeed;
            transform.position = Vector3.Lerp(startMovementPos, startMovementPos + transform.forward, t);
            if(t > 1)
            {
                actionOnUpdate = null;
                callBack();
            }
        }else
        {
            Debugger.Log($"Le robot est bloqué");
            callBack();
        }
    }

    public void TurnRight(Action callBack)
    {
        this.callBack = callBack;
        TurnRight();
    }

    private void TurnRight()
    {
        if (actionOnUpdate == null)
        {
            actionOnUpdate = TurnRight;
            startMovementRot = transform.rotation;
            t = 0f;
        }
        t += 1 * Time.deltaTime * Manager.instance.execSpeed;
        transform.rotation = Quaternion.Lerp(startMovementRot, startMovementRot * Quaternion.Euler(0, 90, 0), t);
        if (t > 1)
        {
            actionOnUpdate = null;
            callBack();
        }
    }

    public void TurnLeft(Action callBack)
    {
        this.callBack = callBack;
        TurnLeft();
    }

    private void TurnLeft()
    {
        if (actionOnUpdate == null)
        {
            actionOnUpdate = TurnLeft;
            startMovementRot = transform.rotation;
            t = 0f;
        }
        t += 1 * Time.deltaTime * Manager.instance.execSpeed;
        transform.rotation = Quaternion.Lerp(startMovementRot, startMovementRot * Quaternion.Euler(0, -90, 0), t);
        if (t > 1)
        {
            actionOnUpdate = null;
            callBack();
        }
    }

    public void Mark()
    {

    }

    public void Unmark()
    {

    }

    public void Reload()
    {

    }

    public void TakeBall()
    {

    }

    public void PlaceBall()
    {

    }

    public void ThrowBall()
    {

    }

    public void Update()
    {
        if (actionOnUpdate != null)
            actionOnUpdate();
    }

    public void DestroyRobotManager()
    {
        Destroy(this.gameObject);
    }

    public bool WallInFront()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 1, wallLayer))
        {
            return true;
        }
        return false;
    }
    public bool WallLeft()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, 1, wallLayer))
        {
            return true;
        }
        return false;
    }
    public bool WallRight()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, 1, wallLayer))
        {
            return true;
        }
        return false;
    }

    public bool IsOut()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1, objectLayer))
        {
            if(hit.collider.gameObject.tag == "EndFlag")
                return true;
        }
        return false;
    }

    public bool IsOnAnOutlet()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1, objectLayer))
        {
            if (hit.collider.gameObject.tag == "Outlet")
                return true;
        }
        return false;
    }

    public bool IsCaseMarked()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1, objectLayer))
        {
            if (hit.collider.gameObject.tag == "Marking")
                return true;
        }
        return false;
    }
}
