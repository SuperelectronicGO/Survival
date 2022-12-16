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
    [Header("")]
    [Header("Biome Objects")]
    [NonReorderable]
    public GenerateableObject[] biomeObjects;
    [Header("Biome Object Settings")]
    public Vector2 treeAmount;
    public Vector2 rockAmount;

}

[System.Serializable]
public class GenerateableObject
{
    public string name;
    public enum typeOfObject
    {
        Tree,
        Rock
    }
    public typeOfObject objType;
    public GameObject prefab;
    public Vector2 newScale;
    [Range(0,1000)]
    public int weight;

    
    
    
}
