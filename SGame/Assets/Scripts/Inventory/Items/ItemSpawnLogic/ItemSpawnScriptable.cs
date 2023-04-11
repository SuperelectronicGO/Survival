using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
[CreateAssetMenu(fileName = "ObjectSpawnScriptable", menuName = "Scriptables/Object Spawn Scriptable", order = 8)]
public class ItemSpawnScriptable : ScriptableObject
{
    [Header("Visable Object")]
    public Mesh mesh;
    public Material[] materials;
    public Vector3 spawnedScale;
    public Vector3 spawnedRotation;
    [Header("Collider")]
    public ColliderType colliderType;
    public Vector3 colliderOffset;
    public Vector3 colliderScale;
    public float colliderRadius;
    public float colliderHeight;
    [Range(0,2)]
    public int colliderDirection;
    public enum ColliderType
    {
        box,
        cylinder,
        sphere,
        capsule
    }
}
