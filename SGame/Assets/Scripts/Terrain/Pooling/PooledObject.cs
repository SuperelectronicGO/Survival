using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Netcode;
public class PooledObject : NetworkBehaviour
{
    public int2 position;
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
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
                StartCoroutine(DespawnAfterTime());
            } 
        }
    }

    //Despawn void to remove from object filtering
    public override void OnNetworkDespawn()
    {
        try
        {
            position = new int2(Mathf.RoundToInt((transform.position.x / 1000) - 0.5f), Mathf.RoundToInt((transform.position.z / 1000) - 0.5f));
            TileObjectList tile = CustomObjectFiltering.instance.terrainPositionList[position];
            tile.objectList.Remove(gameObject);
        }
        catch
        {
            //Object is being despawned due to an invalid position.
        }
    }
    /// <summary>
    /// Coroutine that despawns the object after two seconds to avoid network key errors when an invalid tile placement position is found.
    /// </summary>
    /// <returns>After despawning the object</returns>
    private IEnumerator DespawnAfterTime()
    {
        yield return new WaitForSecondsRealtime(2);
        GetComponent<NetworkObject>().Despawn();
        yield break;
    }
}
