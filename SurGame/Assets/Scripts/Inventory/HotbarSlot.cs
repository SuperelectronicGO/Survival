using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotbarSlot : MonoBehaviour
{
    public Color backgroundColor;

    public bool selected;
    public int slotNumber;
    public GameObject background;

    public Hotbarmanager hotbarmanager;
    public ItemActivation activate;


  
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        backgroundColor.r = background.GetComponent<Image>().color.r;
        backgroundColor.b = background.GetComponent<Image>().color.b;
        backgroundColor.g = background.GetComponent<Image>().color.g;
        if (selected)
        {
            hotbarmanager.acslot = this.GetComponent<inventorySlot>();
            backgroundColor.a = 0.6f;
            if (Input.GetMouseButtonDown(0) && GetComponent<inventorySlot>().heldItem.isUseable())
            {
               
                activate.UseItem(GetComponent<inventorySlot>().heldItem);
            }

            foreach(GameObject obj in hotbarmanager.activeItems)
            {
                if (obj.name == GetComponent<inventorySlot>().heldItem.itemName())
                {
                    obj.SetActive(true);
                }
                else
                {
                    obj.SetActive(false);
                }
            }
           
        }
        else
        {
            backgroundColor.a = 0.4f;
        }
        background.GetComponent<Image>().color = backgroundColor;


    }
}
