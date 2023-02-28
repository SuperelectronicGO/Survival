using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
public class CraftingRecipeIcon : MonoBehaviour
{
    private CraftingManager cManager;
    public CraftingScriptable recipe;
    public Image displayImage;
    private Button_UI button_UI;
    // Start is called before the first frame update
    void Start()
    {
        cManager = FindObjectOfType<CraftingManager>();
        button_UI = GetComponent<Button_UI>();
    }

    // Update is called once per frame
    void Update()
    {
        button_UI.ClickFunc = () =>
        {
            cManager.currentRecipe = recipe;
            cManager.returnCraftingItems();
            cManager.displayCurrentRecipe();
        };
    }
}
