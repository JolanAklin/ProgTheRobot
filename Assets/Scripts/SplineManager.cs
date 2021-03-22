using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// manages a spline maker

// this class need some cleanup. Need to see whats will get removed when the spline making will be completely defined
public class SplineManager : MonoBehaviour
{

    public static List<SplineManager> splineManagers = new List<SplineManager>();

    private SplineMaker splineMaker;

    private SplineMaker.SplineSegment currentSegment;
    private SplineMaker.SplineSegment lastSegment;

    private bool endSpline = false;

    [HideInInspector]
    public Transform startPos, endPos;

    private int robotScriptId;

    private int handleStartNumber;
    private int handleEndNumber;

    // last segment direction. not used yet
    private bool up;
    private bool down;
    private bool right;
    private bool left;

    public GameObject MoveHandle;

    private Nodes nodeStart, nodeEnd;

    // start pos will be used when a node is moved or rized.
    // the node parameter only serve to subscibe to the change/resize event
    public void Init(Transform startPos, Nodes node, int handleId)
    {
        splineManagers.Add(this);
        RobotScript.robotScripts[Manager.instance.currentlySelectedScript].splines.Add(this.gameObject);
        robotScriptId = Manager.instance.currentlySelectedScript;

        splineMaker = GetComponent<SplineMaker>();
        currentSegment = CreateNewSplineSegment(startPos.position);
        node.OnNodeModified += ChangeSpline;
        this.startPos = startPos;
        handleStartNumber = handleId;
        nodeStart = node;
    }

    // create spline from files
    public void Init(Transform startPos, Nodes node, RobotScript robotScript, int handleId)
    {
        splineManagers.Add(this);
        robotScript.splines.Add(this.gameObject);
        robotScriptId = Manager.instance.currentlySelectedScript;

        splineMaker = GetComponent<SplineMaker>();
        currentSegment = CreateNewSplineSegment(startPos.position);
        node.OnNodeModified += ChangeSpline;
        handleStartNumber = handleId;
        this.startPos = startPos;
        nodeStart = node;
    }

    // will update point and handle position when a node is moved or resized
    private void ChangeSpline(object sender, EventArgs e)
    {
        splineMaker.splineSegments[0].splineStart.point = new Vector3(startPos.position.x, startPos.position.y, -0.15f);
        splineMaker.splineSegments[0].splineStart.handle = new Vector3(startPos.position.x, startPos.position.y, -0.15f) + Vector3.down;
        splineMaker.splineSegments[splineMaker.splineSegments.Count - 1].splineEnd.point = new Vector3(endPos.position.x, endPos.position.y, -0.15f);
        splineMaker.splineSegments[splineMaker.splineSegments.Count - 1].splineEnd.handle = new Vector3(endPos.position.x, endPos.position.y, -0.15f) + Vector3.up;

        if(MoveHandle != null)
            MoveHandle.transform.position = new Vector3(endPos.position.x, endPos.position.y, MoveHandle.transform.position.z);

        splineMaker.GenerateMesh();
    }

    // create a new segment added to the current spline
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
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                willBeMoved = false;
                splineManagers.Remove(this);
                Destroy(this.gameObject);
            }
            MousePos = Round(NodeDisplay.instance.nodeCamera.ScreenToWorldPoint(Input.mousePosition), 1);
            splineEndPos = new Vector3(MousePos.x, MousePos.y, 0);

            // update the pos of current spline's point
            currentSegment.splineEnd.point = splineEndPos;
            currentSegment.splineEnd.handle = currentSegment.splineEnd.point + Vector3.down;

            splineMaker.GenerateMesh();
        }
    }

    // finishes the spline
    public void EndSpline(Transform handleTransform, Nodes node, int handleId)
    {
        if (splineMaker != null)
        {
            endSpline = true;
            handleEndNumber = handleId;
            currentSegment.splineEnd.point = handleTransform.position;
            currentSegment.splineEnd.handle = new Vector3(0, 1, 0) + currentSegment.splineEnd.point;
            splineMaker.GenerateMesh();
            endPos = handleTransform;
            handleEndNumber = handleId;
            node.OnNodeModified += ChangeSpline;
            MoveHandle.transform.position = new Vector3(handleTransform.position.x, handleTransform.position.y, MoveHandle.transform.position.z); ;
            MoveHandle.SetActive(true);
            nodeEnd = node;
        }
    }
    private bool willBeMoved = false;
    public void MoveSpline()
    {
        if(!ExecManager.Instance.isRunning)
        {
            willBeMoved = true;
            Destroy(this.gameObject);
            ConnectHandle connect = nodeStart.handleStartArray[handleStartNumber].GetComponent<ConnectHandle>();
            connect.canBeClicked = true;
            connect.Click();
        }
    }

    private void OnDestroy()
    {
        Manager.instance.node = null;
        nodeStart.OnNodeModified -= ChangeSpline;
        try
        {
            nodeEnd.OnNodeModified -= ChangeSpline;
        }catch(Exception e)
        {

        }

        if(!willBeMoved)
        {
            Manager.instance.OnSpline?.Invoke(Manager.instance, new Manager.OnSplineEventArgs() { splineStarted = false});
            ConnectHandle connect = nodeStart.handleStartArray[handleStartNumber].GetComponent<ConnectHandle>();
            connect.canBeClicked = true;
            connect.image.enabled = true;
            connect.boxCollider2d.enabled = true;
        }
    }

    // round vector3
    private Vector3 Round(Vector3 vector3, int decimals)
    {
        return new Vector3((float)Math.Round(vector3.x, decimals), (float)Math.Round(vector3.y, decimals), (float)Math.Round(vector3.z, decimals));
    }

    #region save stuff
    [Serializable]
    public class SerializedSpline
    {
        public int idNodeStart;
        public int handleStart; // number in an array on the node
        public int idNodeEnd;
        public int handleEnd;
        public int robotScriptId;
    }

    public SerializedSpline SerializeSpline()
    {
        SerializedSpline serializedSpline = new SerializedSpline()
        {
            idNodeStart = startPos.gameObject.GetComponent<ConnectHandle>().node.id,
            handleStart = handleStartNumber,
            idNodeEnd = endPos.gameObject.GetComponent<ConnectHandle>().node.id,
            handleEnd = handleEndNumber,
            robotScriptId = robotScriptId,
        };
        return serializedSpline;
    }

    public void DeSerializeSpline(SerializedSpline serializedSpline)
    {
        Init(Nodes.NodesDict[serializedSpline.idNodeStart].handleStartArray[serializedSpline.handleStart].transform, Nodes.NodesDict[serializedSpline.idNodeStart], RobotScript.robotScripts[serializedSpline.robotScriptId], serializedSpline.handleStart);
        EndSpline(Nodes.NodesDict[serializedSpline.idNodeEnd].handleEndArray[serializedSpline.handleEnd].transform, Nodes.NodesDict[serializedSpline.idNodeEnd], serializedSpline.handleEnd);
    }

    public void DestroyAllSplines()
    {
        foreach (SplineManager splineManager in splineManagers)
        {
            Destroy(splineManager.gameObject);
        }
    }
    #endregion
}
