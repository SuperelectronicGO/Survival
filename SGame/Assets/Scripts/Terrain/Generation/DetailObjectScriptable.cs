using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Detail Scriptable", menuName = "Scriptables/Detail Scriptable", order = 5)]
[System.Serializable]
public class DetailObjectScriptable : ScriptableObject
{
    public string detailName;
    [Header("Direct Prototype Settings")]
    public Color dryColor;
    public Color healthyColor;
    public float holeEdgePadding;
    public float minHeight;
    public float maxHeight;
    public float minWidth;
    public float maxWidth;
    public int noiseSeed;
    public float noiseSpread;
    public bool usePrototypeMesh = false;
    [Header("Mesh/Texture")]
    public GameObject prototypeMesh;
    public Texture2D prototypeTexture;
    [Header("Detail Settings")]
    public GenerationType generationType;
    [Range(2, 4)]
    public int quadCount = 2;
    [Range(1, 5)]
    public int detailDensity=1;
    [Range(0, 1000)]
    public int spawnChance=0;

    [Header("Patch Settings")]
    [Range(0,1)]
    public float patchCutoff = 1;
    [Range(0, 1)]
    public float patchFadeMargin = 0.1f;
    public NoisemapSettingsScriptable noisemapSettings;


    public enum GenerationType
    {
        Flood,
        Patches
    }
}
