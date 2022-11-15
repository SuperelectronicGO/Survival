using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotbarmanager : MonoBehaviour
{
    public int activeSlot = 1;
    public GameObject[] slots;
    public GameObject[] activeItems;
    public float actionState = 0;
    public inventorySlot acslot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject obj in activeItems)
        {
            obj.GetComponent<Animator>().SetFloat("ActionState", actionState);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            activeSlot = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            activeSlot = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            activeSlot = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            activeSlot = 4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            activeSlot = 5;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            activeSlot = 6;
        }
        if (Input.GetMouseButton(0))
        {
            actionState = 5;
        }
        else
        {
            actionState = 0;
        }
        foreach (GameObject slot in slots)
        {
            if (slot.GetComponent<HotbarSlot>().slotNumber == activeSlot)
            {
                slot.GetComponent<HotbarSlot>().selected = true;
            }
            else
            {
                slot.GetComponent<HotbarSlot>().selected = false;
            }
        }
    }
}
