using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingButton : MonoBehaviour
{
    [SerializeField] private CraftingManager manager;
    [SerializeField] private CraftingScriptable.recipeType recipetype;
   public void filterRecipes()
    {
        manager.currentRecipeType = recipetype;
        manager.displayValidRecipes(true);

    }
}
