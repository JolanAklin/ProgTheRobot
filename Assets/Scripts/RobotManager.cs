using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RobotManager : MonoBehaviour
{

    public float execSpeed;

    private Action actionOnUpdate;
    private Action callBack;

    private Vector3 targetPos;
    private Vector3 targetRotation;

    private void Start()
    {
        //TurnLeft();
    }

    public void GoForward(Action callBack)
    {
        this.callBack = callBack;
        GoForward();
    }

    private void GoForward()
    {
        if(actionOnUpdate == null)
        {
            actionOnUpdate = GoForward;
            targetPos = transform.position + transform.forward;
        }
        transform.position += transform.forward * Time.deltaTime * execSpeed;
        if(Mathf.Abs(targetPos.x) - Mathf.Abs(transform.position.x) < Vector3.zero.x || Mathf.Abs(targetPos.y) - Mathf.Abs(transform.position.y) < Vector3.zero.y || Mathf.Abs(targetPos.z) - Mathf.Abs(transform.position.z) < Vector3.zero.z)
        {
            transform.position = targetPos;
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
            targetRotation = transform.rotation.eulerAngles + new Vector3(0, -90, 0);
        }
        transform.Rotate(-Vector3.up * execSpeed * Time.deltaTime * 40);
        if (targetRotation.y - transform.rotation.eulerAngles.y > 0)
        {
            transform.rotation = Quaternion.Euler(targetRotation);
            actionOnUpdate = null;
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
            targetRotation = transform.rotation.eulerAngles + new Vector3(0,90,0);
        }
        transform.Rotate(Vector3.up * execSpeed * Time.deltaTime * 40);
        Vector3 robotRot = transform.rotation.eulerAngles;
        if(robotRot.y == 0)
        {
            robotRot = new Vector3(0, 360, 0);
        }
        if (robotRot.y - targetRotation.y > 0)
        {
            transform.rotation = Quaternion.Euler(targetRotation);
            actionOnUpdate = null;
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
}
