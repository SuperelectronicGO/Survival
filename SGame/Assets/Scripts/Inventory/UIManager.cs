using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
   
    [Header("Components")]
    [SerializeField] private GameObject inventoryObject;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject map;

    public bool inventoryOpen = false;
    public bool settingsOpen = false;
    public bool mapOpen = false;



    private Inventory inventory;
    private CraftingManager cManager;
    private RuneManager runeManager;
    private Animator inventoryAnimator;

    private string inventoryBlocker = "Inventory Open";
    // Start is called before the first frame update
    void Start()
    {
        inventory = inventoryObject.GetComponent<Inventory>();
        inventoryAnimator = inventoryObject.GetComponent<Animator>();
        cManager = inventoryObject.GetComponent<CraftingManager>();
        runeManager = GetComponent<RuneManager>();
        if (inventory.mouseItem.itemType != Item.ItemType.Blank)
        {
            inventory.AddItem(inventory.mouseItem);
            inventory.mouseItem = inventory.blankItem;
            inventory.mouseImage.sprite = inventory.blankItem.GetSprite();
            inventory.mouseText.text = string.Empty;
            inventory.setMouseImage(false);
        }
        inventoryObject.SetActive(false);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)&&!cManager.isTyping)
        {
            
           
            switch (inventoryOpen)
            {
                case false:
                    cManager.currentRecipeType = CraftingScriptable.recipeType.All;
                    cManager.displayValidRecipes(true);
                    inventory.refreshSlotValues(inventory.slots);
                    runeManager.refreshRuneSlotValues();
                    cManager.refreshCraftingSlotValues();
                    inventoryObject.SetActive(true);
                    inventoryOpen = true;
                    PlayerHandler.instance.mouseBlockers.Add(inventoryBlocker);
                    //Add a blocker to the player controller
                    return;
                case true:
                    if (inventory.mouseItem.itemType != Item.ItemType.Blank)
                    {
                        inventory.AddItem(inventory.mouseItem);
                        inventory.mouseItem = inventory.blankItem;
                        inventory.mouseImage.sprite = inventory.blankItem.GetSprite();
                        inventory.mouseText.text = string.Empty;
                        inventory.setMouseImage(false);
                        

                    }
                    cManager.returnCraftingItems();
                    inventoryObject.SetActive(false);
                    inventoryOpen = false;
                    PlayerHandler.instance.mouseBlockers.Remove(inventoryBlocker);
                    return;
            }

        }
       



        if (settingsOpen)
        {
            if (!settings.activeInHierarchy)
            {
                settings.SetActive(true);
            }
        }
        else
        {
            if (settings.activeInHierarchy)
            {
                settings.SetActive(false);
            }
        }

        if (mapOpen)
        {
            if (!map.activeInHierarchy)
            {
                map.SetActive(true);
            }
        }
        else{
            if (map.activeInHierarchy)
            {
                map.SetActive(false);
            }
        }

       
    }
}
