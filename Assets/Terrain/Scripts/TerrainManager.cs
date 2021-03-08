using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TerrainManager : MonoBehaviour
{
    public GameObject terrainPart;
    public uint maxSize;
    public TMP_InputField changeSizeXInputField;
    public TMP_InputField changeSizeYInputField;


    private uint[] terrainSize;

    private List<List<GameObject>> terrainParts = new List<List<GameObject>>();

    // Start is called before the first frame update
    void Start()
    {
        terrainSize = new uint[] { 10, 10 };
        changeSizeXInputField.text = terrainSize[0].ToString();
        changeSizeYInputField.text = terrainSize[1].ToString();
        CreateTerrain(terrainSize);
    }

    // Update is called once per frame
    void Update()
    {
        
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
            foreach (List<GameObject> terrainPartlist in terrainParts)
            {
                foreach (GameObject terrainPart in terrainPartlist)
                {
                    Destroy(terrainPart);
                }
            }
            terrainParts.Clear();
        }
        for (int i = 0; i < size[0]; i++)
        {
            terrainParts.Add(new List<GameObject>());
            for (int j = 0; j < size[1]; j++)
            {
                terrainParts[i].Add(Instantiate(terrainPart, new Vector3(i, -0.5f, j), Quaternion.Euler(-90,0,0), this.transform));
            }
        }
    }
    #endregion
}
