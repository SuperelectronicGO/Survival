using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseMapData
{
    public float[,] noiseMap;
    public float[,] heatMap;    


    public NoiseMapData(MapGenerator generator)
    {
        noiseMap = generator.noiseMaps;
        heatMap = generator.heatMaps;
    }
}
