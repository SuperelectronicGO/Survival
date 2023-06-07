using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Netcode;
public class PooledObject : NetworkBehaviour
{
    public int2 position;
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            position = new int2(Mathf.RoundToInt((transform.position.x / 1000) - 0.5f), Mathf.RoundToInt((transform.position.z / 1000) - 0.5f));
            TileObjectList tile = CustomObjectFiltering.instance.terrainPositionList[position];
            tile.objectList.Add(gameObject);
        }
        catch
        {
            if (IsHost)
            {
                GetComponent<NetworkObject>().Despawn();
            } 
        }
    }
}
