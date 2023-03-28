using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarManager : MonoBehaviour
{
    public InventorySlot[] hotbarSlots;
    public PlayerHandler player;
    public Color selectedColor;
    private int currentSlot = 1;
    // Start is called before the first frame update
    void Start()
    {
        hotbarSlots[0].selected = true;
        player.currentSlot = hotbarSlots[0];
    }

    // Update is called once per frame
    void Update()
    {
        //Ensure no equip during spell
        if (!player.currentItem.hasAttribute(ItemAttribute.AttributeName.AllowsSpell))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                currentSlot = 1;
                hotbarSlots[0].selected = true;
                hotbarSlots[1].selected = false;
                hotbarSlots[2].selected = false;
                hotbarSlots[3].selected = false;
                hotbarSlots[4].selected = false;
                hotbarSlots[5].selected = false;
                UpdateAllSlotValues();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                currentSlot = 2;
                hotbarSlots[0].selected = false;
                hotbarSlots[1].selected = true;
                hotbarSlots[2].selected = false;
                hotbarSlots[3].selected = false;
                hotbarSlots[4].selected = false;
                hotbarSlots[5].selected = false;
                UpdateAllSlotValues();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                currentSlot = 3;
                hotbarSlots[0].selected = false;
                hotbarSlots[1].selected = false;
                hotbarSlots[2].selected = true;
                hotbarSlots[3].selected = false;
                hotbarSlots[4].selected = false;
                hotbarSlots[5].selected = false;
                UpdateAllSlotValues();
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                currentSlot = 4;
                hotbarSlots[0].selected = false;
                hotbarSlots[1].selected = false;
                hotbarSlots[2].selected = false;
                hotbarSlots[3].selected = true;
                hotbarSlots[4].selected = false;
                hotbarSlots[5].selected = false;
                UpdateAllSlotValues();
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                currentSlot = 5;
                hotbarSlots[0].selected = false;
                hotbarSlots[1].selected = false;
                hotbarSlots[2].selected = false;
                hotbarSlots[3].selected = false;
                hotbarSlots[4].selected = true;
                hotbarSlots[5].selected = false;
                UpdateAllSlotValues();
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                currentSlot = 6;
                hotbarSlots[0].selected = false;
                hotbarSlots[1].selected = false;
                hotbarSlots[2].selected = false;
                hotbarSlots[3].selected = false;
                hotbarSlots[4].selected = false;
                hotbarSlots[5].selected = true;
                UpdateAllSlotValues();
            }
        }
    }
    public void ReSelectSlot()
    {
        hotbarSlots[currentSlot - 1].selected = true;
        player.currentSlot = hotbarSlots[currentSlot - 1];
    }
    public void DeselectAllSlots()
    {
        for(int i=0; i<hotbarSlots.Length; i++)
        {
            hotbarSlots[i].selected = false;
            hotbarSlots[i].updateSlotValues();
        }
    }
    public void UpdateAllSlotValues()
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            hotbarSlots[i].updateSlotValues();
        }
        player.currentSlot = hotbarSlots[currentSlot - 1];
    }
}
