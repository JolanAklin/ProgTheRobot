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

public class RobotManager : MonoBehaviour
{
    private Action actionOnUpdate; // hold the action (like turning), it while be executed on Update
    private Action carryBall;
    private Ball ball;
    private Action callBack; // call the delegate when the movement (like turning) stops
    private Action noPower;

    private Vector3 startMovementPos; // the pos of the robot a the start of the movement
    private Quaternion startMovementRot; // the rotation of the robot at the start of the rotation

    [HideInInspector]
    public Robot robot;

    public Renderer[] renderers;

    public Renderer bodyRenderer;

    // start tpi
    [Header("Camera follow point")]
    public GameObject cameraPoint;
    public Vector3 defaultCameraPointPos;

    // make robot not pass in each other
    private Vector3Int posOnGrid;
    public Vector3 PosOnGrid { get => posOnGrid; set { posOnGrid = ConvertToVector3Int(value); } }
    public Vector3Int PosOnGridInt { get => posOnGrid; set => posOnGrid = value; }

    private Vector3Int nextPosOnGrid;
    public Vector3 NextPosOnGrid { get => nextPosOnGrid; set { nextPosOnGrid = ConvertToVector3Int(value); } }
    public Vector3Int NextPosOnGridInt { get => nextPosOnGrid; set => nextPosOnGrid = value; }

    // end tpi

    // hold the data to reset the robot to its original position
    public Vector3 robotStartPos;
    public Quaternion robotStartRot;

    public LayerMask wallLayer;
    public LayerMask objectLayer;
    public LayerMask objectPlacement;

    public uint goForwardPower = 20;
    public uint turnPower = 15;
    public uint otherActionPower = 10;

    public Transform ballCarryPoint;
    public float ballThrowForce;

    public GameObject markingPrefab;

    private List<GameObject> markings = new List<GameObject>();
    private List<Ball> ballsToReset = new List<Ball>();
    [HideInInspector]
    public List<Ball> balls = new List<Ball>();
    private float t; // interpolation factor

    private void Start()
    {
        SetDefaultPos(this.transform);
        PosOnGrid = transform.position;
        NextPosOnGrid = transform.position;
    }

    public void SetDefaultPos(Transform transform)
    {
        robotStartPos = transform.position;
        robotStartRot = transform.rotation;
    }

    // start tpi

    /// <summary>
    /// Show the battery usage on the robot
    /// </summary>
    /// <param name="value">Amout of battery left between 0 and 1. (remaining/total)</param>
    public void ShowBatteryUsage(float value)
    {
        value = Mathf.Clamp01(value);
        bodyRenderer.material.SetFloat("amount", value);
    }
    // end tpi

