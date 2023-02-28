using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CodeMonkey.Utils;
public class InventorySlot : MonoBehaviour
{
    public Item heldItem;
    [SerializeField] private bool isCraftingSlot;
    [SerializeField] private bool isHotbarSlot;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Image itemImage;
    [SerializeField] private Image durabilityImage;
    [SerializeField] private Transform durabilityBar;
    public TextMeshProUGUI amountText;

    private Tooltip tooltip;
    private RectTransform rectTransform;
    [HideInInspector]
    public Canvas canvas;

    [HideInInspector]
    public bool selected = false;

    [HideInInspector]
    public Image backgroundImage;

    private HotbarManager hotbarManager;
    private Color defaultColor;

    private Button_UI button_UI;
    // Start is called before the first frame update
    void Awake()
    {
        tooltip = inventory.tooltipObject.GetComponent<Tooltip>();
        rectTransform = GetComponent<RectTransform>();
        backgroundImage = transform.GetChild(0).GetComponent<Image>();
        hotbarManager = FindObjectOfType<HotbarManager>();
        defaultColor = backgroundImage.color;
        button_UI = GetComponent<Button_UI>();
        canvas = inventory.canvas;
    }

    
  
    // Update is called once per frame
    void Update()
    {
        //Manage UI elements




        button_UI.MouseOverOnceFunc = () =>
        {
            if (heldItem.itemType != Item.ItemType.Blank)
            {
                Tooltip.instance.updateTooltip(true, inventory.generateTooltipForItem(heldItem), rectTransform, isHotbarSlot);
            }
            
        };

        button_UI.MouseOutOnceFunc = () =>
        {
            Tooltip.instance.updateTooltip(false, "", rectTransform, isHotbarSlot);
        };

        button_UI.ClickFunc = () =>
        {
            if (inventory.mouseItem.itemType == Item.ItemType.Blank)
            {
                //Inventory mouse empty :(
                if (heldItem.itemType == Item.ItemType.Blank)
                {
                    //empty slot,
                    inventory.setMouseImage(false);
                  
                }
                else
                {
                    inventory.mouseItem = heldItem;
                    heldItem = inventory.blankItem;
                    inventory.updateMouseItem();
                    inventory.setMouseImage(true);
                    
                }

            }
            else
            {
                //Inventory mouse not empty :)
                if (heldItem.itemType == Item.ItemType.Blank)
                {
                    //Give us inventory item
                    heldItem = inventory.mouseItem;
                    inventory.mouseItem = inventory.blankItem;
                    inventory.updateMouseItem();
                    inventory.setMouseImage(false);
                   
                }
                else
                {
                    if (inventory.mouseItem.itemType == heldItem.itemType)
                    {
                        //Slots same item
                        if (inventory.mouseItem.amount + heldItem.amount <= heldItem.MaxStack())
                        {
                            heldItem.amount += inventory.mouseItem.amount;
                            inventory.mouseItem = inventory.blankItem;
                            inventory.updateMouseItem();
                            inventory.setMouseImage(false);
                            
                        }
                        else
                        {
                            if (heldItem.amount != heldItem.MaxStack())
                            {
                                int amountToAdd = heldItem.MaxStack() - heldItem.amount;
                                heldItem.amount += amountToAdd;
                                inventory.mouseItem.amount -= amountToAdd;
                                inventory.updateMouseItem();
                                inventory.setMouseImage(true);
                                
                            }
                            else
                            {
                                int amountToAdd = inventory.mouseItem.MaxStack() - inventory.mouseItem.amount;
                                inventory.mouseItem.amount += amountToAdd;
                               heldItem.amount -= amountToAdd;
                                inventory.updateMouseItem();
                                inventory.setMouseImage(true);
                               
                            }
                        }

                    }
                    else
                    {
                        //Slots not same item
                        Item inventoryItem = inventory.mouseItem;
                        inventory.mouseItem = heldItem;
                        heldItem = inventoryItem;
                        inventory.updateMouseItem();
                        inventory.setMouseImage(true);
                       
                    }
                }

            }
            updateSlotValues();
            if (heldItem.itemType != Item.ItemType.Blank)
            {
                Tooltip.instance.updateTooltip(true, inventory.generateTooltipForItem(heldItem), rectTransform, isHotbarSlot);
            }
            else
            {
                Tooltip.instance.updateTooltip(false, "", rectTransform, isHotbarSlot);
            }
            if (isCraftingSlot)
            {
                inventory.cManager.analyzeItemList();
            }
        };

        button_UI.MouseRightClickFunc = () =>
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                inventory.DropItem(heldItem);
                heldItem = inventory.blankItem;
              
            }
            if (heldItem.itemType == Item.ItemType.Blank)
            {
                //Empty slot
             
                
            }
            else
            {
                if (inventory.mouseItem.itemType == Item.ItemType.Blank)
                {
                    Item dupItem = new Item();
                    dupItem.itemType = heldItem.itemType;
                    dupItem.amount = 1;
                    dupItem.attributes = heldItem.attributes;
                    inventory.mouseItem = dupItem;
                    heldItem.amount -= 1;
                    inventory.updateMouseItem();
                    inventory.setMouseImage(true);
                  
                }else if (inventory.mouseItem.itemType == heldItem.itemType && (inventory.mouseItem.amount < inventory.mouseItem.MaxStack()))
                {
                    inventory.mouseItem.amount += 1;
                    heldItem.amount -= 1;
                    inventory.updateMouseItem();
                    inventory.setMouseImage(true);
                   


                }
             
            }
            updateSlotValues();
            if (heldItem.itemType != Item.ItemType.Blank)
            {
                Tooltip.instance.updateTooltip(true, inventory.generateTooltipForItem(heldItem), rectTransform, isHotbarSlot);
            }
            else
            {
                Tooltip.instance.updateTooltip(false, "", rectTransform, isHotbarSlot);
            }
            if (isCraftingSlot)
            {
                inventory.cManager.analyzeItemList();
            }
        };

        

    }

    public void updateSlotValues()
    {
        if (heldItem.amount <= 0)
        {
            heldItem = inventory.blankItem;
        }

        if (heldItem.Stackable() && heldItem.amount >= 1)
        {
            amountText.text = heldItem.amount.ToString();
        }
        else
        {
            amountText.text = string.Empty;
        }

        if (selected)
        {
            backgroundImage.color = hotbarManager.selectedColor;
        }
        else
        {
            backgroundImage.color = defaultColor;
        }

        itemImage.sprite = heldItem.GetSprite();

        if (heldItem.hasAttribute(ItemAttribute.AttributeName.Durability))
        {
            durabilityBar.gameObject.SetActive(true);
            durabilityImage.fillAmount = heldItem.getAttributeValue(ItemAttribute.AttributeName.Durability) / heldItem.getAttributeValue(ItemAttribute.AttributeName.MaxDurability);
        }
        else
        {
            durabilityBar.gameObject.SetActive(false);
        }
        
    }

    

   
}
