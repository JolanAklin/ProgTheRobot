using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public abstract class Nodes : MonoBehaviour
{
    public enum NodeTypes
    {
        start = 0,
        end,
        execute,
        readWrite,
        test,
        subProgram,
        affectation,
        whileLoop,
        forLoop,
    }

    public enum ErrorCode
    {
        ok = 0,
        wrongInput,
        notConnected,
    }

    /// <summary>
    /// Will convert the node to json
    /// </summary>
    public abstract void SerializeNode();

    public abstract void DeSerializeNode();

    /// <summary>
    /// Execute the node
    /// </summary>
    public abstract void Execute();

    // id stuff
    public int id;
    public static int nextid = 0;


    // connection with other node
    [HideInInspector]
    public int nextNodeId = -1; // id of the next node, -1 = not connected
    [HideInInspector]
    public GameObject nextGameObject;
    [HideInInspector]
    public bool asAParent; // as another node higher in the hierarchy
    public ThreeElementNodeVisual nodeVisual;

    public RectTransform canvas;

    //resize
    private bool resize = false;
    [HideInInspector]
    public bool canResize = true; // the element while accept to be rized only if no other collider touches it
    public GameObject ResizeHandle;

    //move
    private bool move = false;
    [HideInInspector]
    public bool canMove = true;

    // errors
    protected int nodeErrorCode;
    public int NodeErrorCode { get => nodeErrorCode;}

    private void Awake()
    {
        // All nodes have a different id
        id = nextid;
        nextid++;
    }

    public void Start()
    {
        Manager.instance.CheckNode += isConnected;
        canvas.gameObject.GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    private void isConnected(object sender, EventArgs e)
    {
        if(asAParent)
        {
            if(nextNodeId >= 0)
            {
                if(nodeErrorCode == (int)ErrorCode.notConnected)
                {
                    nodeErrorCode = (int)ErrorCode.ok;
                    Manager.instance.canExecute = true;
                }
                return;
            }
        }
        nodeErrorCode = (int)ErrorCode.notConnected;
        Manager.instance.canExecute = false;
        return;
    }

    private void FixedUpdate()
    {
        if(resize)
        {
            Resize();
        }
        if(move)
        {
            Move();
        }
    }

    public void StartEndResize()
    {
        if(resize)
        {
            if(canResize)
                resize = false;
        }else
        {
            resize = true;
        }
    }
    public void StartEndMove()
    {
        if (move)
        {
            if (canMove)
                move = false;
        }
        else
        {
            move = true;
        }
    }

    Vector3 resizeAmount;


    public void Resize()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -1f);
        Vector3 mouseToWorldPoint = NodeDisplay.instance.nodeCamera.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
        Vector3 delta = Absolute(mouseToWorldPoint - transform.position);
        resizeAmount = new Vector3((float)Math.Round(delta.x*2, 1), (float)Math.Round(delta.y*2, 1), 0);
        canvas.sizeDelta = resizeAmount*100;
        nodeVisual.Resize();
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, resizeAmount,0f);
        canResize = true;
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != this.gameObject && collider.gameObject.tag != "ResizeHandle" && collider.gameObject.tag != "ConnectHandle")
            {
                canResize = false;
                break;
            }
        }
    }

    public void Move()
    {
        Vector3 mouseToWorldPoint = NodeDisplay.instance.nodeCamera.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
        Vector3 pos = new Vector3((float)Math.Round(mouseToWorldPoint.x,1), (float)Math.Round(mouseToWorldPoint.y,1), -1);
        transform.position = pos;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, canvas.sizeDelta/100, 0f);
        canMove = true;
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != this.gameObject && collider.gameObject.CompareTag("ResizeHandle") && collider.gameObject.CompareTag("ConnectHandle"))
            {
                canMove = false;
                break;
            }
        }
    }

    private Vector3 Absolute(Vector3 vector3)
    {
        return new Vector3(Math.Abs(vector3.x), Math.Abs(vector3.y), Math.Abs(vector3.z));
    }
}
