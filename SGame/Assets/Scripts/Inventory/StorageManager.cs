using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class StorageManager : MonoBehaviour
{
    public static StorageManager instance { get; private set; }
    public NetworkStorage currentNetworkStorage { get; private set; }

    public ISInterface[] slots;
    public ISInterface[] activeSlots;
    // Start is called before the first frame update
    void Awake()
    {
        //Set instance
        instance = this;
        //Set the index of each storage slot that will correspond with the index in a storages ItemNetworkStruct array.
        for(int i = 0; i < slots.Length; i++)
        {
            (slots[i] as ChestSlot).SetStorageIndex(i);
        }
    }

    /// <summary>
    /// Sets what slots are active when you open a storage. Active slots are enabled and added to the activeSlot array.
    /// </summary>
    /// <param name="amnt">How many slots to set active</param>
    public void SetActiveSlots(int amnt)
    {
        activeSlots = new ISInterface[amnt];
        for(int i=0; i<slots.Length; i++)
        {
            if (i < amnt)
            {
                slots[i].gameObject.SetActive(true);
                activeSlots[i] = slots[i];
            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }
            
        }
    }
    //Refresh the slot values
    public void RefreshSlotValues()
    {
        for(int i=0; i<activeSlots.Length; i++)
        {
            activeSlots[i].updateSlotValues();
        }
    }
    /// <summary>
    /// Calls the SetItem method on the current NetworkStorage to set one of the items in the ItemNetworkStruct array.
    /// </summary>
    /// <param name="index">The index of the value to set</param>
    /// <param name="value">The item value we want to set as</param>
    public void SetCurrentStorageValue(int index, Item value)
    {
        currentNetworkStorage.SetItem(index, value);
    }
    /// <summary>
    /// Sets the current network storage to use
    /// </summary>
    /// <param name="storageReference">The NetworkStorage reference to set as</param>
    public void SetCurrentNetworkStorage(NetworkStorage storageReference)
    {
        currentNetworkStorage = storageReference;
    }
    /// <summary>
    /// Sets the current slots to display the contents of the current NetworkStorage
    /// </summary>
    public void SetSlotValues()
    {
        for(int i = 0; i < currentNetworkStorage.heldItems.Value.items.Length; i++)
        {
            activeSlots[i].heldItem = currentNetworkStorage.heldItems.Value.items[i].ToClass();
       }
        RefreshSlotValues();
    }
}
