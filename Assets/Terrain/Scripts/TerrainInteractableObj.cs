using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainInteractableObj : MonoBehaviour
{
    public enum ObjectType
    {
        none = 0,
        PowerPlug,
        Marking,
        ball
    }

    public ObjectType type;

    private List<Material[]> defaultMat = new List<Material[]>();
    public Renderer[] objectRenderers;
    public Material placeHolderMat;

    public void Awake()
    {
        foreach (Renderer renderer in objectRenderers)
        {
            defaultMat.Add(renderer.materials);
        }
    }

    public void PlaceHolderMat()
    {
        foreach (Renderer renderer in objectRenderers)
        {
            Material[] mats = new Material[renderer.materials.Length];
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                mats[i] = placeHolderMat;
            }
            renderer.materials = mats;
        }
    }

    public void PlacedMat()
    {
        int i = 0;
        foreach (Renderer renderer in objectRenderers)
        {
            renderer.materials = defaultMat[i];
            i++;
        }
    }
}
