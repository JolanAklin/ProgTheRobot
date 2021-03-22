using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainInteractableObj : MonoBehaviour
{
    public enum ObjectType
    {
        none = 0,
        PowerPlug,
    }

    public ObjectType type;
}
