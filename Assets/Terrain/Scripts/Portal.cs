using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// start tpi
public class Portal : TerrainInteractableObj
{
    public static Portal currentPortal;
    public static int nextid = 0;

    public LayerMask robotLayerMask;

    public TerrainManager.ObjectPosition objectPosition;

    private Portal linkedPortal;
    public Portal LinkedPortal { get => linkedPortal; set => linkedPortal = value; }

    private Color portalColor;
    public Color PortalColor { get => portalColor; set => portalColor = value; }

    private int id;
    public int Id { get => id; set => id = value; }

    public void PortalPlaced()
    {
        if (linkedPortal == null)
        {
            id = nextid;
            nextid++;
        }
        // testing for linked portal because if it set when the object is instantiated, it won't be null, so keeping it's configuration
        if (currentPortal != null && linkedPortal == null)
        {

            LinkedPortal = currentPortal;
            LinkedPortal.LinkedPortal = this;
            currentPortal = null;

            portalColor = Random.ColorHSV(0, 1, 0.8f, 1, 0.8f, 1, 1, 1);

            objectPosition.optionalSettings = new List<string>();
            objectPosition.optionalSettings.Add(id.ToString());
            objectPosition.optionalSettings.Add(linkedPortal.id.ToString());
            objectPosition.optionalSettings.Add($"{portalColor.r},{portalColor.g},{portalColor.b},{portalColor.a}");

            LinkedPortal.objectPosition.optionalSettings = new List<string>();
            LinkedPortal.objectPosition.optionalSettings.Add(linkedPortal.id.ToString());
            LinkedPortal.objectPosition.optionalSettings.Add(id.ToString());
            LinkedPortal.objectPosition.optionalSettings.Add($"{portalColor.r},{portalColor.g},{portalColor.b},{portalColor.a}");

            LinkedPortal.portalColor = portalColor;

            SetDefaultMat();
            linkedPortal.SetDefaultMat();

            PlacedMat();
            LinkedPortal.PlacedMat();
        }
        else
            currentPortal = this;
    }

    public void SetDefaultMat()
    {
        foreach (Material[] mats in defaultMat)
        {
            foreach (Material material in mats)
            {
                material.color = portalColor;
            }
        }
    }

    /// <summary>
    /// Check if there is a robot on this portal
    /// </summary>
    /// <returns>True if there isn't a robot in the portal</returns>
    public bool CheckAvailibility()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, new Vector3(0.2f, 0.2f, 0.2f), Quaternion.identity, robotLayerMask);
        if (colliders.Length > 0)
        {
            return false;
        }
        return true;
    }

    private void OnDestroy()
    {
        // destroy the linked portal too
        if(LinkedPortal != null)
        {
            Destroy(LinkedPortal.gameObject);
        }
    }
}
// end tpi
