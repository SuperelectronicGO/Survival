using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public bool triggerThis = false;




    public Image mouseImage;
    public Text mouseText;
    public GameObject mouseStuff;

   

    public GameObject selectedObject = null;
    [SerializeField] private GameObject tooltip;
    [SerializeField] private GameObject itemName;
    [SerializeField] private GameObject regularSlots;
    [SerializeField] private Image itemIcon;

    private bool itemSelected = true;
    public Item selectedItem;
    public Item blankItem;

    private bool inventoryOpen = false;

    public GameObject[] slots;

    [SerializeField] private GameObject statHolder;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // mouse
        {
            mouseText.text = selectedItem.amount.ToString();
            mouseImage.sprite = selectedItem.GetSprite();
            mouseStuff.transform.position = Input.mousePosition;
        }

        //Selected
        {
            if (selectedObject != null)
            {
                if (selectedObject.GetComponent<inventorySlot>().heldItem.itemType != Item.ItemType.Blank)
                {
                    tooltip.GetComponent<Text>().text = selectedObject.GetComponent<inventorySlot>().heldItem.itemTooltip();
                    itemName.GetComponent<Text>().text = selectedObject.GetComponent<inventorySlot>().heldItem.itemName();
                    itemIcon.sprite = selectedObject.GetComponent<inventorySlot>().heldItem.GetSprite();
                    if (triggerThis)
                    {
                        statHolder.GetComponent<statHolders>().displayStats(selectedObject.GetComponent<inventorySlot>().heldItem.stats.statPer(),selectedObject);
                        triggerThis = false;
                    }
                }
                else
                {
                    tooltip.GetComponent<Text>().text = "";
                    itemName.GetComponent<Text>().text = "";
                    itemIcon.sprite = selectedObject.GetComponent<inventorySlot>().heldItem.GetSprite();
                    statHolder.GetComponent<statHolders>().eraseStats();
                }


            }


        }



        //Open and close
        { 
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (inventoryOpen)
            {
                inventoryOpen = false;

            }
            else
            {
                inventoryOpen = true;
            }
        }
        if (inventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            if (!regularSlots.activeInHierarchy)
            {
                regularSlots.SetActive(true);
                mouseStuff.SetActive(true);
            }
        } else
        {
            Cursor.lockState = CursorLockMode.Locked;
            if (regularSlots.activeInHierarchy)
            {
                regularSlots.SetActive(false);
                mouseStuff.SetActive(false);
            }
        }
    }
    }
  
}
