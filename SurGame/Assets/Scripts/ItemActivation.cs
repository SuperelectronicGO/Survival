using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemActivation : MonoBehaviour
{
    public InventoryManager manager;
    public GameObject[] hotbarSlots;
    public Hotbarmanager hotbarManager;

    public GameObject spawnZone;
    public ItemAssets assets;
    public void UseItem(Item item)
        {
            switch (item.itemType)
            {
                case Item.ItemType.Grenade:

                    foreach (GameObject slot in hotbarSlots)
                    {
                        if (slot.GetComponent<HotbarSlot>().slotNumber == hotbarManager.activeSlot)
                        {
                            if (item.amount > 1)
                            {
                                slot.GetComponent<inventorySlot>().heldItem.amount -= 1;

                            }else if(item.amount == 1)
                            {
                                slot.GetComponent<inventorySlot>().heldItem = manager.blankItem;
                            }
                        Vector3 spawnPos = new Vector3(spawnZone.transform.position.x, spawnZone.transform.position.y, spawnZone.transform.position.z);
                        Instantiate(assets.grenade, spawnPos, Quaternion.identity);
                    }
                    }

                    break;
            }






        }
   
    
}
