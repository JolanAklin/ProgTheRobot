using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Linq;

public class TerrainManager : MonoBehaviour
{
    public GameObject terrainPartPrefab;
    public GameObject fencePlacementPrefab;
    public GameObject objectPlacement;
    public uint maxSize;
    public TMP_InputField changeSizeXInputField;
    public TMP_InputField changeSizeYInputField;

    public List objectList;
    public GameObject[] objects;


    private uint[] terrainSize;

    private List<List<terrainPart>> terrainParts = new List<List<terrainPart>>();


    // raycast stuff
    public GameObject canvas;
    GraphicRaycaster graphicraycaster;
    PointerEventData pointerevent;
    EventSystem eventsystem;
    List<RaycastResult> rayCastResults = new List<RaycastResult>();
    public Camera terrainCam;
    public LayerMask fencePlacementLayer;
    public LayerMask objectPlacementLayer;


    public RenderTexture rt;
    public RectTransform robotImage;

    private Action addObjectActionOnUpdate;


    // robot placement stuff
    public ListRobot listRobot;
    public LayerMask robotPlacementLayer;
    private GameObject robotToMove;

    public class terrainPart
    {
        public GameObject block;
        public GameObject objectPlacement;
        public GameObject[] fencePlacement;
    }


    private List<FencePostion> fencePostions = new List<FencePostion>();
    private List<ObjectPosition> objectPositions = new List<ObjectPosition>();

    private void Awake()
    {
        graphicraycaster = canvas.GetComponent<GraphicRaycaster>();
        eventsystem = canvas.GetComponent<EventSystem>();
    }

    // Start is called before the first frame update
    void Start()
    {

        terrainSize = new uint[] { 10, 10 };
        changeSizeXInputField.text = terrainSize[0].ToString();
        changeSizeYInputField.text = terrainSize[1].ToString();
        CreateTerrain(terrainSize);

        // fill the object list
        objectList.AddChoice(new List.ListElement()
        {
            displayedText = "Barrières",
            actionOnClick = () => { addObjectActionOnUpdate = () => { AddFence(); }; }
        });
        objectList.AddChoice(new List.ListElement()
        {
            displayedText = "Prises",
            actionOnClick = () => { 
                addObjectActionOnUpdate = () => { AddObject(objects[0],0); };
                if (currentObject != null)
                    Destroy(currentObject);
            }
        });
        objectList.AddChoice(new List.ListElement()
        {
            displayedText = "Ballon",
            actionOnClick = () => { 
                addObjectActionOnUpdate = () => { AddObject(objects[1], 1); };
                if (currentObject != null)
                    Destroy(currentObject);
            }
        });
        objectList.AddChoice(new List.ListElement()
        {
            displayedText = "Panier",
            actionOnClick = () => {
                addObjectActionOnUpdate = () => { AddObject(objects[2], 2); };
                if (currentObject != null)
                    Destroy(currentObject);
            }
        });
        objectList.AddChoice(new List.ListElement()
        {
            displayedText = "Arrivée",
            actionOnClick = () => { 
                addObjectActionOnUpdate = () => { AddObject(objects[3], 3); };
                if (currentObject != null)
                    Destroy(currentObject);
            }
        });

    }

    public void LoadRobotList()
    {
        listRobot.Clear();
        Dictionary<int, ListRobot.ListElement> listElements = new Dictionary<int, ListRobot.ListElement>();
        foreach (KeyValuePair<int, ListRobot.ListElement> listElement in Manager.instance.listRobot.getChoices())
        {
            if (!listElement.Value.isAddRobot)
                listElements.Add(listElement.Key, new ListRobot.ListElement() { isAddRobot = false, robotColor = listElement.Value.robotColor, actionOnClick = () => { robotToMove = Robot.robots[listElement.Key].robotManager.gameObject; } });
        }
        listRobot.Init(listElements, 0);
    }

