using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CodeMonkey.Utils;
public class InventorySlot : MonoBehaviour
{
    public Item heldItem;

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
    // Start is called before the first frame update
    void Start()
    {
        tooltip = inventory.tooltipObject.GetComponent<Tooltip>();
        rectTransform = GetComponent<RectTransform>();
        backgroundImage = transform.GetChild(0).GetComponent<Image>();
        hotbarManager = FindObjectOfType<HotbarManager>();
        defaultColor = backgroundImage.color;
    }

    // Update is called once per frame
    void Update()
    {
        //Manage UI elements
        if (selected)
        {
            backgroundImage.color = hotbarManager.selectedColor;
        }
        else
        {
            backgroundImage.color = defaultColor;
        }
        itemImage.sprite = heldItem.GetSprite();
        if (heldItem.Stackable()&&heldItem.amount>=1)
        {
            amountText.text = heldItem.amount.ToString();
        }
        else
        {
            amountText.text = string.Empty;
        }
        if (heldItem.hasAttribute(ItemAttribute.AttributeName.Durability))
        {
            durabilityBar.gameObject.SetActive(true);
            durabilityImage.fillAmount = heldItem.getAttributeValue(ItemAttribute.AttributeName.Durability) / heldItem.getAttributeValue(ItemAttribute.AttributeName.MaxDurability);
        }
        else
        {
            durabilityBar.gameObject.SetActive(false);
        }



        GetComponent<Button_UI>().MouseOverFunc = () =>
        {
            if (heldItem.itemType != Item.ItemType.Blank)
            {
                //  inventory.tooltipObject.SetActive(true);
                tooltip.tooltipText.text = heldItem.itemName() + "\n<i>" + heldItem.itemTooltip() + "</i>\n";


                //(rectTransform.rect.height * rectTransform.localScale.y / 2) -
                tooltip.transform.position = new Vector3(rectTransform.position.x + (rectTransform.rect.width * rectTransform.localScale.x / 2) + 2, rectTransform.position.y - ((tooltip.tooltipText.GetComponent<RectTransform>().rect.height / 2) * canvas.scaleFactor), 0);
                Vector2 tooltipSizeModiified = new Vector2(tooltip.tooltipRect.rect.width + 2, tooltip.tooltipRect.rect.height + 2);
                tooltip.background.sizeDelta = tooltipSizeModiified;
                tooltip.background.transform.position = new Vector3(tooltip.tooltipText.transform.position.x - 1, tooltip.tooltipText.transform.position.y - 1, 0);
                tooltip.tooltipText.gameObject.SetActive(true);
                tooltip.background.gameObject.SetActive(true);
            }
            
        };

        GetComponent<Button_UI>().MouseOutOnceFunc = () =>
        {
           
            // inventory.tooltipObject.SetActive(false);
            tooltip.background.gameObject.SetActive(false);
            tooltip.tooltipText.gameObject.SetActive(false);
            tooltip.tooltipText.text = string.Empty;
        };

        GetComponent<Button_UI>().ClickFunc = () =>
        {
            if (inventory.mouseItem.itemType == Item.ItemType.Blank)
            {
                //Inventory mouse empty :(
                if (heldItem.itemType == Item.ItemType.Blank)
                {
                    //empty slot, return
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

            inventory.cManager.updateCurrentRecipe(true,false);
        };

        GetComponent<Button_UI>().MouseRightClickFunc = () =>
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





            inventory.cManager.updateCurrentRecipe(true,false);
        };

        if (heldItem.amount <= 0)
        {
            heldItem = inventory.blankItem;
        }

    }


   
}
