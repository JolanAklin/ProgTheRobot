using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerOutlet : TerrainInteractableObj
{
    public uint powerPerTick = 100;

   
    public uint PowerPerTick { get => powerPerTick; set => powerPerTick = value; }
}
