using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoop : TerrainInteractableObj
{
    [SerializeField] private ParticleSystem winEffect;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "BallObj")
        {
            winEffect.Play();
        }
    }
}
