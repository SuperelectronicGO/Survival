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
    private Animator inventoryAnimator;
    
    // Start is called before the first frame update
    void Start()
    {
        inventory = inventoryObject.GetComponent<Inventory>();
        inventoryAnimator = inventoryObject.GetComponent<Animator>();
        cManager = inventoryObject.GetComponent<CraftingManager>();
        inventoryAnimator.Play("Inventory_Close");
        if (inventory.mouseItem.itemType != Item.ItemType.Blank)
        {
            inventory.AddItem(inventory.mouseItem);
            inventory.mouseItem = inventory.blankItem;
            inventory.mouseImage.sprite = inventory.blankItem.GetSprite();
            inventory.mouseText.text = string.Empty;
            inventory.setMouseImage(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)&&!cManager.isTyping)
        {
            
            inventoryOpen = !inventoryOpen;
            switch (inventoryOpen)
            {
                case true:

                    //open anim
                    inventoryAnimator.Play("Inventory_Open");
                    cManager.hideRecipe();
                    cManager.currentRecipeType = CraftingScriptable.recipeType.All;
                    cManager.displayValidRecipes(true);
                    return;
                case false:
                   
                    //close anim
                    inventoryAnimator.Play("Inventory_Close");
                    if (inventory.mouseItem.itemType != Item.ItemType.Blank)
                    {
                        inventory.AddItem(inventory.mouseItem);
                        inventory.mouseItem = inventory.blankItem;
                        inventory.mouseImage.sprite = inventory.blankItem.GetSprite();
                        inventory.mouseText.text = string.Empty;
                        inventory.setMouseImage(false);
                    }
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

        if (!inventoryOpen && !settingsOpen && !mapOpen)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
