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

    // hold the data to reset the robot to its original position
    public Vector3 robotStartPos;
    public Quaternion robotStartRot;


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
}
