using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class DroppedItem : NetworkBehaviour
{
    private bool constructed = false;
    //Item being held
    public Item item;
    //Server Item struct used to sync data between the server and the clients
    public NetworkVariable<ItemNetworkStruct> itemStruct = new NetworkVariable<ItemNetworkStruct>(new ItemNetworkStruct(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        /* The server has the correct item automatically as its set after the spawn, 
         * and THEN the constructor is is called. The client needs to start the 
         * WaitOnItemStructSync() coroutine to wait until the itemStruct value is NOT blank,
         * at which point it will run the item constructor. There should never be an instance of a blank item dropping.*/
        //If we are not the server, wait until the item has been synced, then call the constructor

        if (!IsServer)
        {
            StartCoroutine(WaitOnItemStructSync());
        }
        
      
    }
   //Coroutine to wait on the item struct being synced
   private IEnumerator WaitOnItemStructSync()
    {
        while (itemStruct.Value.type == Item.ItemType.Blank)
        {
            yield return new WaitForEndOfFrame();
        }
        SyncClientAndConstruct();
        yield break;
    }
    //Method the client calls when the itemStruct data is syced to construct
    private void SyncClientAndConstruct()
    {
        item = itemStruct.Value.ToClass();
        if (TryGetComponent<ItemSpawnStart>(out ItemSpawnStart spawnStart))
        {
            spawnStart.RecieveItemType(item);
        }
    }
    //Method the server calls to construct the server object and sync its data
    public void SetItemStructValue(ItemNetworkStruct sentItemStruct)
    {
        
        itemStruct.Value = sentItemStruct;
        item = itemStruct.Value.ToClass();
        if (TryGetComponent<ItemSpawnStart>(out ItemSpawnStart spawnStart))
        {
            spawnStart.RecieveItemType(item);
        }
        

    }

}
