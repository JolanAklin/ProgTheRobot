using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TerrainManager : MonoBehaviour
{
    public GameObject terrainPartPrefab;
    public GameObject fencePlacementPrefab;
    public GameObject objectPlacement;
    public uint maxSize;
    public TMP_InputField changeSizeXInputField;
    public TMP_InputField changeSizeYInputField;


    private uint[] terrainSize;

    private List<List<terrainPart>> terrainParts = new List<List<terrainPart>>();


    // raycast stuff
    public GameObject canvas;
    GraphicRaycaster graphicraycaster;
    PointerEventData pointerevent;
    EventSystem eventsystem;
    List<RaycastResult> rayCastResults = new List<RaycastResult>();
    public Camera terrainCam;

    public RenderTexture rt;
    public RectTransform robotImage;

    public class terrainPart
    {
        public GameObject block;
        public GameObject objectPlacement;
        public GameObject[] fencePlacement;
    }

    // using an uint to store the different look of fence
    private uint[][] fencesPlacement;

    // Start is called before the first frame update
    void Start()
    {
        terrainSize = new uint[] { 10, 10 };
        changeSizeXInputField.text = terrainSize[0].ToString();
        changeSizeYInputField.text = terrainSize[1].ToString();
        CreateTerrain(terrainSize);

        graphicraycaster = canvas.GetComponent<GraphicRaycaster>();
        eventsystem = canvas.GetComponent<EventSystem>();

        terrainCam.targetTexture.Release();
        rt.width = Screen.width;
        rt.height = Screen.height;
        rt.Create();
        robotImage.sizeDelta = new Vector2(Screen.width, Screen.height);
        Debug.Log(Screen.height + ", " + Screen.width);
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
        if(terrainParts != null)
        {
            foreach (List<terrainPart> terrainPartlist in terrainParts)
            {
                foreach (terrainPart terrainPart in terrainPartlist)
                {
                    Destroy(terrainPart.block);
                    foreach (GameObject fencePlacement in terrainPart.fencePlacement)
                    {
                        Destroy(fencePlacement);
                    }
                }
            }
            terrainParts.Clear();
        }
        for (int i = 0; i < size[0]; i++)
        {
            terrainParts.Add(new List<terrainPart>());
            for (int j = 0; j < size[1]; j++)
            {
                GameObject[] fencesPlacement = new GameObject[4];
                for (int k = 0; k < 2; k++)
                {
                    fencesPlacement[k] = Instantiate(fencePlacementPrefab, new Vector3(i, 0, j), Quaternion.Euler(0, k * 90, 0), transform);
                }
                if(i == 0)
                    fencesPlacement[2] = Instantiate(fencePlacementPrefab, new Vector3(i, 0, j), Quaternion.Euler(0, 3 * 90, 0), transform);
                if (j == 0)
                    fencesPlacement[3] = Instantiate(fencePlacementPrefab, new Vector3(i, 0, j), Quaternion.Euler(0, 2 * 90, 0), transform);
                terrainParts[i].Add(new terrainPart() { block = Instantiate(terrainPartPrefab, new Vector3(i, -0.5f, j), Quaternion.Euler(-90, 0, 0), this.transform), objectPlacement = Instantiate(objectPlacement, new Vector3(i, 0, j), Quaternion.identity, this.transform), fencePlacement = fencesPlacement });
            }
        }
    }
    #endregion

    private void ChangeFenceArray()
    {

    }


    private void Update()
    {
        pointerevent = new PointerEventData(eventsystem);
        pointerevent.position = Input.mousePosition;
        rayCastResults = new List<RaycastResult>();
        graphicraycaster.Raycast(pointerevent, rayCastResults);

        if (rayCastResults.Find(X => X.gameObject.tag == "ModifyTerrain").gameObject != null)
        {
            RaycastHit hit;
            Ray ray = terrainCam.ScreenPointToRay(Input.mousePosition);
            //if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
            Vector2 mousepos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            if (Physics.Raycast(terrainCam.transform.position, terrainCam.ScreenPointToRay(mousepos).direction, out hit, Mathf.Infinity))
            {
                if (currentFence == null)
                    currentFence = CreateFence();
                else
                {
                    currentFence.transform.position = hit.transform.position;
                    currentFence.transform.rotation = hit.transform.rotation;
                }
            }
        }
    }

    public GameObject[] fences;

    public Material showMat;
    public Material finalMat;

    private GameObject currentFence;

    public GameObject CreateFence()
    {
        GameObject fence = Instantiate(fences[Random.Range(0, fences.Length)], transform.position, transform.rotation, GameObject.FindGameObjectWithTag("Terrain").transform);
        foreach (MeshRenderer meshRenderer in fence.transform.GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.material = showMat;
        }
        return fence;
    }
}
