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
    public string name;
    [NonReorderable]
    public List<ingredient> Elements = new List<ingredient>();
    public int amount;

}
[System.Serializable]
public class ingredient{
    public string name;
    public Item.ItemType Item;
    [NonReorderable]
    public CraftingChangeModifier[] modifiers;
}
