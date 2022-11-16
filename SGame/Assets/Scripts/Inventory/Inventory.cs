using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Inventory : MonoBehaviour
{
    [Header("Inventory Components")]
    [Tooltip("Gameobject that is the parent of all inventory slots")]
    [SerializeField] private GameObject slotParent;
    [SerializeField] private GameObject hotbarParent;
    public GameObject tooltipObject;
    public Item blankItem;
    public Item mouseItem;
    public Canvas canvas;
    public Image mouseImage;
    public TextMeshProUGUI mouseText;
    [Header("Other Components")]
    public GameObject player;
    public CraftingManager cManager;
    //Used to set the multiplicitave alpha value to fade in and fade out the inventory during animations
    public float inventoryAlphaValue = -0.1f;
    //Bool to control if alpha being set
    public bool setAlpha;
    //List of all slots
    private List<InventorySlot> slots = new List<InventorySlot>();
    void Start()
    {
        slots.Clear();
        for (int i = 0; i < hotbarParent.transform.childCount; i++)
        {
            if (hotbarParent.transform.GetChild(i).transform.name.Contains("HotbarSlot"))
            {
                slots.Add(hotbarParent.transform.GetChild(i).GetComponent<InventorySlot>());
                hotbarParent.transform.GetChild(i).GetComponent<InventorySlot>().canvas = canvas;
            }
        }
        for (int i=0; i<slotParent.transform.childCount; i++)
        {
         if(slotParent.transform.GetChild(i).transform.name.Contains("InventorySlot"))
            {
                slots.Add(slotParent.transform.GetChild(i).GetComponent<InventorySlot>());
                slotParent.transform.GetChild(i).GetComponent<InventorySlot>().canvas = canvas;
            }
        }
        mouseImage.gameObject.SetActive(false);
        mouseText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        mouseImage.transform.position = Input.mousePosition;

        if(inventoryAlphaValue!=-0.1f|| inventoryAlphaValue != 1.1f)
        {
            setAlpha = true;
        }
        else
        {
            setAlpha = false;
        }
    }

    public void setMouseImage(bool active)
    {
        switch (active)
        {
            case true:
                mouseImage.gameObject.SetActive(true);
                mouseText.gameObject.SetActive(true);
                return;
            case false:
                mouseImage.gameObject.SetActive(false);
                mouseText.gameObject.SetActive(false);
                return;

        }
        
    }
    public void updateMouseItem()
    {
        mouseImage.sprite = mouseItem.GetSprite();

        if (mouseItem.Stackable())
        {
            mouseText.text = mouseItem.amount.ToString();
        }
        else
        {
            mouseText.text = string.Empty;
        }
    }
    public void AddItem(Item item)
    {
        int amount = item.amount;
        

        //Check all slots for matching item
        foreach(InventorySlot slot in slots)
        {
            Item slotItem = slot.heldItem;
            if ((slotItem.itemType == item.itemType) && (slotItem.amount!=slotItem.MaxStack()))
            {
                //Slot has enough space to fit all of needed item
                if (slotItem.amount + amount <= slotItem.MaxStack())
                {
                    slotItem.amount += amount;
                    return;
                }
                //Otherwise the slot can fit some, but not all
                else
                {
                    amount -= slotItem.MaxStack() - slotItem.amount;
                    slotItem.amount = slotItem.MaxStack();
                }
            }
        }
        
        //If this point is reached, there is still some amount of the item to still put away, and we need to take up a new slot
        foreach(InventorySlot slot in slots)
        {
            Item slotItem = slot.heldItem;
            if (slotItem.itemType == Item.ItemType.Blank)
            {
                //Should never be false unless adding an amount to the inventory that is greater than the items max stack size
                if (amount <= item.MaxStack())
                {
                    Item newItem=new Item();
                    newItem.itemType = item.itemType;
                    newItem.amount = amount;
                    newItem.attributes = item.attributes;

                    slot.heldItem = newItem;
                  
                    return;
                }
                else
                {
                    Item newItem = new Item();
                    newItem.itemType = item.itemType;
                    newItem.amount = item.MaxStack();
                    newItem.attributes = item.attributes;

                     amount -= slotItem.MaxStack();
                }
            }
        }

        Item droppedItem = new Item();
        droppedItem.itemType = item.itemType;
        droppedItem.amount = amount;
        droppedItem.attributes = item.attributes;
        DropItem(droppedItem);
    }

    public void DropItem(Item item)
    {
        Vector3 spawnPos = player.transform.position + (player.transform.forward*2);
        spawnPos.y += .5f;
        GameObject droppedItem = Instantiate(item.getModel(), spawnPos, Quaternion.identity);
        droppedItem.GetComponent<DroppedItem>().item = item;

    }

    public void RemoveItem(Item item)
    {
        int amount = item.amount;
        foreach(InventorySlot slot in slots)
        {
            if(slot.heldItem.itemType == item.itemType)
            {
                if (slot.heldItem.amount > amount)
                {
                  
                    slot.heldItem.amount -= amount;
                    return;

                }
                else
                {
                    int slotAmount = slot.heldItem.amount;
                    slot.heldItem = blankItem;
                    amount -= slotAmount;
                }
            }
            else
            {
                continue;
            }
        }
    }

    public bool HasItem(Item item) {
        int amount = 0;
        foreach (InventorySlot slot in slots)
        {
            if (slot.heldItem.itemType == item.itemType)
            {
                amount += slot.heldItem.amount;
            }


        }
        if (amount >= item.amount)
        {
            return true;
        }
        else
        {
            return false;
        }


        }

    


}
