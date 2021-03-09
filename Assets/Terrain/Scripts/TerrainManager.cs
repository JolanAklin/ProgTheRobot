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

    public class terrainPart
    {
        public GameObject block;
        public GameObject objectPlacement;
        public GameObject[] fencePlacement;
    }

    [Serializable]
    private class FencePostion
    {
        public int[] position;
        public int rot; // this number multiplied by 90

        public int fenceLook;
    }

    [Serializable]
    private class ObjectPosition
    {
        public int[] position;
        public int objectType;
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


        MatchRTToScreen();

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

    private void MatchRTToScreen()
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

                ObjectPosition objectPosition = Array.Find(objectPositionsCopy, x => x.position[0] == i && x.position[1] == j);
                if(objectPosition != null)
                {
                    objectPositions.Add(new ObjectPosition() { position = new int[] { i, j }, objectType = objectPosition.objectType });
                    GenerateObject(objectPosition.objectType, new Vector3(objectPosition.position[0], 0, objectPosition.position[1]));
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
                    if(fencePos != null)
                    {
                        fencePostions.Add(new FencePostion() { position = new int[] { fencePlacementScript.pos.x, fencePlacementScript.pos.y }, rot = fencePlacementScript.rot, fenceLook = fencePos.fenceLook });
                        GenerateFence(fencePos.fenceLook, fencesPlacement[k].transform.GetChild(0).transform.position, k);
                        fencesPlacement[k].transform.GetChild(0).tag = "PlacementOccupied";
                    }

                }
                if (i == 0)
                {
                    fencesPlacement[2] = Instantiate(fencePlacementPrefab, new Vector3(i, 0, j), Quaternion.Euler(0, 3 * 90, 0), transform);
                    fencePlacement fencePlacementScript = fencesPlacement[2].GetComponentInChildren<fencePlacement>();
                    fencePlacementScript.pos = new Vector2Int(i, j);
                    fencePlacementScript.rot = 3;

                    // create a fence if it was there on the terrain before
                    FencePostion fencePos = Array.Find(fencePostionsCopy, x => x.position[0] == i && x.position[1] == j && x.rot == 3);
                    if (fencePos != null)
                    {
                        fencePostions.Add(new FencePostion() { position = new int[] { fencePlacementScript.pos.x, fencePlacementScript.pos.y }, rot = fencePlacementScript.rot, fenceLook = fencePos.fenceLook });
                        GenerateFence(fencePos.fenceLook, fencesPlacement[2].transform.GetChild(0).transform.position, 3);
                        fencesPlacement[2].transform.GetChild(0).tag = "PlacementOccupied";
                    }

                }
                if (j == 0)
                {
                    fencesPlacement[3] = Instantiate(fencePlacementPrefab, new Vector3(i, 0, j), Quaternion.Euler(0, 2 * 90, 0), transform);
                    fencePlacement fencePlacementScript = fencesPlacement[3].GetComponentInChildren<fencePlacement>();
                    fencePlacementScript.pos = new Vector2Int(i, j);
                    fencePlacementScript.rot = 2;

                    // create a fence if it was there on the terrain before
                    FencePostion fencePos = Array.Find(fencePostionsCopy, x => x.position[0] == i && x.position[1] == j && x.rot == 2);
                    if (fencePos != null)
                    {
                        fencePostions.Add(new FencePostion() { position = new int[] { fencePlacementScript.pos.x, fencePlacementScript.pos.y }, rot = fencePlacementScript.rot, fenceLook = fencePos.fenceLook });
                        GenerateFence(fencePos.fenceLook, fencesPlacement[3].transform.GetChild(0).transform.position, 2);
                        fencesPlacement[3].transform.GetChild(0).tag = "PlacementOccupied";
                    }

                }
                terrainParts[i].Add(new terrainPart() { block = Instantiate(terrainPartPrefab, new Vector3(i, -0.5f, j), Quaternion.Euler(-90, 0, 0), this.transform), objectPlacement = objPlacement, fencePlacement = fencesPlacement });
            }
        }
    }
    #endregion


    #region add object to the terrain
    private void Update()
    {
        pointerevent = new PointerEventData(eventsystem);
        pointerevent.position = Input.mousePosition;
        rayCastResults = new List<RaycastResult>();
        graphicraycaster.Raycast(pointerevent, rayCastResults);

        if (rayCastResults.Find(X => X.gameObject.tag == "ModifyTerrain").gameObject != null)
        {
            addObjectActionOnUpdate?.Invoke();
        }
    }

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
                        fencePostions.Add(new FencePostion() { position = new int[] { fencePlacementScript.pos.x, fencePlacementScript.pos.y }, rot = fencePlacementScript.rot, fenceLook = currentLook });
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
                        objectPositions.Add(new ObjectPosition() { position = new int[] { Mathf.RoundToInt(hit.transform.parent.position.x), Mathf.RoundToInt(hit.transform.parent.position.z) }, objectType = type });
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

    private void GenerateFence(int look, Vector3 position, int rotation)
    {
        GameObject fence = Instantiate(fences[look], position, Quaternion.Euler(0, rotation * 90, 0), GameObject.FindGameObjectWithTag("Terrain").transform);
    }

    private void GenerateObject(int objectType, Vector3 position)
    {
        Instantiate(objects[objectType], position, Quaternion.identity, GameObject.FindGameObjectWithTag("Terrain").transform);
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

}