    public void SetRobotColor(Color color)
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = color;
        }
    }

    public void ResetTerrainObj()
    {
        RemoveMarks();
        ResetBalls();
        GetBallOnTerrain();
    }

    public void GetBallOnTerrain()
    {
        balls.Clear();
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Ball"))
        {
            balls.Add(gameObject.GetComponent<Ball>());
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

    public void RemoveMarks()
    {
        foreach (GameObject gameObject in markings)
        {
            Destroy(gameObject);
        }
    }

    public void ResetBalls()
    {
        carryBall = null;
        this.ball = null;
        foreach (Ball ball in ballsToReset)
        {
            ball.BallReset();
        }
        ballsToReset.Clear();
    }

    public void GoForward(Action callBack, Action noPower)
    {
        if (!WallInFront())
        {
            if (robot.Power >= goForwardPower)
            {
                // start tpi
                PosOnGrid = transform.position;
                switch (Mathf.RoundToInt(transform.rotation.eulerAngles.y))
                {
                    case 0:
                        nextPosOnGrid = new Vector3Int(posOnGrid.x, posOnGrid.y, posOnGrid.z + 1);
                        break;
                    case 180:
                        nextPosOnGrid = new Vector3Int(posOnGrid.x, posOnGrid.y, posOnGrid.z - 1);
                        break;
                    case 90:
                        nextPosOnGrid = new Vector3Int(posOnGrid.x + 1, posOnGrid.y, posOnGrid.z);
                        break;
                    case 270:
                        nextPosOnGrid = new Vector3Int(posOnGrid.x - 1, posOnGrid.y, posOnGrid.z);
                        break;
                }
                foreach (Robot robot in Robot.robots.Values)
                {
                    if(nextPosOnGrid == robot.robotManager.nextPosOnGrid && robot.robotManager != this)
                    {
                        nextPosOnGrid = posOnGrid;
                        Debugger.Log($"Le robot {robot.robotName} est bloqué");
                        callBack();
                        return;
                    }
                }
                // end tpi
                this.noPower = noPower;
                this.callBack = callBack;
                GoForward();
            }
            else
            {
                noPower();
            }
        }
        else
        {
            Debugger.Log($"Le robot {robot.robotName} est bloqué");
            callBack();
        }
    }

    private void GoForward()
    {
        
        if (actionOnUpdate == null)
        {
            actionOnUpdate = GoForward;
            startMovementPos = transform.position;
            robot.Power -= goForwardPower;
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

    public void TurnRight(Action callBack, Action noPower)
    {
        if (robot.Power >= turnPower)
        {
            this.noPower = noPower;
            this.callBack = callBack;
            TurnRight();
        }
        else
        {
            noPower();
        }
    }

    private void TurnRight()
    {
        if (actionOnUpdate == null)
        {
            actionOnUpdate = TurnRight;
            startMovementRot = transform.rotation;
            robot.Power -= turnPower;
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

    public void TurnLeft(Action callBack, Action noPower)
    {
        if (robot.Power >= turnPower)
        {
            this.noPower = noPower;
            this.callBack = callBack;
            TurnLeft();
        }
        else
        {
            noPower();
        }
    }

    private void TurnLeft()
    {
        if (actionOnUpdate == null)
        {
            actionOnUpdate = TurnLeft;
            startMovementRot = transform.rotation;
            robot.Power -= turnPower;
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

    public void Mark(Action callBack, Action noPower)
    {
        if(!IsCaseMarked())
        {
            if (robot.Power >= turnPower)
            {
                this.noPower = noPower;
                this.callBack = callBack;
                robot.Power -= otherActionPower;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1, objectPlacement))
                {
                    markings.Add(Instantiate(markingPrefab, hit.transform.position, Quaternion.identity));
                }
                else
                {
                    Debugger.Log("Impossible de marquer la case");
                }
                callBack?.Invoke();
            }
            else
            {
                noPower();
            }
        }
        else
        {
            Debugger.Log("Impossible de marquer la case");
            callBack?.Invoke();
        }
    }

    public void Unmark(Action callBack, Action noPower)
    {
        if (IsCaseMarked())
        {
            if (robot.Power >= turnPower)
            {
                this.noPower = noPower;
                this.callBack = callBack;
                robot.Power -= otherActionPower;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1, objectLayer))
                {
                    Marking marking;
                    if (hit.collider.gameObject.TryGetComponent(out marking))
                    {
                        if (marking.type == TerrainInteractableObj.ObjectType.Marking)
                        {
                            markings.Remove(marking.parent.gameObject);
                            Destroy(marking.parent.gameObject);
                        }
                    }
                }
                else
                {
                    Debugger.LogError("Impossible de démarquer la case");
                }
                callBack?.Invoke();
            }
            else
            {
                noPower();
            }
        }
        else
        {
            Debugger.LogError("Impossible de démarquer la case");
            callBack?.Invoke();
        }
    }

    public void Charge(Action callBack)
    {
        this.callBack = callBack;
        Charge();
    }

    private void Charge()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1, objectLayer))
        {
            TerrainInteractableObj powerOutlet;
            if (hit.collider.gameObject.TryGetComponent(out powerOutlet))
            {
                if(powerOutlet.type == TerrainInteractableObj.ObjectType.PowerPlug)
                {
                    IEnumerator coroutine = Charge((PowerOutlet)powerOutlet);
                    StartCoroutine(coroutine);
                    return;
                }
            }
        }
        Debugger.Log("Impossible de charger, le robot n'est pas sur une prise");
        callBack?.Invoke();
    }

    private IEnumerator Charge(PowerOutlet powerOutlet)
    {
        powerOutlet.StartParticleSystem();
        while(robot.Power < robot.defaultPower)
        {
            if(robot.Power + powerOutlet.PowerPerTick <= robot.defaultPower)
                robot.Power += powerOutlet.PowerPerTick;
            else
                robot.Power = robot.defaultPower;
            yield return new WaitForSeconds(3/Manager.instance.execSpeed);
        }
        powerOutlet.StopParticleSystem();
        callBack?.Invoke();
    }

    public void TakeBall(Action callBack, Action noPower)
    {
        if (robot.Power >= turnPower)
        {
            this.noPower = noPower;
            this.callBack = callBack;
            robot.Power -= otherActionPower;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1, objectLayer))
            {
                TerrainInteractableObj tio;
                if (hit.collider.gameObject.TryGetComponent(out tio))
                {
                    if (tio.type == TerrainInteractableObj.ObjectType.ball)
                    {
                        Ball tempBall = (Ball)tio;
                        if(!tempBall.ballTaken)
                        {
                            ball = tempBall;
                            ballsToReset.Add(ball);
                            ball.ballTaken = true;
                            ball.GetObjectPlacement();

                            ball.currentObjectPlacement.tag = "Untagged";
                            ball.currentObjectPlacement.terrainObject = null;

                            carryBall = () =>
                            {
                                ball.ballObject.transform.position = ballCarryPoint.position;
                            };
                        }
                        else
                        {
                            Debugger.LogError("Impossible de prendre le ballon");
                        }
                    }
                }
            }
            else
            {
                Debugger.LogError("Il n'y a pas de ballon sur cette case");
            }
            callBack?.Invoke();
        }
        else
        {
            noPower?.Invoke();
        }
    }

    public void PlaceBall(Action callBack, Action noPower)
    {
        if (robot.Power >= turnPower)
        {
            this.noPower = noPower;
            this.callBack = callBack;
            robot.Power -= otherActionPower;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1, objectPlacement))
            {
                if(hit.collider.gameObject.tag != "PlacementOccupied")
                {
                    ObjectPlacement objectPlacement;
                    if (hit.collider.gameObject.TryGetComponent(out objectPlacement) && ball != null)
                    {
                        ball.currentObjectPlacement = objectPlacement;

                        objectPlacement.tag = "PlacementOccupied";
                        objectPlacement.terrainObject = ball.gameObject;

                        ball.ballTaken = false;

                        carryBall = null;

                        ball.parent.transform.position = objectPlacement.transform.position;
                        ball.parent.transform.rotation = objectPlacement.transform.rotation;

                        ball.ballObject.transform.position = ball.ballObjectDefaultPos.position;
                        ball.ballObject.transform.rotation = ball.ballObjectDefaultPos.rotation;

                        ball = null;
                    }
                    else
                    {
                        Debugger.LogError("Il n'y a pas de ballon à placer");
                    }
                }
                else
                {
                    Debugger.LogError("Cet emplacement est déjà occupé");
                }
            }
            else
            {
                Debugger.LogError("Le ballon ne peut pas être posé ici");
            }
            callBack?.Invoke();
        }
    }

    public void ThrowBall(Action callBack, Action noPower)
    {
        if (robot.Power >= turnPower)
        {
            this.noPower = noPower;
            this.callBack = callBack;
            robot.Power -= otherActionPower;

            if(ball != null)
            {
                carryBall = null;
                Rigidbody rb = ball.ballObject.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.AddForce(transform.TransformDirection(new Vector3(0, 290, 100)).normalized * ballThrowForce, ForceMode.Force);
                ball = null;
            }
            
            callBack?.Invoke();
        }
        else
        {
            noPower?.Invoke();
        }
    }

    public void Update()
    {
        actionOnUpdate?.Invoke();
        carryBall?.Invoke();
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

    public int WallDistance()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, Mathf.Infinity, wallLayer))
        {
            return Mathf.RoundToInt(hit.distance);
        }
        return 0;
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

    // start tpi

    /// <summary>
    /// Update the position of the robot on the grid
    /// </summary>
    public void UpdatePosOnGrid()
    {
        PosOnGrid = transform.position;
    }

    /// <summary>
    /// Round a Vector3 to a Vector3Int
    /// </summary>
    /// <param name="vector3">The vector to convert to Vector3Int</param>
    /// <returns>The converted Vector3Int</returns>
    private Vector3Int ConvertToVector3Int(Vector3 vector3)
    {
        return new Vector3Int
        (
            Mathf.RoundToInt(vector3.x),
            Mathf.RoundToInt(vector3.y),
            Mathf.RoundToInt(vector3.z)
        );
    }
    //end tpi
}
