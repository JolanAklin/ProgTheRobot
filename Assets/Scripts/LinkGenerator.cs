using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LinkGenerator : MonoBehaviour
{
    public Nodes nodeStart;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private GameObject linkPartInstance;
    public GameObject linkPart;
    private bool isStarted = false;
    public float width = 0.2f;
    private float height = 0;

    private Vector3 lastPos;
    private Vector3 linkDir;
    private Vector3 lastLinkDir;

    private bool changeDir = false;

    private List<GameObject> links = new List<GameObject>();

    // last linkpart dir
    private bool up;
    private bool down;
    private bool right;
    private bool left;

    private bool end = false;

    public void Start()
    {
        lastPos = this.transform.position;
        StartLink();
    }

    public void Update()
    {
        if(Input.GetMouseButtonDown(0) && !end)
        {
            isStarted = false;
            StartLink();
        }
    }
    public void FixedUpdate()
    {
        if(isStarted)
        {
            // rotate and change the size of the link
            Vector3 mouseToWorldPoint = Round(NodeDisplay.instance.nodeCamera.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono),1);
            if (Mathf.Abs(mouseToWorldPoint.y - linkPartInstance.transform.position.y) < Mathf.Abs(mouseToWorldPoint.x - linkPartInstance.transform.position.x))
            {
                if (mouseToWorldPoint.x > linkPartInstance.transform.position.x && !left)
                {
                    linkDir = new Vector3(1, 0, 0);
                    if(changeDir)
                    {
                        linkPartInstance.transform.rotation = Quaternion.Euler(0, 0, -90);
                        linkPartInstance.transform.position = new Vector3(lastPos.x, lastPos.y + width / 2, lastPos.z);
                    }
                }

                if (mouseToWorldPoint.x < linkPartInstance.transform.position.x && !right)
                {
                    linkDir = new Vector3(-1, 0, 0);
                    if(changeDir)
                    {
                        linkPartInstance.transform.rotation = Quaternion.Euler(0, 0, 90);
                        linkPartInstance.transform.position = new Vector3(lastPos.x, lastPos.y - width/2, lastPos.z);
                    }
                }
                height = (float)Math.Round(Mathf.Abs(mouseToWorldPoint.x - linkPartInstance.transform.position.x),1);
            }
            if (Mathf.Abs(mouseToWorldPoint.y - linkPartInstance.transform.position.y) > Mathf.Abs(mouseToWorldPoint.x - linkPartInstance.transform.position.x))
            {
                if(mouseToWorldPoint.y > linkPartInstance.transform.position.y && !down)
                {
                    linkDir = new Vector3(0, 1, 0);
                    if(changeDir)
                    {
                        linkPartInstance.transform.rotation = Quaternion.Euler(0, 0, 0);
                        linkPartInstance.transform.position = new Vector3(lastPos.x - width / 2, lastPos.y, lastPos.z);
                    }
                }

                if (mouseToWorldPoint.y < linkPartInstance.transform.position.y && !up)
                {
                    linkDir = new Vector3(0, -1, 0);
                    if(changeDir)
                    {
                        linkPartInstance.transform.rotation = Quaternion.Euler(0, 0, 180);
                        linkPartInstance.transform.position = new Vector3(lastPos.x + width / 2, lastPos.y, lastPos.z);
                    }
                }
                height = (float)Math.Round(Mathf.Abs(mouseToWorldPoint.y - linkPartInstance.transform.position.y),1);
            }
            changeDir = false;
            if(linkDir != lastLinkDir)
            {
                changeDir = true;
            }
            lastLinkDir = linkDir;

            #region mesh creation
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(0, 0, 0),
                new Vector3(width, 0, 0),
                new Vector3(0, height, 0),
                new Vector3(width, height, 0)
            };
            mesh.vertices = vertices;

            int[] tris = new int[6]
            {
                // lower left triangle
                0, 2, 1,
                // upper right triangle
                2, 3, 1
            };
            mesh.triangles = tris;

            Vector3[] normals = new Vector3[4]
            {
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward
            };
            mesh.normals = normals;

            Vector2[] uv = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };
            mesh.uv = uv;

            meshFilter.mesh = mesh;
            #endregion
        }
    }
    public void StartLink(bool end = false)
    {
        up = false;
        down = false;
        right = false;
        left = false;
        // calculate the position for the next link
        Vector3 newPos = Round(linkDir * Vector3.Dot(NodeDisplay.instance.nodeCamera.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono), linkDir), 1);
        if (linkDir.x != 0)
        {
            lastPos = new Vector3(newPos.x, lastPos.y, lastPos.z);
            if (linkDir.x == 1)
                right = true;
            else
                left = true;
        }
        if (linkDir.y != 0)
        {
            lastPos = new Vector3(lastPos.x, newPos.y, lastPos.z);
            if (linkDir.y == 1)
                up = true;
            else
                down = true;
        }
        if(!end)
        {
            linkPartInstance = Instantiate(linkPart, lastPos, Quaternion.identity, transform);
            links.Add(linkPartInstance);
            height = 0;
            meshRenderer = linkPartInstance.GetComponent<MeshRenderer>();
            meshFilter = linkPartInstance.GetComponent<MeshFilter>();
            isStarted = true;
        }
    }

    public void EndLink()
    {
        StartLink(true);
        this.end = true;
        isStarted = false;
        Debug.Log("ending link");
    }

    private Vector3 Round(Vector3 vector3, int decimals)
    {
        return new Vector3((float)Math.Round(vector3.x, decimals), (float)Math.Round(vector3.y, decimals), (float)Math.Round(vector3.z, decimals));
    }


}