    public void RotateRobot(int dir) // 1 or -1
    {
        if (robotToMove != null)
        {
            robotToMove.transform.Rotate(new Vector3(0, dir * 90, 0));
            robotToMove.GetComponent<RobotManager>().SetDefaultPos(robotToMove.transform.rotation);
        }
    }

    public void MoveRobotButtonClicked()
    {
        addObjectActionOnUpdate = () => { MoveRobot(); };
    }

    private void MoveRobot()
    {
        if(robotToMove != null)
        {
            RaycastHit hit;
            Ray ray = terrainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(terrainCam.transform.position, terrainCam.ScreenPointToRay(Input.mousePosition).direction, out hit, Mathf.Infinity, robotPlacementLayer))
            {
                robotToMove.transform.position = new Vector3(hit.transform.position.x, 0.5f, hit.transform.position.z);
                if(Input.GetKeyDown(KeyCode.Mouse0))
                {
                    addObjectActionOnUpdate = null;
                    robotToMove.GetComponent<RobotManager>().SetDefaultPos(robotToMove.transform);
                }
            }
        }else
        {
            addObjectActionOnUpdate = null;
        }
    }

    public void ShowRobot()
    {
        if(robotToMove != null)
        {
            robotToMove.SetActive(!robotToMove.activeSelf);
        }
    }

    // match the render texture to the app size
    public void MatchRTToScreen()
    {
        terrainCam.targetTexture.Release();
        rt.width = Screen.width;
        rt.height = Screen.height;
        rt.Create();
        robotImage.sizeDelta = new Vector2(Screen.width, Screen.height);
        terrainCam.targetTexture = rt;
    }

    #region generate map
    public void ChangeX()
    {
        if (!uint.TryParse(changeSizeXInputField.text, out terrainSize[0]))
        {
            terrainSize[0] = 1;
            changeSizeXInputField.text = "1";
        }
        else
        {
            if (terrainSize[0] > maxSize)
            {
                terrainSize[0] = maxSize;
                changeSizeXInputField.text = maxSize.ToString();
            }
        }
        CreateTerrain(terrainSize);
    }
    public void ChangeY()
    {
        if (!uint.TryParse(changeSizeYInputField.text, out terrainSize[1]))
        {
            terrainSize[1] = 1;
            changeSizeYInputField.text = "1";
        }
        else
        {
            if (terrainSize[1] > maxSize)
            {
                terrainSize[1] = maxSize;
                changeSizeYInputField.text = maxSize.ToString();
            }
        }
        CreateTerrain(terrainSize);
    }

    public void CreateTerrain(uint[] size)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        terrainParts.Clear();


        FencePostion[] fencePostionsCopy = new FencePostion[fencePostions.Count];
        fencePostions.CopyTo(fencePostionsCopy);
        fencePostions.Clear();

        ObjectPosition[] objectPositionsCopy = new ObjectPosition[objectPositions.Count];
        objectPositions.CopyTo(objectPositionsCopy);
        objectPositions.Clear();

        for (int i = 0; i < size[0]; i++)
        {
            terrainParts.Add(new List<terrainPart>());
            for (int j = 0; j < size[1]; j++)
            {
                GameObject objPlacement = Instantiate(objectPlacement, new Vector3(i, 0, j), Quaternion.identity, this.transform);

                // create a fence if it was there on the terrain before
                ObjectPosition objectPosition = Array.Find(objectPositionsCopy, x => x.position[0] == i && x.position[1] == j);
                if(objectPosition != null)
                {
                    ObjectPlacement objectPlacementScript = objPlacement.GetComponentInChildren<ObjectPlacement>();
                    ObjectPosition objPosition = new ObjectPosition() { position = new int[] { i, j }, objectType = objectPosition.objectType };
                    objectPositions.Add(objPosition);
                    objectPlacementScript.terrainObject = GenerateObject(objectPosition.objectType, new Vector3(objectPosition.position[0], 0, objectPosition.position[1]));
                    objectPlacementScript.objectPosition = objPosition;
                    objPlacement.transform.GetChild(0).tag = "PlacementOccupied";
                }

                GameObject[] fencesPlacement = new GameObject[4];
                for (int k = 0; k < 2; k++)
                {
                    fencesPlacement[k] = Instantiate(fencePlacementPrefab, new Vector3(i, 0, j), Quaternion.Euler(0, k * 90, 0), transform);
                    fencePlacement fencePlacementScript = fencesPlacement[k].GetComponentInChildren<fencePlacement>();
                    fencePlacementScript.pos = new Vector2Int(i, j);
                    fencePlacementScript.rot = k;

                    // create a fence if it was there on the terrain before
                    FencePostion fencePos = Array.Find(fencePostionsCopy, x => x.position[0] == i && x.position[1] == j && x.rot == k);

                    if (j == size[1] - 1 && k == 0 || i == size[0] - 1 && k == 1)
                    {
                        int look = UnityEngine.Random.Range(0, fences.Length);
                        FencePostion fencePosition = new FencePostion() { position = new int[] { fencePlacementScript.pos.x, fencePlacementScript.pos.y }, rot = fencePlacementScript.rot, fenceLook = look, isMapEdge = true };
                        fencePostions.Add(fencePosition);
                        fencePlacementScript.fence = GenerateFence(look, fencesPlacement[k].transform.GetChild(0).transform.position, k);
                        fencesPlacement[k].transform.GetChild(0).tag = "PlacementOccupied";

                        fencePlacementScript.fencePos = fencePosition;
                    }
                    else if (fencePos != null && !fencePos.isMapEdge)
                    {
                        FencePostion fencePosition = new FencePostion() { position = new int[] { fencePlacementScript.pos.x, fencePlacementScript.pos.y }, rot = fencePlacementScript.rot, fenceLook = fencePos.fenceLook };
                        fencePostions.Add(fencePosition);
                        fencePlacementScript.fence = GenerateFence(fencePos.fenceLook, fencesPlacement[k].transform.GetChild(0).transform.position, k);
                        fencesPlacement[k].transform.GetChild(0).tag = "PlacementOccupied";

                        fencePlacementScript.fencePos = fencePosition;
                    }

                }
                // create fence placement for the case on bottom and left side of the terrain
                if (i == 0)
                {
                    fencesPlacement[2] = Instantiate(fencePlacementPrefab, new Vector3(i, 0, j), Quaternion.Euler(0, 3 * 90, 0), transform);
                    fencePlacement fencePlacementScript = fencesPlacement[2].GetComponentInChildren<fencePlacement>();
                    fencePlacementScript.pos = new Vector2Int(i, j);
                    fencePlacementScript.rot = 3;

                    // create that is indestructible
                    int look = UnityEngine.Random.Range(0, fences.Length);
                    FencePostion fencePosition = new FencePostion() { position = new int[] { fencePlacementScript.pos.x, fencePlacementScript.pos.y }, rot = fencePlacementScript.rot, fenceLook = look, isMapEdge = true };
                    fencePostions.Add(fencePosition);
                    fencePlacementScript.fence = GenerateFence(look, fencesPlacement[2].transform.GetChild(0).transform.position, fencePlacementScript.rot);
                    fencesPlacement[2].transform.GetChild(0).tag = "PlacementOccupied";

                    fencePlacementScript.fencePos = fencePosition;
                }
                if (j == 0)
                {
                    fencesPlacement[3] = Instantiate(fencePlacementPrefab, new Vector3(i, 0, j), Quaternion.Euler(0, 2 * 90, 0), transform);
                    fencePlacement fencePlacementScript = fencesPlacement[3].GetComponentInChildren<fencePlacement>();
                    fencePlacementScript.pos = new Vector2Int(i, j);
                    fencePlacementScript.rot = 2;

                    // create that is indestructible
                    int look = UnityEngine.Random.Range(0, fences.Length);
                    FencePostion fencePosition = new FencePostion() { position = new int[] { fencePlacementScript.pos.x, fencePlacementScript.pos.y }, rot = fencePlacementScript.rot, fenceLook = look, isMapEdge = true };
                    fencePostions.Add(fencePosition);
                    fencePlacementScript.fence = GenerateFence(look, fencesPlacement[3].transform.GetChild(0).transform.position, fencePlacementScript.rot);
                    fencesPlacement[3].transform.GetChild(0).tag = "PlacementOccupied";

                    fencePlacementScript.fencePos = fencePosition;
                }
                terrainParts[i].Add(new terrainPart() { block = Instantiate(terrainPartPrefab, new Vector3(i, -0.5f, j), Quaternion.Euler(-90, 0, 0), this.transform), objectPlacement = objPlacement, fencePlacement = fencesPlacement });
            }
        }
    }
    #endregion


    private void Update()
    {
        pointerevent = new PointerEventData(eventsystem);
        pointerevent.position = Input.mousePosition;
        rayCastResults = new List<RaycastResult>();
        graphicraycaster.Raycast(pointerevent, rayCastResults);

        if (rayCastResults.Find(X => X.gameObject.tag == "ModifyTerrain").gameObject != null)
        {
            addObjectActionOnUpdate?.Invoke();

            if(Input.GetKeyDown(KeyCode.Mouse1))
            {
                RaycastHit hit;
                Ray ray = terrainCam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(terrainCam.transform.position, terrainCam.ScreenPointToRay(Input.mousePosition).direction, out hit, Mathf.Infinity, fencePlacementLayer))
                {
                    // remove the fence GameObject and remove the fence on the list
                    if (hit.transform.gameObject.tag == "PlacementOccupied")
                    {
                        fencePlacement fencePlacementScript = hit.transform.GetComponent<fencePlacement>();
                        if (!fencePlacementScript.fencePos.isMapEdge)
                        {
                            Destroy(fencePlacementScript.fence);
                            fencePostions.Remove(fencePlacementScript.fencePos);
                            hit.transform.gameObject.tag = "Untagged";
                        }
                    }
                }else if (Physics.Raycast(terrainCam.transform.position, terrainCam.ScreenPointToRay(Input.mousePosition).direction, out hit, Mathf.Infinity, objectPlacementLayer))
                {
                    // remove the fence GameObject and remove the fence on the list
                    if (hit.transform.gameObject.tag == "PlacementOccupied")
                    {
                        ObjectPlacement objectPlacement = hit.transform.GetComponent<ObjectPlacement>();
                        Destroy(objectPlacement.terrainObject);
                        objectPositions.Remove(objectPlacement.objectPosition);
                        hit.transform.gameObject.tag = "Untagged";
                    }
                }
            }
        }
    }

    #region add object to the terrain
    private void AddFence()
    {
        RaycastHit hit;
        Ray ray = terrainCam.ScreenPointToRay(Input.mousePosition);

        // fence placement
        if (Physics.Raycast(terrainCam.transform.position, terrainCam.ScreenPointToRay(Input.mousePosition).direction, out hit, Mathf.Infinity, fencePlacementLayer))
        {
            if (currentFence == null && hit.transform.gameObject.tag != "PlacementOccupied")
                currentFence = CreateFence();
            else
            {
                if (hit.transform.gameObject.tag != "PlacementOccupied")
                {
                    currentFence.transform.rotation = hit.transform.rotation;
                    currentFence.transform.position = hit.transform.position;

                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        fencePlacement fencePlacementScript = hit.transform.GetComponent<fencePlacement>();
                        FencePostion fencePostion = new FencePostion() { position = new int[] { fencePlacementScript.pos.x, fencePlacementScript.pos.y }, rot = fencePlacementScript.rot, fenceLook = currentLook };
                        fencePostions.Add(fencePostion);

                        fencePlacementScript.fence = currentFence;
                        fencePlacementScript.fencePos = fencePostion;

                        hit.transform.gameObject.tag = "PlacementOccupied";
                        PlaceObject(currentFence);
                        currentFence = CreateFence();
                    }
                }
            }
        }
    }

    private void AddObject(GameObject prefab, int type)
    {
        RaycastHit hit;
        Ray ray = terrainCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(terrainCam.transform.position, terrainCam.ScreenPointToRay(Input.mousePosition).direction, out hit, Mathf.Infinity, objectPlacementLayer))
        {
            if (currentObject == null && hit.transform.gameObject.tag != "PlacementOccupied")
                currentObject = CreateObject(prefab);
            else
            {
                if (hit.transform.gameObject.tag != "PlacementOccupied")
                {
                    currentObject.transform.rotation = hit.transform.rotation;
                    currentObject.transform.position = hit.transform.position;

                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {

                        ObjectPlacement objectPlacement = hit.transform.GetComponent<ObjectPlacement>();
                        ObjectPosition objPosition = new ObjectPosition() { position = new int[] { Mathf.RoundToInt(hit.transform.parent.position.x), Mathf.RoundToInt(hit.transform.parent.position.z) }, objectType = type };
                        objectPositions.Add(objPosition);

                        objectPlacement.terrainObject = currentObject;
                        objectPlacement.objectPosition = objPosition;

                        hit.transform.gameObject.tag = "PlacementOccupied";
                        PlaceObject(currentObject);
                        currentObject = CreateObject(prefab);
                    }
                }
            }
        }
    }

    public GameObject[] fences;

    public Material showMat;
    public Material finalMat;

    private GameObject currentFence;
    private int currentLook;
    private GameObject currentObject;

    public GameObject CreateFence()
    {
        currentLook = UnityEngine.Random.Range(0, fences.Length);
        GameObject fence = Instantiate(fences[currentLook], transform.position, transform.rotation, GameObject.FindGameObjectWithTag("Terrain").transform);
        foreach (MeshRenderer meshRenderer in fence.transform.GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.material = showMat;
        }
        return fence;
    }

    private GameObject GenerateFence(int look, Vector3 position, int rotation)
    {
        return Instantiate(fences[look], position, Quaternion.Euler(0, rotation * 90, 0), GameObject.FindGameObjectWithTag("Terrain").transform);
    }

    private GameObject GenerateObject(int objectType, Vector3 position)
    {
        return Instantiate(objects[objectType], position, Quaternion.identity, GameObject.FindGameObjectWithTag("Terrain").transform);
    }

    public GameObject CreateObject(GameObject prefab)
    {
        GameObject instance = Instantiate(prefab, transform.position, transform.rotation, GameObject.FindGameObjectWithTag("Terrain").transform);
        foreach (MeshRenderer meshRenderer in instance.transform.GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.material = showMat;
        }
        return instance;
    }

    private void PlaceObject(GameObject objectToPlace)
    {
        foreach (MeshRenderer meshRenderer in objectToPlace.transform.GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.material = finalMat;
        }
    }
    #endregion


    #region save and load
    [Serializable]
    public class FencePostion
    {
        public int[] position;
        public int rot; // this number multiplied by 90

        public int fenceLook;

        public bool isMapEdge = false;
    }

    [Serializable]
    public class ObjectPosition
    {
        public int[] position;
        public int objectType;
    }
    [Serializable]
    public class SerializedTerrain
    {
        public uint[] terrainSize;
        [SerializeField]
        public List<FencePostion> fences = new List<FencePostion>();
        [SerializeField]
        public List<ObjectPosition> objects = new List<ObjectPosition>();
    }

    public SerializedTerrain Serialize()
    {
        return new SerializedTerrain()
        {
            terrainSize = terrainSize,
            fences = fencePostions,
            objects = objectPositions,
        };
    }

    public void DeSerialize(SerializedTerrain serializedTerrain)
    {
        terrainSize = serializedTerrain.terrainSize;
        fencePostions = serializedTerrain.fences;
        objectPositions = serializedTerrain.objects;
        changeSizeXInputField.text = terrainSize[0].ToString();
        changeSizeYInputField.text = terrainSize[1].ToString();
        CreateTerrain(terrainSize);
    }
    #endregion
}
