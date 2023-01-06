using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Biome Storage", menuName = "Scriptables/Biome Storage", order = 3)]
public class BiomeStorage : ScriptableObject
{
    [NonReorderable]
    public TerrainBiome[] biomes;
    


}
[System.Serializable]
public class TerrainBiome
{
    public enum BiomeType
    {
      OakForest,
      SpruceForest,
      Plains,
      DryPlains,


    }

    public string name;
    public BiomeType biomeType;
    [Header("Biome Texture")]
    public TerrainLayer biomeLayer;
    [Header("")]
    [Header("Biome Settings")]
    public float minTemp;
    public float maxTemp;
    public float minHum;
    public float maxHum;
    public float[,] splatMap;
    [Header("Chance from 0-1000")]
    [Range(0,1000)]
    public int biomeDensity;
    [Header("")]
    [Header("Biome Objects")]
    [NonReorderable]
    public GenerateableObject[] biomeObjects;
    [Header("")]
    [Header("Detail Objects")]
    [NonReorderable]
    public DetailObjectScriptable[] details;
}

[System.Serializable]
public class GenerateableObject
{
    public string name;
    public enum typeOfObject
    {
        Tree,
        Rock,
        Foliage,
        Misc
    }
    public typeOfObject objType;
    public GameObject prefab;
    public AnimationCurve scale;
    public float yOffset;
    public bool alignNormals;
    public bool offsetHeightByNormals;
    [Range(0,1000)]
    public int weight;

    
    
    
}
