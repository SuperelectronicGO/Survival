using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAdder : MonoBehaviour
{
    public GameObject[] slots;
    /*
    public void addItem(Item item)
    {
        
        int amount = item.amount;
        foreach (GameObject obj in slots)
        {
            if(obj.GetComponent<inventorySlot>().heldItem.itemType==item.itemType&& obj.GetComponent<inventorySlot>().heldItem.amount< obj.GetComponent<inventorySlot>().heldItem.MaxStack())
            {
               
                if (amount > obj.GetComponent<inventorySlot>().heldItem.MaxStack()-obj.GetComponent<inventorySlot>().heldItem.amount)
                {
                    amount -= (obj.GetComponent<inventorySlot>().heldItem.MaxStack() - obj.GetComponent<inventorySlot>().heldItem.amount);
                    obj.GetComponent<inventorySlot>().heldItem.amount = obj.GetComponent<inventorySlot>().heldItem.MaxStack();
                }
                else if (amount <= obj.GetComponent<inventorySlot>().heldItem.amount)
                {
                    obj.GetComponent<inventorySlot>().heldItem.amount += amount;
                    amount = 0;
                    break;
                }

            }



        }
        if (amount > 0)
        {
            bool sol=false;
            foreach(GameObject obj in slots)
            {
                if(obj.GetComponent<inventorySlot>().heldItem.itemType == Item.ItemType.Blank)
                {
                    obj.GetComponent<inventorySlot>().heldItem = item;
                    obj.GetComponent<inventorySlot>().heldItem.amount = amount;
                    sol = true;
                    break;
                }
            }
            if (!sol)
            {
                Debug.Log("Dropped" + item.itemName() + " " + amount);
                //drop item
            }
        }

        }
    */
}
