// Copyright 2021 Jolan Aklin

//This file is part of Prog The Robot.

//Prog The Robot is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 3 of the License.

//Prog The Robot is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Prog the robot.  If not, see<https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

// handle all the common features of nodes
public abstract class Nodes : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
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
        sound,
    }

    public enum ErrorCode
    {
        ok = 0,
        wrongInput,
        notConnected,
    }

    protected NodeTypes nodeTypes;
    public NodeTypes NodeType { get => nodeTypes; protected set => nodeTypes = value; }

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
    public int id = -1;
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
    public int parentId = -1; // as another node higher in the hierarchy
    public ThreeElementNodeVisual nodeVisual; // change the node element to look good when resized
    [HideInInspector]
    private int numberOfInputConnection = 0;

    public RectTransform canvasRect; // the node canvas
    private Canvas canvas;
    public Canvas Canvas { get => canvas; private set => canvas = value; }

    // use to test if a node collide with an other
    public LayerMask nodeLayerMask;
    public LayerMask insideLoopMask;

    public ConnectHandle[] handleStartArray;
    public ConnectHandle[] handleEndArray;

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
    public List<BoxCollider2D> collidersToIgnore;
    public EventHandler<OnNodeModifiedEventArgs> OnNodeModified;

    public class OnNodeModifiedEventArgs : EventArgs
    {
        public bool destroy = false;
    }

    public SplineManager[] currentSplines = new SplineManager[2]; // splines going out of this node
    private GameObject nodeHolder;
    protected LoopArea nodesLoopArea;
    public LoopArea NodesLoopArea { get => nodesLoopArea; protected set => nodesLoopArea = value; }
    private Action onMoveEnd;
    private List<Nodes> nodesInsideLoop = new List<Nodes>();
    public List<Nodes> NodesInsideLoop { get => nodesInsideLoop; private set => nodesInsideLoop = value; }

    private Vector3 mouseCenterDelta;

    public uint nodeExecPower = 5;


    // errors
    protected ErrorCode nodeErrorCode = ErrorCode.ok;
    public ErrorCode NodeErrorCode { get => nodeErrorCode;}

    // border, will change color when there is an error or when selected
    public List<Image> borders = new List<Image>();
    public Color defaultColor;
    public Color selectedColor;
    public Color errorColor;
    public Color currentExecutedNode;
    public Color insideNodeColor;
    protected float executedColorTime = 1;

    public string infoTextTitle;
    [TextArea]
    public string infoText;


    public bool isEndNode = false;

    [Header("Size")]
    [Tooltip("Put minWidth and maxWidth to same number to prevent the node to be resized")]
    public float minWidth;
    [Tooltip("Put minHeight and maxHeight to same number to prevent the node to be resized")]
    public float minHeight;
    [Tooltip("Put minWidth and maxWidth to same number to prevent the node to be resized")]
    public float maxWidth;
    [Tooltip("Put minHeight and maxHeight to same number to prevent the node to be resized")]
    public float maxHeight;
    [Tooltip("Width will be multiplied by the height * minAspectRation. Put this number to 0 to do nothing")]
    public float minAspectRatio;

    private LoopArea parentLoopArea;
    public LoopArea ParentLoopArea { get => parentLoopArea; set => parentLoopArea = value; }

    private bool isInputLocked = true;
    public bool IsInputLocked { get => isInputLocked; protected set => isInputLocked = value; }

    /// <summary>
    /// Used to confine a node to a certain area
    /// </summary>
    private class NodeConfinement
    {
        public float left;
        public float right;
        public float top;
        public float bottom;
    }

    private NodeInfo.Info nodeInfo;
    [SerializeField]
    protected TMP_Text nodeContentDisplay;

    #region click handling
    public EventHandler OnDoubleClick;
    public EventHandler OnRightClick;
    public EventHandler OnLeftClick;

    /// <summary>
    /// When true, a double click can happen
    /// </summary>
    private bool doubleClick = false;
    private bool preventDrag = false;
    private bool preventClicks = false;
    public bool preventMove = false;

    private static bool nodeBeenLeftClicked = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        MenuToolTip.instance.HideToolTip();
        if(!preventClicks)
        {
            if (eventData.button == PointerEventData.InputButton.Left && doubleClick)
            {
                // double click performed
                nodeBeenLeftClicked = true;
                OnDoubleClick?.Invoke(this, EventArgs.Empty);
                preventDrag = true;
            }
            else if (eventData.button == PointerEventData.InputButton.Left)
            {
                // simple left click performed
                nodeBeenLeftClicked = true;

                // stop the movement of a node only by clicking once on it
                if (move)
                {
                    EndMove();
                    return;
                }

                OnLeftClick?.Invoke(this, EventArgs.Empty);
                doubleClick = true;
                StartCoroutine("DoubleClick");
                SelectNode();

                // show infos about this node
                NodeInfo.infoTitle.text = nodeInfo.infoTextTitle;
                NodeInfo.infoDesc.text = nodeInfo.infoText;
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                // simple right click performed
                OnRightClick?.Invoke(this, EventArgs.Empty);
                ShowContextMenu();
            }
        }
    }

    /// <summary>
    /// Wait for double click
    /// </summary>
    /// <returns></returns>
    IEnumerator DoubleClick()
    {
        yield return new WaitForSecondsRealtime(0.4f);
        doubleClick = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (preventMove)
                return;
            if (preventDrag)
            {
                preventDrag = false;
                return;
            }
            foreach (Nodes selectedNode in SelectionManager.instance.SelectedNodes)
            {
                selectedNode.StartMove();
            }
        }

        // Prepare the movement of the script area
        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            MoveScriptArea.instance.PrepareDrag();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // move the script area
        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            MoveScriptArea.instance.MoveArea();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            foreach (Nodes selectedNode in SelectionManager.instance.SelectedNodes)
            {
                selectedNode.EndMove();
            }
        }

        // finishe the move area movement
        MoveScriptArea.instance.EndDrag();
    }

    // show tooltip
    private bool ShouldShowToolTip = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        MoveScriptArea.instance.CursorHover(true);
        if (nodeContentDisplay == null)
            return;
        if (nodeContentDisplay.text == null)
            return;
        if (nodeContentDisplay.text.Length == 0)
            return;
        if(TextToolTip.instance.isToolTipOpened)
        {
            TextToolTip.instance.ShowToolTip(nodeContentDisplay.text);
        }
        else
        {
            ShouldShowToolTip = true;
            StartCoroutine("ShowToolTip");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MoveScriptArea.instance.CursorHover(false);
        ShouldShowToolTip = false;
        TextToolTip.instance.HideToolTip();
    }
    IEnumerator ShowToolTip()
    {
        yield return new WaitForSeconds(0.5f);
        if(ShouldShowToolTip)
            TextToolTip.instance.ShowToolTip(nodeContentDisplay.text);
    }

    private void ShowContextMenu()
    {
        if (NodeType != NodeTypes.start && nodeTypes != NodeTypes.end)
        {
            MenuToolTip.instance.SetContent("Node menu", new MenuToolTip.ButtonContent[] {
                new MenuToolTip.ButtonContent("Modify", () => { ModifyNodeContent(this, EventArgs.Empty);MenuToolTip.instance.HideToolTip(); }),
                new MenuToolTip.ButtonContent("Delete", () => { Destroy(this.gameObject);MenuToolTip.instance.HideToolTip(); }),
            });
        }
        else
        {
            MenuToolTip.instance.SetContent("Node menu", new MenuToolTip.ButtonContent[] {
                new MenuToolTip.ButtonContent("Delete", () => { Destroy(this.gameObject);MenuToolTip.instance.HideToolTip(); }),
            });
        }
        MenuToolTip.instance.ShowToolTip();
    }

    private void SelectNode()
    {
        if (SelectionManager.instance.SelectedNodes.Contains(this))
            return;
        if (Input.GetKey(KeyCode.LeftShift))
            SelectionManager.instance.AddNodeToSelection(this, false);
        else
            SelectionManager.instance.AddNodeToSelection(this);
    }
    #endregion


    protected void Awake()
    {
        for (int i = 0; i < handleStartArray.Length; i++)
        {
            handleStartArray[i].handleNumber = i;
        }
        for (int i = 0; i < handleEndArray.Length; i++)
        {
            handleEndArray[i].handleNumber = i;
        }
        UpdateIgnoreColliderList();

        canvas = canvasRect.GetComponent<Canvas>();

        nodesLoopArea = GetComponentInChildren<LoopArea>();

    }

    
    protected void DestroyNode()
    {
        //hide the tooltip
        ShouldShowToolTip = false;
        TextToolTip.instance.HideToolTip();

        rs.onStop -= PostExecutionCleanUp;
        OnNodeModified?.Invoke(this, new OnNodeModifiedEventArgs() { destroy = true });

        // remove all the connected splines
        for (int j = 0; j < currentSplines.Length; j++)
        {
            if (currentSplines[j] != null)
                Destroy(currentSplines[j].gameObject);
        }
        NodesDict.Remove(id);
        rs.nodes.Remove(this.gameObject);
    }

    protected void Start()
    {
        nodeInfo = NodeInfo.nodesInfos.Find(x => x.nodeTypes == nodeTypes);
        // All nodes have a different id
        SetId();

        // subscribe to the checknode event. The node will check if it is connected correctly
        Manager.instance.CheckNode += isConnected;
        // set the event camera of the canvas
        canvasRect.gameObject.GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rs.onStop += PostExecutionCleanUp;

        nodeHolder = GameObject.FindGameObjectWithTag("NodeHolder");
    }

    public void SetId()
    {
        if(id == -1)
        {
            id = nextid;
            nextid++;
            nodes.Add(id, this);
        }
    }

    protected abstract void ModifyNodeContent(object sender, EventArgs e);

    public void ChangeBorderColor(Color color)
    {
        if(color != defaultColor || NodeErrorCode == ErrorCode.ok)
        {
            foreach (Image image in borders)
            {
                image.color = color;
            }
            return;
        }
        if(NodeErrorCode != ErrorCode.ok)
        {
            foreach (Image image in borders)
            {
                image.color = errorColor;
            }
        }
    }

    // will return true only if there is no error and the node is connected correctly
    private void isConnected(object sender, EventArgs e)
    {
        if(numberOfInputConnection > 0 && !isEndNode)
        {
            if(!NodesDict.ContainsKey(nextNodeId))
            {
                if(nodeErrorCode == ErrorCode.ok)
                {
                    nodeErrorCode = ErrorCode.notConnected;
                    ChangeBorderColor(errorColor);
                    Manager.instance.canExecute = false;
                }
            }
            else
            {
                if (nodeErrorCode != ErrorCode.wrongInput)
                {
                    nodeErrorCode = ErrorCode.ok;
                    ChangeBorderColor(defaultColor);
                }
            }
        }
        else
        {
            if (nodeErrorCode != ErrorCode.wrongInput)
            {
                nodeErrorCode = ErrorCode.ok;
                ChangeBorderColor(defaultColor);
            }
        }
    }

    public void AddConnection()
    {
        numberOfInputConnection++;
    }
    public void RemoveConnection()
    {
        numberOfInputConnection--;
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
        preventClicks = true;
        // get the delta between the node and the mouse cursor to avoid weird snap to the mouse cursor
        Vector3 mouseToWorldPoint = NodeDisplay.instance.nodeCamera.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
        mouseCenterDelta = transform.position - mouseToWorldPoint;
        canMove = false;
        move = true;
    }

    private void UpdateIgnoreColliderList()
    {
        collidersToIgnore.Clear();
        collidersToIgnore.AddRange(GetComponents<BoxCollider2D>());
        BoxCollider2D[] boxCollider2Ds = GetComponentsInChildren<BoxCollider2D>();
        collidersToIgnore.AddRange(boxCollider2Ds);
    }

    public void EndMove()
    {
        if(nodesLoopArea != null)
            nodesLoopArea.collider.enabled = true;
        if (lastInsideLoopImage != null)
            lastInsideLoopImage.color = insideNodeColor;
        if (canMove)
            move = false;
        preventClicks = false;
        onMoveEnd?.Invoke();
    }


    Vector3 resizeAmount;
    public void Resize()
    {
        // round the mouse position
        //transform.position = new Vector3(transform.position.x, transform.position.y, -1f);
        Vector3 mouseToWorldPoint = NodeDisplay.instance.nodeCamera.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
        Vector3 delta = Absolute(mouseToWorldPoint - transform.position);

        // The delta is calculated between the center and the corner. It is multiplied by 2 to have the full height
        resizeAmount = new Vector3((float)Math.Round(delta.x*2, 1), (float)Math.Round(delta.y*2, 1), 0);

        //start tpi
        // apply a min form factor to the node. It prevent it from making some weird visual for some of them
        if(resizeAmount.x < resizeAmount.y * minAspectRatio && minAspectRatio != 0)
        {
            resizeAmount.x = resizeAmount.y * minAspectRatio;
        }
        // clamp the node size
        resizeAmount = new Vector3(Mathf.Clamp(resizeAmount.x, minWidth, maxWidth), Mathf.Clamp(resizeAmount.y, minHeight, maxHeight), 0);
        //end tpi

        // scale the size by a 100 cause the canvas scale is multiplied by 0.01
        canvasRect.sizeDelta = resizeAmount*100;

        nodeVisual.Resize();
        // change the spline
        OnNodeModified?.Invoke(this, new OnNodeModifiedEventArgs());

        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, canvasRect.sizeDelta / 100, 0f, nodeLayerMask);
        foreach (Collider2D collider in colliders)
        {
            canResize = false;
            foreach (BoxCollider2D collider2D in collidersToIgnore)
            {
                if (collider == collider2D)
                {
                    canResize = true;
                    break;
                }
            }
        }
        if (canResize)
            ChangeBorderColor(selectedColor);
        else
            ChangeBorderColor(errorColor);
    }

    public void Resize(Vector2 size)
    {
        canvasRect.sizeDelta = size;
    }

    private Image lastInsideLoopImage;
    public void Move()
    {
        // round the mouse position
        Vector3 mouseToWorldPoint = NodeDisplay.instance.nodeCamera.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
        mouseToWorldPoint += mouseCenterDelta;
        Vector3 pos = new Vector3((float)Math.Round(mouseToWorldPoint.x,1), (float)Math.Round(mouseToWorldPoint.y,1), -890);

        //start tpi
        // force the node to stay inside a defined area
        if (parentLoopArea != null)
        {
            NodeConfinement confinement = new NodeConfinement() { right = parentLoopArea.Right(), left = parentLoopArea.Left(), top = parentLoopArea.Top(), bottom = parentLoopArea.Bottom() };
            float xPos;
            float yPos;
            if (confinement.left > confinement.right)
                xPos = Mathf.Clamp(pos.x, confinement.right, confinement.left);
            else
                xPos = Mathf.Clamp(pos.x, confinement.left, confinement.right);

            if (confinement.top > confinement.bottom)
                yPos = Mathf.Clamp(pos.y, confinement.bottom, confinement.top);
            else
                yPos = Mathf.Clamp(pos.y, confinement.top, confinement.bottom);

            pos = new Vector3(xPos, yPos, pos.z);
        }
        //end tpi

        // change the node position
        transform.position = pos;
        // change the spline
        OnNodeModified?.Invoke(this, new OnNodeModifiedEventArgs());

        //bool blocInLoop = false;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, canvasRect.sizeDelta/100, 0f, nodeLayerMask);
        foreach (Collider2D collider in colliders)
        {
            canMove = false;
            foreach (BoxCollider2D collider2D in collidersToIgnore)
            {
                if (collider == collider2D)
                {
                    canMove = true;
                    break;
                }
            }
        }
        if (canMove)
            ChangeBorderColor(selectedColor);
        else
            ChangeBorderColor(errorColor);

        MoveSplineForNodeInsideLoop(nodesInsideLoop);


        if (nodesLoopArea != null)
            nodesLoopArea.collider.enabled = false;

        if (lastInsideLoopImage != null)
            lastInsideLoopImage.color = insideNodeColor;

        // prevent the node end and node start from being in a loop
        if(nodeTypes != NodeTypes.end && nodeTypes != NodeTypes.start)
        {
            // put the node in the loop or get it out
            RaycastHit2D hit;
            Ray ray = NodeDisplay.instance.nodeCamera.ScreenPointToRay(Input.mousePosition);
            if (hit = Physics2D.Raycast(transform.position, transform.forward, Mathf.Infinity, insideLoopMask))
            {
                LoopArea loopArea = hit.collider.GetComponent<LoopArea>();
                if(!loopArea.parent.transform.IsChildOf(transform))
                {
                    // changing the color
                    Image currentInsideLoop = loopArea.image;
                    currentInsideLoop.color = selectedColor;
                    if (lastInsideLoopImage != null && lastInsideLoopImage != currentInsideLoop)
                        lastInsideLoopImage.color = insideNodeColor;
                    lastInsideLoopImage = currentInsideLoop;


                    // changing the sorting order of the canvas to ensure that the node is on the top
                    canvas.sortingOrder = loopArea.nodeCanvas.sortingOrder + 1;
                    Nodes node = loopArea.parent.GetComponent<Nodes>();
                    parentId = node.id;
                    onMoveEnd = () =>
                    {
                        transform.parent = loopArea.parent.transform;
                        float zpos = (loopArea.parent.transform.position.z - 0.001f);
                        transform.position = new Vector3(transform.position.x, transform.position.y, zpos);
                        node.nodesInsideLoop.Add(this);

                        parentLoopArea = loopArea;
                        handleStartArray[0].loopArea = parentLoopArea;
                        handleEndArray[0].loopArea = parentLoopArea;
                        if(nodeTypes == NodeTypes.test)
                        {
                            handleStartArray[1].loopArea = parentLoopArea;
                        }
                    };
                }
            
            }
            else
            {
                parentId = -1;

                // changing the color back
                if (lastInsideLoopImage != null)
                    lastInsideLoopImage.color = insideNodeColor;

                canvas.sortingOrder = 0;

                onMoveEnd = () =>
                {
                    if(NodesDict.ContainsKey(parentId))
                    {
                        NodesDict[parentId].GetComponent<Nodes>().nodesInsideLoop.Remove(this);
                    }
                    else
                    {
                        parentId = -1;
                    }
                    transform.parent = nodeHolder.transform;
                    transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
                };
            }
        }
        else
        {
            onMoveEnd = () =>
            {
                transform.parent = nodeHolder.transform;
                transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
            };
        }
    }

    private void MoveSplineForNodeInsideLoop(List<Nodes> nodes)
    {
        foreach (Nodes node in nodes)
        {
            node.OnNodeModified?.Invoke(this, new OnNodeModifiedEventArgs());
            MoveSplineForNodeInsideLoop(node.nodesInsideLoop);
        }
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
        public int nextNodeId; // the node after this one
        public int parentId;
        public string type;
        [SerializeField]
        public float[] position;
        [SerializeField]
        public float[] size;
        [SerializeField]
        public List<string> nodeSettings;
    }

    /// <summary>
    /// Will convert the node to json
    /// </summary>
    public abstract SerializableNode SerializeNode();

    /// <summary>
    /// Will convert the json to a usable node object
    /// </summary>
    public abstract void DeSerializeNode(SerializableNode serializableNode);

    public void FindParent()
    {
        if(NodesDict.ContainsKey(parentId))
        {
            transform.parent = NodesDict[parentId].transform;
            Nodes parentNode = NodesDict[parentId].GetComponent<Nodes>();
            canvas.sortingOrder = parentNode.canvas.sortingOrder + 1;
            parentNode.nodesInsideLoop.Add(this);
            // start tpi
            // get the loop area of the parent and set it accordingly
            parentLoopArea = NodesDict[parentId].nodesLoopArea;
            handleStartArray[0].loopArea = parentLoopArea;
            handleEndArray[0].loopArea = parentLoopArea;
            // end tpi
        }
    }

    #endregion
}
