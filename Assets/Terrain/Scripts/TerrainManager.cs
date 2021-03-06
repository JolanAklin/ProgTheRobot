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
    public uint[] TerrainSize { get => terrainSize; private set => terrainSize = value; }

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
        WindowResized.instance.onWindowResized += MatchRTToScreen;


        terrainSize = new uint[] { 10, 10 };
        changeSizeXInputField.text = terrainSize[0].ToString();
        changeSizeYInputField.text = terrainSize[1].ToString();

        CreateTerrain(terrainSize);

        // fill the object list
        objectList.AddChoice(new List.ListElement()
        {
            displayedText = "Barri?res",
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
            displayedText = "Arriv?e",
            actionOnClick = () => { 
                addObjectActionOnUpdate = () => { AddObject(objects[3], 3); };
                if (currentObject != null)
                    Destroy(currentObject);
            }
        });
        // start tpi
        objectList.AddChoice(new List.ListElement()
        {
            displayedText = "Porte temporelle",
            actionOnClick = () =>
            {
                addObjectActionOnUpdate = () => { AddObject(objects[4], 4); } ;
                if (currentObject != null)
                    Destroy(currentObject);
            }
        });
        // end tpi

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
                // start tpi
                RobotManager manager = robotToMove.GetComponent<RobotManager>();
                // keep the cameraPoint at the same position
                Vector3 oldPos = manager.cameraPoint.transform.position;
                robotToMove.transform.position = new Vector3(hit.transform.position.x, 0.5f, hit.transform.position.z);
                manager.cameraPoint.transform.position = oldPos;
                // end tpi
                if(Input.GetKeyDown(KeyCode.Mouse0))
                {
                    // start tpi
                    // test if there is another robot on the same spot
                    foreach (Robot robot in Robot.robots.Values)
                    {
                        robot.robotManager.UpdatePosOnGrid();
                        if (manager.PosOnGridInt == robot.robotManager.PosOnGridInt && robot.robotManager != manager)
                            return;
                    }
                    // end tpi
                    addObjectActionOnUpdate = null;
                    manager.SetDefaultPos(robotToMove.transform);
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
    private void MatchRTToScreen(object sender, WindowResized.WindowResizedEventArgs e)
    {
        rt.Release();
        terrainCam.targetTexture = null;
        rt.width = e.screenWidth;
        rt.height = e.screenHeight;
        rt.Create();
        robotImage.sizeDelta = new Vector2(e.screenWidth, e.screenHeight);
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
        // start tpi
        if(!(terrainSize[0] * terrainSize[1] >= Robot.robots.Count))
        {
            terrainSize[0] = Convert.ToUInt32(Mathf.CeilToInt((float)Robot.robots.Count / (float)terrainSize[1]));
            changeSizeXInputField.text = terrainSize[0].ToString();
        }
        CreateTerrain(terrainSize);
        // end tpi

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
        // start tpi
        if (!(terrainSize[0] * terrainSize[1] >= Robot.robots.Count))
        {
            terrainSize[1] = Convert.ToUInt32(Mathf.CeilToInt((float)Robot.robots.Count / (float)terrainSize[0]));
            changeSizeYInputField.text = terrainSize[1].ToString();
        }
        CreateTerrain(terrainSize);
        // end tpi
    }

    public void CreateTerrain(uint[] size)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        terrainParts.Clear();

        // start tpi
        foreach (Robot robot in Robot.robots.Values)
        {
            // grid start at zero
            if(robot.robotManager.transform.position.x > size[0] - 1 || robot.robotManager.transform.position.z > size[1] - 1)
            {
                robot.robotManager.transform.position = Manager.instance.GetNextPlaceForRobot();
                robot.robotManager.UpdatePosOnGrid();
            }
        }
        // end tpi


        FencePostion[] fencePostionsCopy = new FencePostion[fencePostions.Count];
        fencePostions.CopyTo(fencePostionsCopy);
        fencePostions.Clear();

        ObjectPosition[] objectPositionsCopy = new ObjectPosition[objectPositions.Count];
        objectPositions.CopyTo(objectPositionsCopy);
        objectPositions.Clear();

        // start tpi
        Dictionary<Portal, ObjectPosition> portals = new Dictionary<Portal, ObjectPosition>();
        // end tpi

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

                    // tpi
                    if (objPosition.objectType == 4)
                    {
                        // set the id of the portal if there is all the other arguments
                        Portal portal = objectPlacementScript.terrainObject.GetComponent<Portal>();
                        if(objectPosition.optionalSettings.Count == 3)
                        {
                            portal.Id = Convert.ToInt32(objectPosition.optionalSettings[0]);
                            portals.Add(portal, objectPosition);
                        }
                    }
                    // end tpi
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

        // start tpi
        // set the portal id, linkedPortal and color of each portal
        foreach (KeyValuePair<Portal, ObjectPosition> portal in portals)
        {
            if(portal.Value.optionalSettings.Count == 3)
            {
                portal.Key.LinkedPortal = portals.First(x => x.Key.Id == Convert.ToInt32(portal.Value.optionalSettings[1])).Key;
                string[] colorValues = portal.Value.optionalSettings[2].Split(',');
                portal.Key.PortalColor = new Color(float.Parse(colorValues[0]), float.Parse(colorValues[1]), float.Parse(colorValues[2]), float.Parse(colorValues[3]));
                portal.Key.SetDefaultMat();
                portal.Key.PlacedMat();
            }
        }
        // end tpi
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
            {
                currentFence = CreateFence();
            }
            else
            {
                if (hit.transform.gameObject.tag != "PlacementOccupied")
                {
                    if(!currentFence.activeSelf)
                        currentFence.SetActive(true);

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
                else
                {
                    if (currentFence.activeSelf)
                        currentFence.SetActive(false);
                }
            }
        }
        else
        {
            if(currentFence != null)
            {
                if (currentFence.activeSelf)
                    currentFence.SetActive(false);
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
            {
                currentObject = CreateObject(prefab);
            }
            else
            {
                if (hit.transform.gameObject.tag != "PlacementOccupied")
                {
                    if(!currentObject.activeSelf)
                        currentObject.SetActive(true);

                    currentObject.transform.rotation = hit.transform.rotation;
                    currentObject.transform.position = hit.transform.position;

                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {

                        ObjectPlacement objectPlacement = hit.transform.GetComponent<ObjectPlacement>();
                        ObjectPosition objPosition = new ObjectPosition() { position = new int[] { Mathf.RoundToInt(hit.transform.parent.position.x), Mathf.RoundToInt(hit.transform.parent.position.z) }, objectType = type };

                        // tpi
                        if(objPosition.objectType == 4)
                        {
                            Portal portal = currentObject.GetComponent<Portal>();
                            if(portal.objectPosition != null)
                                portal.objectPosition = objPosition;
                            portal.PortalPlaced();
                        }
                        // end tpi
                        
                        objectPositions.Add(objPosition);

                        objectPlacement.terrainObject = currentObject;
                        objectPlacement.objectPosition = objPosition;

                        hit.transform.gameObject.tag = "PlacementOccupied";
                        PlaceObject(currentObject);
                        currentObject = CreateObject(prefab);
                    }
                }
                else
                {
                    if(currentObject.activeSelf)
                        currentObject.SetActive(false);
                }
            }
        }
        else
        {
            if(currentObject != null)
            {
                if (currentObject.activeSelf)
                    currentObject.SetActive(false);

            }
        }
    }

    public void RemoveCurrentObject()
    {
        if(currentFence != null)
        {
            Destroy(currentFence);
        }
        if(currentObject != null)
        {
            Destroy(currentObject);
        }
    }

    public void ResetTerrain()
    {
        foreach (Robot robot in Robot.robots.Values)
        {
            robot.robotManager.ResetTerrainObj();
            robot.robotManager.transform.position = robot.robotManager.robotStartPos;
            robot.robotManager.transform.rotation = robot.robotManager.robotStartRot;
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
        fence.SetActive(false);
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
        instance.SetActive(false);

        TerrainInteractableObj tio;
        if((tio = instance.GetComponentInChildren<TerrainInteractableObj>()) != null)
        {
            tio.PlaceHolderMat();
        }
        return instance;
    }

    private void PlaceObject(GameObject objectToPlace)
    {
        TerrainInteractableObj tio;
        if ((tio = objectToPlace.GetComponentInChildren<TerrainInteractableObj>()) != null)
        {
            tio.PlacedMat();
        }
        else
        {
            foreach (MeshRenderer meshRenderer in objectToPlace.transform.GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.material = finalMat;
            }
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
        // start tpi
        [SerializeField]
        public List<string> optionalSettings;
        // end tpi
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
