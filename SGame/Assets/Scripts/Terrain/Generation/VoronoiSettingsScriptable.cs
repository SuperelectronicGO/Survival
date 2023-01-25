using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Voronoi Settings", menuName = "Scriptables/VoronoiSettings", order = 7)]

public class VoronoiSettingsScriptable : ScriptableObject
{
    public int mapSize;
    public int regionAmount;
    public int seed;
}
