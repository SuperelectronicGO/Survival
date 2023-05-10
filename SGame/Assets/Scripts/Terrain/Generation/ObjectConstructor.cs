using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ObjectConstructor : NetworkBehaviour
{
    public enum ObjectType
    {
        LeafyHemlock,
        LeafyPine,
        ScragglyHemlock,
        ScragglyPine,
        MossyBirch,
        DyLeafyHemlock,
        DryLeafyPine,
        DryScragglyHemlock,
        DryScragglyPine,
        DeadTree,
        RottenStump,
        RockOne,
        RockTwo
    }
    public ObjectType type;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
