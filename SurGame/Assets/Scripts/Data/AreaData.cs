using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu()]
public class AreaData : UpdatableData
{
    public Zone[] zones;
}
[System.Serializable]
public class Zone
{
    public Vector2 middleChunkCoord;
    public int size;
    public GameObject spawnedObject;
}
