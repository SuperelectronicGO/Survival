using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;


public class inventorySlot : MonoBehaviour
{
    public InventoryManager invManager;
    public Item heldItem;

    public Image item;
    public Text amountText;

    [SerializeField] private Image durBarBackground;
    [SerializeField] private Image durBar;
    // Start is called before the first frame update
    void Start()
    {
        item.sprite = heldItem.GetSprite();
        amountText.text = heldItem.amount.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (heldItem.HasDur())
        {
            durBar.fillAmount = 0+ (heldItem.durability / heldItem.maxDurability);
            if (!durBar.isActiveAndEnabled)
            {
                durBar.gameObject.SetActive(true);
                durBarBackground.gameObject.SetActive(true);
            }
        }else if (durBar.isActiveAndEnabled)
        {
            durBar.gameObject.SetActive(false);
            durBarBackground.gameObject.SetActive(false);
        }




        item.sprite = heldItem.GetSprite();
        amountText.text = heldItem.amount.ToString();

       




        GetComponent<Button_UI>().ClickFunc = () =>
        {

            invManager.selectedObject = this.gameObject;
            invManager.triggerThis = true;

        };
        GetComponent<Button_UI>().MouseRightClickFunc = () =>
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (heldItem.itemType != Item.ItemType.Blank && invManager.selectedItem.itemType == Item.ItemType.Blank)
                {
                    if (heldItem.amount == 1)
                    {
                        invManager.selectedItem = heldItem;
                        heldItem = invManager.blankItem;
                    }
                    else if (heldItem.amount > 1)
                    {
                        Item dupItem = new Item { itemType = heldItem.itemType, durability = heldItem.durability, maxDurability = heldItem.maxDurability, amount = 1 };
                        heldItem.amount -= 1;
                        invManager.selectedItem = dupItem;


                    }
                }
                else if (heldItem.itemType != Item.ItemType.Blank && invManager.selectedItem.itemType == heldItem.itemType)
                {
                    if (heldItem.amount > 1)
                    {
                        heldItem.amount -= 1;
                        invManager.selectedItem.amount += 1;
                    }
                    else
                    {
                        invManager.selectedItem.amount += 1;
                        heldItem = invManager.blankItem;
                    }
                }

            }
            else
            {
                if (invManager.selectedItem.itemType == Item.ItemType.Blank && heldItem != invManager.blankItem)
                {
                    invManager.selectedItem = heldItem;
                    heldItem = invManager.blankItem;
                }
                else if (invManager.selectedItem.itemType != Item.ItemType.Blank && heldItem.itemType == Item.ItemType.Blank)
                {
                    heldItem = invManager.selectedItem;
                    invManager.selectedItem = invManager.blankItem;

                }
                else if (invManager.selectedItem.itemType != heldItem.itemType)
                {
                    Item item1;
                    Item item2;
                    item1 = heldItem;
                    item2 = invManager.selectedItem;

                    invManager.selectedItem = item1;
                    heldItem = item2;
                }
                else if (invManager.selectedItem.itemType == heldItem.itemType)
                {
                    if (heldItem.amount + invManager.selectedItem.amount <= heldItem.MaxStack())
                    {
                        heldItem.amount += invManager.selectedItem.amount;
                        invManager.selectedItem = invManager.blankItem;

                    }
                    else
                    {
                        invManager.selectedItem.amount -= (heldItem.MaxStack() - heldItem.amount);
                        heldItem.amount = heldItem.MaxStack();
                    }
                }
            }






        };
    }

   
}
