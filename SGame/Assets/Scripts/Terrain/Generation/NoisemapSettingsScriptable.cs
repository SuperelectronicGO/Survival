using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Noisemap Settings", menuName = "Scriptables/NoisemapSettings", order = 4)]

public class NoisemapSettingsScriptable : ScriptableObject
{
    public int mapWidth;
    public int mapHeight;
    public float scale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public int seed;
}
