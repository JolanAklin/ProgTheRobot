using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerOutlet : TerrainInteractableObj
{
    public uint powerPerTick = 100;

    public ParticleSystem particleSystem;

   
    public uint PowerPerTick { get => powerPerTick; set => powerPerTick = value; }

    public void StartParticleSystem()
    {
        particleSystem.Play();
    }

    public void StopParticleSystem()
    {
        particleSystem.Stop();
    }
}
