using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenSettings : MonoBehaviour
{
    public uint seed;
    public static WorldGenSettings instance;



    public uint tempMapSeed { get; private set; }
    public uint humidMapSeed { get; private set; }
    public uint voronoiMapSeed { get; private set; }
    public uint tseed;
    public uint hseed;
    public uint vseed;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Void that gets the first random value from the given seed, then uses that to seed another random to get three different seeds for noisemaps
    public void CalculateMapSeeds()
    {
        Unity.Mathematics.Random initialRandom = new Unity.Mathematics.Random(seed);
        uint seededValue = initialRandom.NextUInt();
        Unity.Mathematics.Random secondRandom = new Unity.Mathematics.Random(seededValue);
        tempMapSeed = secondRandom.NextUInt();
        humidMapSeed = secondRandom.NextUInt();
        voronoiMapSeed = secondRandom.NextUInt();
        tseed = tempMapSeed;
        hseed = humidMapSeed;
        vseed = voronoiMapSeed;
    }
}
