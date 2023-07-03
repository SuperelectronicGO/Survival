using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }
    [Header("Components")]
    [SerializeField] private GameObject inventoryObject;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject map;
    [SerializeField] private GameObject recipePanel;
    [SerializeField] private GameObject craftingDisplay;
    [SerializeField] private GameObject chestDisplay;
    public bool inventoryOpen = false;
    public bool settingsOpen = false;
    public bool mapOpen = false;



    private Inventory inventory;
    private CraftingManager cManager;
    private RuneManager runeManager;
    private Animator inventoryAnimator;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
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
        if (Input.GetKeyDown(KeyCode.Tab) && !cManager.isTyping)
        {


            switch (inventoryOpen)
            {
                case false:
                    OpenInventory(true, false);
                    return;
                case true:
                    CloseInventory();
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
        else
        {
            if (map.activeInHierarchy)
            {
                map.SetActive(false);
            }
        }


    }
    /// <summary>
    /// Opens the inventory
    /// </summary>
    /// <param name="showCrafting">If the crafting windows should open</param>
    /// <param name="showChest">If the chest window should open</param>
    /// <param name="chestSlots">The number of slots to show for the test window (Default = 18)</param>
    /// <param name="storageReference">The reference to NetworkStorage we want to use</param>
    public void OpenInventory(bool showCrafting, bool showChest, int chestSlots = 18, NetworkStorage storageReference = null)
    {
       /* If showing the crafting menu,
        * Filter the current recipes using the "all" filter
        * Display the recipes that are available
        * Refresh the values of the crafting slots
        * Set the recipe and display panels to be active. */
        if (showCrafting)
        {
            cManager.currentRecipeType = CraftingScriptable.recipeType.All;
            cManager.displayValidRecipes(true);
            cManager.refreshCraftingSlotValues();
            recipePanel.SetActive(true);
            craftingDisplay.SetActive(true);
        }
        //If not showing crafting, set the recipe and display panel inactive
        else
        {
            recipePanel.SetActive(false);
            craftingDisplay.SetActive(false);
        }
        /* If showing a chest,
         * Set the chest display to be active in the inventory
         * Set the chest slots in the inventory active depending on the chest storage
         * Set the values of the items within those slots. */
        if (showChest)
        {
            chestDisplay.SetActive(true);
            StorageManager.instance.SetActiveSlots(chestSlots);
            StorageManager.instance.SetSlotValues();
        }
        //If not showing a chest, hide the chest menu
        else
        {
            chestDisplay.SetActive(false);
        }
        /* Refresh the values of the inventory slots
         * Refresh the values of the rune slots
         * Set the inventory active and the inventoryOpen booolean to true
         * Add the inventory being open to the list of mouse blockers. */
        inventory.refreshSlotValues(inventory.slots);
        runeManager.refreshRuneSlotValues();
        inventoryObject.SetActive(true);
        inventoryOpen = true;
        PlayerHandler.instance.mouseBlockers.Add(this.gameObject);
    }
    public void CloseInventory()
    {
        if (inventory.mouseItem.itemType != Item.ItemType.Blank)
        {
            inventory.AddItem(inventory.mouseItem);
            inventory.mouseItem = inventory.blankItem;
            inventory.mouseImage.sprite = inventory.blankItem.GetSprite();
            inventory.mouseText.text = string.Empty;
            inventory.setMouseImage(false);


        }
        cManager.returnCraftingItems();
        PlayerHandler.instance.mouseBlockers.Remove(this.gameObject);
        inventoryOpen = false;
        inventoryObject.SetActive(false);
    }
}
