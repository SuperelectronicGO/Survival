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
    public float minTemp;
    public float maxTemp;
    public float minHum;
    public float maxHum;
    [NonReorderable]
    public SubBiome[] subBiomes;
}
[System.Serializable]
public class SubBiome
{
    public enum BiomeType
    {
        Spruce_Dead,
        Spruce_Barren,
        Spruce_Lush,
        Oak_Lush,
        Plains_Lush


    }

    public string name;
    public BiomeType biomeType;
    [Header("Biome Texture")]
    public TerrainLayer biomeLayer;
    [Header("")]
    [Header("Subbiome Data")]
    [Range(0, 1000)]
    public int biomeDensity;
    [Range(0, 1)]
    public float minCutoff;
    [Range(0, 1)]
    public float maxCutoff;
    [Header("")]
    [Header("Biome Objects")]
    [NonReorderable]
    public ObjectScriptable[] biomeObjects;
    [Header("")]
    [Header("Detail Objects")]
    [NonReorderable]
    public DetailObjectScriptable[] details;
}


