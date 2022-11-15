using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
public class CraftingManager : MonoBehaviour
{
    public GameObject recipeDisplayTemplate;
    [SerializeField] private GameObject spawnPosition;
    public string currentFilter = "All";
    public string searchingForName = "";
    public GameObject inputBar;
    public bool searchingForSpecific = false;
    public craftingRecipe[] recipes;

    void Update()
    {
        updateCraftingRecipes();
        if (searchingForSpecific)
        {
            searchingForName = inputBar.GetComponent<InputField>().text;
        }
        if (searchingForName != "")
        {
            searchingForSpecific = true;
        }

    }





















    private void setSearchModeCategory()
    {
        searchingForSpecific = false;
    }
   public void updateCraftingRecipes()
    {
        for(int i = 0; i<spawnPosition.transform.childCount; i++)
        {
            Destroy(spawnPosition.transform.GetChild(i).gameObject);
        }
        if (currentFilter=="All" && searchingForName == "")
        {
            int x = 0;
            int y = 0;
            foreach (craftingRecipe Recipe in recipes)
            {
                GameObject r = recipeDisplayTemplate;
                r.GetComponent<craftingRecipeIcon>().recipe = Recipe;
                r.GetComponentInChildren<Image>().sprite = Recipe.spriteItem.GetSprite();
                Instantiate(r, new Vector3(spawnPosition.transform.position.x+x*50, spawnPosition.transform.position.y+y*50, spawnPosition.transform.position.z), Quaternion.identity, spawnPosition.transform);
                x += 1;
                if (x > 4)
                {
                    y -= 1;
                    x = 0;
                }
            }
        }
        if (currentFilter != "All"&&searchingForName=="")
        {
            int x = 0;
            int y = 0;
            foreach (craftingRecipe Recipe in recipes)
            {
                if (Recipe.tag == currentFilter)
                {
                    GameObject r = recipeDisplayTemplate;
                    r.GetComponent<craftingRecipeIcon>().recipe = Recipe;
                    r.GetComponentInChildren<Image>().sprite = Recipe.spriteItem.GetSprite();
                    Instantiate(r, new Vector3(spawnPosition.transform.position.x + x * 50, spawnPosition.transform.position.y + y * 50, spawnPosition.transform.position.z), Quaternion.identity, spawnPosition.transform);
                    x += 1;
                    if (x > 4)
                    {
                        y -= 1;
                        x = 0;
                    }
                }
            }
        }
        if (searchingForSpecific)
        {
           
            int x = 0;
            int y = 0;
            foreach (craftingRecipe Recipe in recipes)
            {

                if (Recipe.name.Contains(searchingForName))
                {
                    GameObject r = recipeDisplayTemplate;
                    r.GetComponent<craftingRecipeIcon>().recipe = Recipe;
                    r.GetComponentInChildren<Image>().sprite = Recipe.spriteItem.GetSprite();
                    Instantiate(r, new Vector3(spawnPosition.transform.position.x + x * 50, spawnPosition.transform.position.y + y * 50, spawnPosition.transform.position.z), Quaternion.identity, spawnPosition.transform);
                    x += 1;
                    if (x > 4)
                    {
                        y -= 1;
                        x = 0;
                    }
                }
            }
        
        }
    }
}

[System.Serializable]
public class craftingRecipe
{
    public Item spriteItem;
    public Item[] outputs;
    public Item[] inputs;
    public string tag;
    public string name;
    


}
