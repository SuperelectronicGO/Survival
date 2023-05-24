using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAssets : MonoBehaviour
{
   public static ParticleAssets instance { get; private set; }
    private void Start()
    {
        instance = this;
    }
    public GameObject bloodParticle;
}
