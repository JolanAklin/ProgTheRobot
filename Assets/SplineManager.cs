using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SplineManager : MonoBehaviour
{
    private SplineMaker splineMaker;

    private SplineMaker.SplineSegment currentSegment;
    private SplineMaker.SplineSegment lastSegment;

    private bool endSpline = false;

    [HideInInspector]
    public Transform startPos, endPos;

    // last segment direction
    private bool up;
    private bool down;
    private bool right;
    private bool left;

    public void Init(Transform startPos, Nodes node)
    {
        splineMaker = GetComponent<SplineMaker>();
        currentSegment = CreateNewSplineSegment(startPos.position);
        node.OnNodeModified += ChangeSpline;
        this.startPos = startPos;
    }

    private void ChangeSpline(object sender, EventArgs e)
    {
        Debug.Log(startPos.position);
        splineMaker.splineSegments[0].splineStart.point = startPos.position;
        splineMaker.splineSegments[splineMaker.splineSegments.Count - 1].splineEnd.point = endPos.position;
        splineMaker.GenerateMesh();
    }

    public SplineMaker.SplineSegment CreateNewSplineSegment(Vector3 startPos)
    {
        Vector3 MousePos = Round(NodeDisplay.instance.nodeCamera.ScreenToWorldPoint(Input.mousePosition), 1);
        SplineMaker.SplineSegment Segment = new SplineMaker.SplineSegment()
        {
            splineStart = new SplineMaker.SplinePoint
            {
                point = startPos,
                handle = Vector3.down + startPos
            },
            splineEnd = new SplineMaker.SplinePoint
            {
                point = new Vector3(MousePos.x, MousePos.y, 0),
                handle = Vector3.down + startPos
            }
        };
        splineMaker.splineSegments.Add(Segment);
        return Segment;
    }

    private void Update()
    {
        Vector3 MousePos = Vector3.zero;
        Vector3 splineEndPos = Vector3.zero;
        if (!endSpline)
        {
            MousePos = Round(NodeDisplay.instance.nodeCamera.ScreenToWorldPoint(Input.mousePosition), 1);
            splineEndPos = new Vector3(MousePos.x, MousePos.y, 0);

            // test if the mouse pos x is greater than y
            if(lastSegment != null)
            {
                if (Mathf.Abs(MousePos.y - currentSegment.splineStart.point.y) < Mathf.Abs(MousePos.x - currentSegment.splineStart.point.x))
                {
                    if (MousePos.x > currentSegment.splineStart.point.x && !left)
                    {
                        lastSegment.splineEnd.handle = new Vector3(-1, 0, 0) + lastSegment.splineEnd.point;
                        currentSegment.splineStart.handle = new Vector3(1, 0, 0) + currentSegment.splineStart.point;
                    }

                    if (MousePos.x < currentSegment.splineStart.point.x && !right)
                    {
                        lastSegment.splineEnd.handle = new Vector3(1, 0, 0) + lastSegment.splineEnd.point;
                        currentSegment.splineStart.handle = new Vector3(-1, 0, 0) + currentSegment.splineStart.point;
                    }
                    //splineEndPos = new Vector3(splineEndPos.x, lastSegment.splineEnd.point.y, 0);
                }

                // test if the mouse pos y is greater than x
                if (Mathf.Abs(MousePos.y - currentSegment.splineStart.point.y) > Mathf.Abs(MousePos.x - currentSegment.splineStart.point.x))
                {
                    if (MousePos.y > currentSegment.splineStart.point.y && !down)
                    {
                        lastSegment.splineEnd.handle = new Vector3(0, -1, 0) + lastSegment.splineEnd.point;
                        currentSegment.splineStart.handle = new Vector3(0, 1, 0) + currentSegment.splineStart.point;
                    }

                    if (MousePos.y < currentSegment.splineStart.point.y && !up)
                    {
                        lastSegment.splineEnd.handle = new Vector3(0, 1, 0) + lastSegment.splineEnd.point;
                        currentSegment.splineStart.handle = new Vector3(0, -1, 0) + currentSegment.splineStart.point;

                    }
                    //splineEndPos = new Vector3(lastSegment.splineEnd.point.x, splineEndPos.y, 0);
                }

            }

            currentSegment.splineEnd.point = splineEndPos;
            currentSegment.splineEnd.handle = currentSegment.splineEnd.point + Vector3.down;

            splineMaker.GenerateMesh();
        }
        if (Input.GetMouseButtonDown(0))
        {
            lastSegment = currentSegment;
            if (!endSpline)
            {
                currentSegment = CreateNewSplineSegment(new Vector3(MousePos.x, MousePos.y, 0));
            }else
            {
                currentSegment = null;
            }
        }
    }

    public void EndSpline(Transform handleTransform, Nodes node)
    {
        if(splineMaker != null)
        {
            endSpline = true;
            currentSegment.splineEnd.point = handleTransform.position;
            currentSegment.splineEnd.handle = new Vector3(0, 1, 0) + currentSegment.splineEnd.point;
            splineMaker.GenerateMesh();
            endPos = handleTransform;
            node.OnNodeModified += ChangeSpline;
        }
    }

    // round vector3
    private Vector3 Round(Vector3 vector3, int decimals)
    {
        return new Vector3((float)Math.Round(vector3.x, decimals), (float)Math.Round(vector3.y, decimals), (float)Math.Round(vector3.z, decimals));
    }

    private void OnDrawGizmos()
    {
        if (currentSegment != null)
        {
            Gizmos.DrawSphere(currentSegment.splineStart.point, 0.1f);
            Gizmos.DrawSphere(currentSegment.splineStart.handle, 0.1f);
            Gizmos.DrawSphere(currentSegment.splineEnd.handle, 0.1f);
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(currentSegment.splineEnd.point, 0.1f);
        }
        if (lastSegment != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(lastSegment.splineEnd.handle, 0.1f);
        }
    }
}
