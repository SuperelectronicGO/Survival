using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CraftingManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject recipeIconParent;
    [SerializeField] private GameObject recipeTemplate;
    [SerializeField] private Image displayImage;
    [SerializeField] private InputField searchBar;
    [SerializeField] private Image[] ingredientDisplayImages;
    [SerializeField] private Image[] ingredientDisplayImageBackgrounds;
    [SerializeField] private Button craftButton;
    public TextMeshProUGUI[] amountTexts;
    [SerializeField] private Sprite blankImage;
    [SerializeField] private TextMeshProUGUI resultDescription;
    public TextMeshProUGUI alternateResultDescription;
    [SerializeField] private Transform alternateIngredientAnchor;
    [SerializeField] private GameObject alternateIngredientButton;
    [Header("Recipe Data")]
    [SerializeField] private List<CraftingScriptable> recipes = new List<CraftingScriptable>();
    private List<CraftingScriptable> validRecipes = new List<CraftingScriptable>();
    public CraftingScriptable currentRecipe;
    public List<Item> currentIngredients = new List<Item>();
    [Header("Data")]
    public CraftingScriptable.recipeType currentRecipeType = CraftingScriptable.recipeType.All;
    public bool isTyping;
    
    // Start is called before the first frame update
    void Start()
    {
        validRecipes = recipes;
    }

    public void showRecipe()
    {
        craftButton.gameObject.SetActive(true);
        displayImage.gameObject.SetActive(true);
        resultDescription.gameObject.SetActive(true);
        alternateResultDescription.gameObject.SetActive(true);
        for (int i = 0; i < ingredientDisplayImages.Length; i++)
        {
            ingredientDisplayImages[i].gameObject.SetActive(true);
            ingredientDisplayImageBackgrounds[i].gameObject.SetActive(true);
            amountTexts[i].gameObject.SetActive(true);
            
        }


    }

    public void hideRecipe()
    {
        craftButton.gameObject.SetActive(false);
        resultDescription.text = string.Empty;
        alternateResultDescription.text = string.Empty;
        displayImage.gameObject.SetActive(false);
        resultDescription.gameObject.SetActive(false);
        alternateResultDescription.gameObject.SetActive(false);
        for(int i=0; i<ingredientDisplayImages.Length; i++)
        {
            ingredientDisplayImages[i].gameObject.SetActive(false);
            ingredientDisplayImageBackgrounds[i].gameObject.SetActive(false);
            amountTexts[i].gameObject.SetActive(false);
            amountTexts[i].text = string.Empty;

        }
        for (int i = 0; i < alternateIngredientAnchor.childCount; i++)
        {
            Destroy(alternateIngredientAnchor.GetChild(i).gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        isTyping = searchBar.isFocused;
    }
  
    public void updateCurrentRecipe(bool updateAll, bool ShowRecipe)
    {
        
        if (currentRecipe == null)
        {
            return;
        }
        if (updateAll)
        {
            displayImage.sprite = currentRecipe.result.GetSprite();
            resultDescription.text = currentRecipe.itemDescription;
            currentIngredients.Clear();
            int x = 0;
            int y = 0;
            int j = 0;
            for(int i=0; i<alternateIngredientAnchor.childCount; i++)
            {
                Destroy(alternateIngredientAnchor.GetChild(i).gameObject);
            }
            foreach (RecipeComponent r in currentRecipe.ingredients)
            {
                currentIngredients.Add(r.Ingredients[0].item);
                if (r.Ingredients.Count > 1)
                {
                 
                      
                    for (int i = 0; i < r.Ingredients.Count; i++) {
                        Vector3 spawnpos = alternateIngredientAnchor.position;
                        spawnpos.x += (22.5f * x * canvas.scaleFactor);
                        spawnpos.y += (22.5f * y * canvas.scaleFactor);
                        y += 1;
                        if (y >= 2)
                        {
                            y = 0;
                            x += 1;
                        }

                        GameObject g = Instantiate(alternateIngredientButton, spawnpos, Quaternion.identity, alternateIngredientAnchor);

                        DifferentRecipeButton d = g.GetComponent<DifferentRecipeButton>();
                        d.description = r.Ingredients[i].changeDescription;
                        d.manager = this;
                        d.heldItem = r.Ingredients[i].item;
                        d.orderInIngredientList = j;
                        d.icon.sprite = r.Ingredients[i].item.GetSprite();
                            }
                            
                }
                j += 1;
            }
            if(ShowRecipe)
            showRecipe();
        }



        for(int i= 0; i< ingredientDisplayImages.Length; i++)
        {
            if (i<currentIngredients.Count)
            {
              
                ingredientDisplayImages[i].sprite = currentIngredients[i].GetSprite();
                Color displayColor = Color.white;
                amountTexts[i].text = currentIngredients[i].amount.ToString();
                if (inventory.HasItem(currentIngredients[i]))
                {
                    displayColor.a = 1;


                }
                else
                {
                    displayColor.a = 0.35f;
                }
                ingredientDisplayImages[i].color = displayColor;


            }
            else
            {
                ingredientDisplayImages[i].sprite = blankImage;
                amountTexts[i].text = string.Empty;
            }
        }
        bool displayCraft = true;
        foreach(Item i in currentIngredients)
        {
            if (inventory.HasItem(i))
            {
                continue;
            }
            else
            {
                displayCraft = false;
            }
        }
        if (displayCraft)
        {
            craftButton.interactable = true;
        }
        else
        {
            craftButton.interactable = false;
        }



    }
    
    public void displayValidRecipes(bool isButton)
    {
        for(int i =0; i<recipeIconParent.transform.childCount; i++)
        {
            Destroy(recipeIconParent.transform.GetChild(i).gameObject);
        }
        int x = 0;
        int y = 0;
        foreach (CraftingScriptable s in validRecipes)
        {
            if (isButton)
            {
                if(s.recipetype == currentRecipeType||currentRecipeType==CraftingScriptable.recipeType.All)
                {
                    //Do nothing and continue
                }
                else
                {
                    //Skip this one
                    continue;
                }


            }
            else
            {
                if (s.result.itemName().ToLower().Contains(searchBar.text.ToLower()))
                {
                    //Do nothing and continue
                }
                else
                {
                    //Skip this one
                    continue;
                }
            }
            Vector3 spawnPos = recipeIconParent.transform.position;
            spawnPos.x += x * 40 * canvas.scaleFactor;
            spawnPos.y += y * 40 * canvas.scaleFactor;
            GameObject g = Instantiate(recipeTemplate, spawnPos, Quaternion.identity, recipeIconParent.transform);
            CraftingRecipeIcon r = g.GetComponent<CraftingRecipeIcon>();
            r.displayImage.sprite = s.result.GetSprite();
            r.recipe = s;

            x += 1;
            if (x > 4)
            {
                x = 0;
                y += 1;
            }

        }
        
    }

    public void craftRecipe()
    {
        int i = 0;
        bool hasItems = true;
        foreach(Item item in currentIngredients)
        {
            if (inventory.HasItem(currentIngredients[i]))
            {
                i += 1;
               
            }
            else
            {
                i += 1;
                hasItems = false;
              
            }
            
        }
        if (hasItems)
        {
           
            for(int j=0; j<currentIngredients.Count; j++) 
            {
                inventory.RemoveItem(currentIngredients[j]);
            }
                inventory.AddItem(currentRecipe.result);
            updateCurrentRecipe(false, true);
        }

       
    }

   
}
