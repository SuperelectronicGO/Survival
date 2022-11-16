using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
public class DifferentRecipeButton : MonoBehaviour
{
    public string description;
    public Item heldItem;
    public Image icon;
    public int orderInIngredientList = 0;
    public CraftingManager manager;
    private Inventory inventory;
    // Start is called before the first frame update

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
    }
    // Update is called once per frame
    void Update()
    {
        //icon.sprite = heldItem.GetSprite();
        Color a = Color.white;
        if (inventory.HasItem(heldItem))
        {
            a.a = 1;
        }
        else
        {
            a.a = .3f;

        }
        icon.color = a;

        GetComponent<Button_UI>().MouseOverFunc = () =>
        {
            manager.alternateResultDescription.text = description;

        };
        GetComponent<Button_UI>().MouseOutOnceFunc = () =>
        {
            manager.alternateResultDescription.text = string.Empty;

        };
    }
    public void ReplaceItem()
    {
        if (inventory.HasItem(heldItem))
        {
            manager.currentIngredients[orderInIngredientList] = heldItem;
            manager.updateCurrentRecipe(false, true);
        }
    }
}
