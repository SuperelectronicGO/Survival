using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName ="Crafting Recipe",menuName ="Scriptables/Crafting Recipe",order =2)]
public class CraftingScriptable : ScriptableObject
{
    public enum recipeType
    {
        All,
        Tool,
        Building
    }
    public recipeType recipetype;
    public Item result;
    [NonReorderable]
    [SerializeField]
    
    public List<RecipeComponent> ingredients = new List<RecipeComponent>();
    public string itemDescription;

   

    
}
[System.Serializable]
public class RecipeComponent
{
    [NonReorderable]
    public List<ingredient> Ingredients = new List<ingredient>();

}
[System.Serializable]
public class ingredient{
    public Item item;
    public string changeDescription;
}
