using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newObjectScriptable", menuName = "Scriptables/Object Scriptable", order = 8)]
[System.Serializable]
public class ObjectScriptable : ScriptableObject
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
    public Vector3 scaleMultiplier = Vector3.one;
    public float yOffset;
    public bool alignNormals;
    public bool offsetHeightByNormals;
    [Range(0, 1000)]
    public int weight;
    [Header("Materials")]
    public bool terrainBlending;
    public Material defaultMaterial;
}
