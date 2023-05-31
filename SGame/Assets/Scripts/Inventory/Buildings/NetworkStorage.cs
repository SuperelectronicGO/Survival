using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class NetworkStorage : NetworkBehaviour
{
    //How many slots this chest has
    public int slotAmount = 18;
    //List of held items
    public NetworkVariable<NetworkStorageItemWrapper> heldItems = new NetworkVariable<NetworkStorageItemWrapper>(new NetworkStorageItemWrapper(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    //bool spawned = false;
    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            heldItems.Value = new NetworkStorageItemWrapper { items = new ItemNetworkStruct[slotAmount] };
            for (int i = 0; i < slotAmount; i++)
            {
                heldItems.Value.items[i] = new ItemNetworkStruct
                {
                    type = Item.ItemType.Campfire,
                    amount = 0,
                    attributeNames = new ItemAttribute.AttributeName[0],
                    attributeValues = new float[0],
                    attributeInfo = new NetworkString512Bytes(),
                    spellType = Spell.SpellType.None,
                    spellAttributeTypes = new SpellAttribute.AttributeType[0],
                    spellAttributeValues = new float[0]

                };
            }
        }
        //spawned = true;
        heldItems.OnValueChanged += (NetworkStorageItemWrapper previousValue, NetworkStorageItemWrapper newValue) =>
        {
            if (StorageManager.instance.currentNetworkStorage == this)
            {
                StorageManager.instance.SetSlotValues();
            }
        };
    }

    //Wrapper class for the list of held items
    [System.Serializable]
    public struct NetworkStorageItemWrapper : INetworkSerializable
    {
        [NonReorderable]
        public ItemNetworkStruct[] items;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref items);
        }
    }
    /// <summary>
    /// Sets an item in the list of held items
    /// </summary>
    /// <param name="index">The index of the value to set</param>
    /// <param name="value">The Item value to set to</param>
    public void SetItem(int index, Item value)
    {
        if (IsHost)
        {
            //  heldItems.Value[index] = value.ToStruct();
            heldItems.Value.items[index] = value.ToStruct();
            heldItems.SetDirty(true);
        }
        else
        {
            SetStorageItemServerRPC(index, value.ToStruct());
        }
    }
    /// <summary>
    /// Server RPC that does the same as SetItem, called on the client
    /// </summary>
    /// <param name="index">The index of the value to set</param>
    /// <param name="value">The ItemNetworkStruct value to set to</param>
    [ServerRpc(RequireOwnership = false)]
    public void SetStorageItemServerRPC(int index, ItemNetworkStruct value)
    {
        heldItems.Value.items[index] = value;
        heldItems.SetDirty(true);
        if(StorageManager.instance.currentNetworkStorage == this)
        {
            StorageManager.instance.SetSlotValues();
        }
    }
}

