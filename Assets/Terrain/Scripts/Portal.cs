using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// start tpi
public class Portal : TerrainInteractableObj
{
    public static Portal currentPortal;

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
}
// end tpi
