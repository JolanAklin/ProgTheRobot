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

    /// <summary>
    /// Will convert the node to json
    /// </summary>
    public abstract void SerializeNode();

    public abstract void DeSerializeNode();

    /// <summary>
    /// Execute the node
    /// </summary>
    public abstract void Execute();

    public int id;
    public static int nextid = 0;
    private int nextId;
    private GameObject nextGameObject;

    public RectTransform canvas;
    public GameObject ResizeHandle;
    private bool resize = false;
    private bool canResize = true; // the element while accept to be rized only if no other collider touches it
    public ThreeElementNodeVisual nodeVisual;

    private void Awake()
    {
        // All nodes have a different id
        id = nextid;
        nextid++;
    }

    public void Start()
    {
        
    }

    public void Move()
    {

    }

    private void Update()
    {
        if(resize)
        {
            Resize();
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

    Vector3 resizeAmount;

    public void Resize()
    {
        Vector3 mouseToWorldPoint = NodeDisplay.instance.nodeCamera.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
        Vector3 delta = Absolute(mouseToWorldPoint - transform.position);
        resizeAmount = new Vector3((float)Math.Round(delta.x*2, 1), (float)Math.Round(delta.y*2, 1), 0);
        canvas.sizeDelta = resizeAmount*100;
        nodeVisual.Resize();
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, resizeAmount,0f);
        canResize = true;
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != this.gameObject && collider.gameObject.tag != "ResizeHandle")
            {
                canResize = false;
                break;
            }
        }
    }

    private Vector3 Absolute(Vector3 vector3)
    {
        return new Vector3(Math.Abs(vector3.x), Math.Abs(vector3.y), Math.Abs(vector3.z));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, resizeAmount);
    }
}
