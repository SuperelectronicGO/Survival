using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Generation Speed Settings Scriptable", menuName = "Scriptables/Generation Speed Settings Scriptable", order = 6)]
[System.Serializable]
public class GenerationSpeedSettings : ScriptableObject
{
    [Header("For loop settings")]
    public int maxIterationsPerFrame;
    public int maxLoopBeforeWait;
    [Header("Terrain Settings")]
    public int splatmapStrips;
}
