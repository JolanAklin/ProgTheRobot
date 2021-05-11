using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// start tpi
public class Portal : TerrainInteractableObj
{
    public static Portal currentPortal;

    public LayerMask robotLayerMask;

    private Portal linkedPortal;

    public Portal LinkedPortal { get => linkedPortal; private set => linkedPortal = value; }

    private void Start()
    {
        if (currentPortal != null)
        {
            LinkedPortal = currentPortal;
            LinkedPortal.LinkedPortal = this;
            currentPortal = null;

            Color portalColor = Random.ColorHSV(0, 1, 0.8f, 1, 0.8f, 1, 1, 1);
            foreach (Material[] mats in defaultMat)
            {
                foreach (Material material in mats)
                {
                    material.color = portalColor;
                }
            }
            foreach (Material[] mats in LinkedPortal.defaultMat)
            {
                foreach (Material material in mats)
                {
                    material.color = portalColor;
                }
            }
            PlacedMat();
            LinkedPortal.PlacedMat();
        }
        else
            currentPortal = this;
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
}
// end tpi
