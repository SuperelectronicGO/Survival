using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSeed : MonoBehaviour
{
    public MapMagic.Core.MapMagicObject M_Graph;
    public int seed;
    // Start is called before the first frame update
    void Start()
    {
        seed = Random.Range(0, 100000);
        M_Graph.graph.random = new Den.Tools.Noise(seed, 32768);


    }

    // Update is called once per frame
    void Update()
    {
       
    }

  
}
