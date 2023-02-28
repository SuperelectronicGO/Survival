using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHandler : MonoBehaviour
{
    //Declare a singleton for this script
    public static PlayerHandler instance;
    
    [Header ("Inventory Logic")]

    //The inventory component
    public Inventory inventory;

    //Component to manage building
    public BuildingManager buildingManager;

    //Current slot we have equipped
    [HideInInspector] public InventorySlot currentSlot;

    //Gameobject with the animator component
    public GameObject toolAnchor;

    //Component to control the current tool equipped
    [SerializeField] private ToolFilter toolFilter;
    
    //Animator for the player
    private Animator anim;

    //Current Item we have equipped
    public Item currentItem;

    

  
  
    void Awake()
    {
        anim = toolAnchor.GetComponent<Animator>();
        instance = this;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentItem != currentSlot.heldItem)
        {
            OnEquip(currentSlot.heldItem);
            currentItem = currentSlot.heldItem;
        }
        if (Input.GetMouseButtonDown(0))
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
    public void calculateCurrentItemStats()
    {

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
   
}
