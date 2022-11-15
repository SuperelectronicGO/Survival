using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ToggleCrafting : MonoBehaviour
{
    private string page = "Inventory";
    [SerializeField] private Image thisImage;
    [SerializeField] private Sprite craftingSprite;
    [SerializeField] private Sprite exitSprite;
    [SerializeField] private GameObject craftingMenu;
    [SerializeField] private GameObject inventoryMenu;

    public void swapScreens()
    {
        if (page == "Inventory")
        {
            page = "Crafting";
        }
        else if(page=="Crafting")
        {
            page = "Inventory";
        }

        switch (page)
        {
            case "Inventory":
                inventoryMenu.SetActive(true);
                craftingMenu.SetActive(false);
                thisImage.sprite = craftingSprite;
                

                return;
            case "Crafting":
                inventoryMenu.SetActive(false);
                craftingMenu.SetActive(true);
                thisImage.sprite = exitSprite;
                return;
        }

    }
}
