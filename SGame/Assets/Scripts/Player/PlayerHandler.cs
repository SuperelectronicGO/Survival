using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHandler : MonoBehaviour
{

    [Header ("Inventory Logic")]
    public Inventory inventory;
    public BuildingManager buildingManager;
    [HideInInspector] public InventorySlot currentSlot;
    public GameObject toolAnchor;
    [SerializeField] private ToolFilter toolFilter;
    private Animator anim;
    private Item currentItem;
    [Header("Unused")]
    [SerializeField] private GameObject enviromentUIEffects;
    [SerializeField] private Text envDisplayText;
    

  
    // Start is called before the first frame update
    void Start()
    {
        anim = toolAnchor.GetComponent<Animator>();
        
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
