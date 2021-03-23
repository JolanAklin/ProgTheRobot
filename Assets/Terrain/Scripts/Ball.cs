using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : TerrainInteractableObj
{
    public GameObject parent;
    public Transform ballObjectDefaultPos;
    public GameObject ballObject;
    public ObjectPlacement DefaultObjectPlacement;
    public ObjectPlacement currentObjectPlacement;
    public bool ballTaken;
    public LayerMask objectLayer;

    public void GetObjectPlacement()
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), transform.TransformDirection(Vector3.down), out hit, 1, objectLayer))
        {
            hit.collider.TryGetComponent(out DefaultObjectPlacement);
            currentObjectPlacement = DefaultObjectPlacement;
        }
    }

    public void BallReset()
    {
        parent.transform.position = DefaultObjectPlacement.transform.position;
        parent.transform.rotation = DefaultObjectPlacement.transform.rotation;
        ballTaken = false;

        ballObject.transform.position = ballObjectDefaultPos.position;
        ballObject.transform.rotation = ballObjectDefaultPos.rotation;


        currentObjectPlacement.tag = "Untagged";
        currentObjectPlacement.terrainObject = null;

        DefaultObjectPlacement.tag = "PlacementOccupied";
        DefaultObjectPlacement.terrainObject = gameObject;
        ballObject.GetComponent<Rigidbody>().isKinematic = true;
    }
}
