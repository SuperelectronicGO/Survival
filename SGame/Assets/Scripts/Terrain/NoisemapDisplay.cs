using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class NoisemapDisplay : MonoBehaviour
{
    [SerializeField] private NoisemapSettingsScriptable settings;
    [SerializeField] private VoronoiSettingsScriptable voronoiSettings;

    [SerializeField] private MapDisplay display;
    public void displayMap()
    {
        int2 dimensions = new int2(settings.mapWidth, settings.mapHeight);
        display.DrawNoiseMap(TerrainNoise.GenerateNoiseMap(dimensions, (uint)settings.seed, settings.scale, settings.octaves, settings.persistance, settings.lacunarity, new int2(0, 0)));
    }
}
