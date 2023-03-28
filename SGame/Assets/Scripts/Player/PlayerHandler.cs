using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHandler : MonoBehaviour
{
    //Declare a singleton for this script
    public static PlayerHandler instance { get; private set; }
    
    [Header ("Inventory Logic")]

    //The inventory component
    public Inventory inventory;

    //Component to manage building
    public BuildingManager buildingManager;

    //Current slot we have equipped
    [HideInInspector] public InventorySlot currentSlot;

    //Current Item we have equipped
    public Item currentItem;

    //Gameobject with the animator component
    public GameObject toolAnchor;

    //Component to control the current tool equipped
    [SerializeField] private ToolFilter toolFilter;
    
    //Animator for the player
    public Animator anim;

    //Action blockers
    [HideInInspector] public List<string> mouseBlockers = new List<string>();
    [HideInInspector] public List<string> KeyBlockers = new List<string>();
    public bool ableToMouseLook = true;
    public bool ableToKey = true;



    void Awake()
    {
        anim = toolAnchor.GetComponent<Animator>();
        instance = this;
        StartCoroutine(CheckLockConditions());
    }

    // Update is called once per frame
    void Update()
    {
        //If the current item we have equipped isn't the one in our active slot, then equip the new item, and play a pullout animation if needed
        if (currentItem != currentSlot.heldItem)
        {
            OnEquip(currentSlot.heldItem);
            currentItem = currentSlot.heldItem;
            CrosshairManager.instance.ChangeCrosshairOnItem(currentItem);
        }
        if (Input.GetMouseButtonDown(0)&&ableToMouseLook)
        {
           
            UseItem(currentSlot.heldItem);
        }
        
        

    }

    public void OnEquip(Item item)
    {
        toolFilter.filterTools(item.itemType);
        bool build = item.hasAttribute(ItemAttribute.AttributeName.EnablesBuilding);
        buildingManager.buildingEnabled = build;
        if (build)
        {
            buildingManager.selectPrefab(item);
        }
        
        switch (item.itemType)
        {
            default:
                return;
            case Item.ItemType.StoneHatchet:
            case Item.ItemType.ShellAxe:
               
                    anim.Play("Pullout");
                

                return;
        }
      
    }
    public void SetActiveSlot(InventorySlot slot)
    {
        currentSlot = slot;
    }
    public void UseItem(Item item)
    {
        switch (item.itemType)
        {
            default:
                return;
            case Item.ItemType.StoneHatchet:
            case Item.ItemType.ShellAxe:
                if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Idle"))
                {
                    anim.Play("Swing");
                }

                return;
        }
    }
    public IEnumerator CheckLockConditions()
    {
        if (mouseBlockers.Count != 0)
        {
            ableToMouseLook = false;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            ableToMouseLook = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (KeyBlockers.Count != 0)
        {
            ableToKey = false;
        }
        else
        {
            ableToKey = true;
        }

        yield return new WaitForSecondsRealtime(0.05f);
        StartCoroutine(CheckLockConditions());
    }
}
