using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.UI;

// handle all the common features of nodes
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
    /// Execute the node
    /// </summary>
    public abstract void Execute();

    public abstract void CallNextNode();

    /// <summary>
    /// Clean the node when the execution of the script is completed
    /// </summary>
    public abstract void PostExecutionCleanUp(object sender, EventArgs e);

    // id stuff
    public int id;
    public static int nextid = 0;
    private static Dictionary<int, Nodes> nodes = new Dictionary<int, Nodes>();
    public static Dictionary<int, Nodes> NodesDict { get => nodes; set => nodes = value; }

    public RobotScript rs;

    // connection with other node
    [HideInInspector]
    public int nextNodeId = -1; // id of the next node, -1 = not connected
    [HideInInspector]
    public GameObject nextGameObject; // will simplify script execution. It's not vital to have
    [HideInInspector]
    public bool asAParent; // as another node higher in the hierarchy
    public ThreeElementNodeVisual nodeVisual; // change the node element to look good when resized

    public RectTransform canvas; // the node canvas

    // use to test if a node collide with an other
    public LayerMask nodeLayerMask;

    //resize
    private bool resize = false;
    [HideInInspector]
    public bool canResize = true; // the element while accept to be rized only if no other collider touches it
    public GameObject ResizeHandle;

    //move
    private bool move = false;
    public bool isMoving { get => move;}
    [HideInInspector]
    public bool canMove = true;
    public EventHandler OnNodeModified;


    // errors
    protected int nodeErrorCode;
    public int NodeErrorCode { get => nodeErrorCode;}

    // border, will change color when there is an error or when selected
    public List<Image> borders = new List<Image>();
    public Color defaultColor;
    public Color selectedColor;
    public Color errorColor;
    public Color currentExecutedNode;
    protected float executedColorTime = 1;

    public void Awake()
    {
        // All nodes have a different id
        id = nextid;
        nextid++;
        nodes.Add(id, this);
    }

    private void OnDestroy()
    {
        rs.onStop -= PostExecutionCleanUp;
    }

    public void Start()
    {
        // subscribe to the checknode event. The node will check if it is connected correctly
        Manager.instance.CheckNode += isConnected;
        // set the event camera of the canvas
        canvas.gameObject.GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rs.onStop += PostExecutionCleanUp;
    }

    public void ChangeBorderColor(Color color)
    {
        foreach (Image image in borders)
        {
            image.color = color;
        }
    }

    // will return true only if there is no error and the node is connected correctly
    private void isConnected(object sender, EventArgs e)
    {
        if(asAParent)
        {
            if(nextNodeId >= 0)
            {
                if(nodeErrorCode == (int)ErrorCode.notConnected)
                {
                    nodeErrorCode = (int)ErrorCode.ok;
                    ChangeBorderColor(defaultColor);
                    Manager.instance.canExecute = true;
                }
                return;
            }
        }
        nodeErrorCode = (int)ErrorCode.notConnected;
        ChangeBorderColor(errorColor);
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

    public void StartMove()
    {
        canMove = false;
        move = true;
    }

    public void EndMove()
    {
        if (canMove)
            move = false;
    }


    Vector3 resizeAmount;
    public void Resize()
    {
        // round the mouse position
        transform.position = new Vector3(transform.position.x, transform.position.y, -1f);
        Vector3 mouseToWorldPoint = NodeDisplay.instance.nodeCamera.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
        Vector3 delta = Absolute(mouseToWorldPoint - transform.position);

        // The delta is calculated between the center and the corner. It is multiplied by 2 to have the full height
        resizeAmount = new Vector3((float)Math.Round(delta.x*2, 1), (float)Math.Round(delta.y*2, 1), 0);
        // scale the size by a 100 cause the canvas scale is multiplied by 0.01
        canvas.sizeDelta = resizeAmount*100;

        nodeVisual.Resize();
        // change the spline
        OnNodeModified?.Invoke(this, EventArgs.Empty);

        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, resizeAmount, 0f, nodeLayerMask);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != this.gameObject)
            {
                canResize = false;
                ChangeBorderColor(errorColor);
                return;
            }
        }
        ChangeBorderColor(defaultColor);
        canResize = true;
    }

    public void Move()
    {
        // round the mouse position
        Vector3 mouseToWorldPoint = NodeDisplay.instance.nodeCamera.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
        Vector3 pos = new Vector3((float)Math.Round(mouseToWorldPoint.x,1), (float)Math.Round(mouseToWorldPoint.y,1), -1);

        // change the node position
        transform.position = pos;
        // change the spline
        OnNodeModified?.Invoke(this, EventArgs.Empty);

        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, canvas.sizeDelta/100, 0f, nodeLayerMask);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != this.gameObject)
            {
                canMove = false;
                ChangeBorderColor(errorColor);
                return;
            }
        }
        ChangeBorderColor(defaultColor);
        canMove = true;
    }

    // convert a vector3 to absolute value
    private Vector3 Absolute(Vector3 vector3)
    {
        return new Vector3(Math.Abs(vector3.x), Math.Abs(vector3.y), Math.Abs(vector3.z));
    }

    public void DestroyAllNodes()
    {
        foreach (KeyValuePair<int, Nodes> node in nodes)
        {
            Destroy(node.Value.gameObject);
        }
    }

    #region Save stuff
    [Serializable]
    public class SerializableNode
    {
        public int id;
        public int nextNodeId;
        public string type;
        [SerializeField]
        public float[] position;
        [SerializeField]
        public List<string> nodeSettings;
    }
    /// <summary>
    /// Will convert the node to json
    /// </summary>
    public abstract SerializableNode SerializeNode();

    public abstract void DeSerializeNode();

    #endregion
}
