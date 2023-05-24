using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class NetworkStorage : NetworkBehaviour
{
    public int slotAmount = 18;
    public NetworkVariable<ItemNetworkStruct[]> heldItems;
    bool spawned = false;
    
    public override void OnNetworkSpawn()
    {
        //On the server, create a new array. If we are not on the server, we don't want to be creating a new array because we will be hosting
        heldItems = new NetworkVariable<ItemNetworkStruct[]>(new ItemNetworkStruct[slotAmount], NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        for(int i = 0; i< heldItems.Value.Length; i++)
        {
            heldItems.Value[i] = new ItemNetworkStruct();
        }
        spawned = true;
    }
    private void Update()
    {
        if (spawned)
        {
            Debug.Log(heldItems.Value[0].type.ToString() + ", " + heldItems.Value[1].type.ToString());
        }
    }
    /// <summary>
    /// Sets an item in the list of held items
    /// </summary>
    /// <param name="index">The index of the value to set</param>
    /// <param name="value">The ItemNetworkStruct value to set to</param>
    public void SetItem(int index, Item value)
    {
        heldItems.Value[index] = value.ToStruct();
    }
}
