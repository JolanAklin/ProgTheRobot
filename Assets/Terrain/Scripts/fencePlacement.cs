using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fencePlacement : MonoBehaviour
{
    public GameObject[] fences;

    public Material showMat;
    public Material finalMat;

    private GameObject currentFence;

    public void Show(bool show)
    {
        if (show)
        {
            if(currentFence == null)
            {
                currentFence = Instantiate(fences[Random.Range(0, fences.Length)], transform.position, transform.rotation, GameObject.FindGameObjectWithTag("Terrain").transform);
                foreach (MeshRenderer meshRenderer in currentFence.transform.GetComponentsInChildren<MeshRenderer>())
                {
                    meshRenderer.material = showMat;
                }
            }
        }
        else
        {
            Destroy(currentFence);
        }
    }
}
