using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CraftingManager : MonoBehaviour
{
    [Header("Components")]
    //Reference to canvas
    [SerializeField] private Canvas canvas;
    //Reference to inventory
    [SerializeField] private Inventory inventory;
    //The image used to display the selected recipe's icon
    [SerializeField] private Image displayImage;
    //The text for the result of the recipe
    [SerializeField] private TextMeshProUGUI resultDisplayText;
    //The search bar for the crafting recipe field
    [SerializeField] private InputField searchBar;
    //The array of slots for crafting
    [SerializeField] private List<ISInterface> slots = new List<ISInterface>();
    [Header("Anchors")]
    //Gameobject to use as the start pos for spawning the different recipes
    [SerializeField] private GameObject recipeIconParent;
    //Gameobject to use as the start pos for spawning the different recipe ingredients
    [SerializeField] private Transform ingredientParent;
    [Header("Templates")]
    //The blank recipe prefab
    [SerializeField] private GameObject recipeTemplate;
    //The blank ingredient prefab
    [SerializeField] private GameObject ingredientTemplate;
    [Header("Recipe Data")]
    //List of all recipes
    [SerializeField] private List<CraftingScriptable> recipes = new List<CraftingScriptable>();
    //List of recipes that are beign displayed
    private List<CraftingScriptable> validRecipes = new List<CraftingScriptable>();
    //Current selected recipe
    public CraftingScriptable currentRecipe;
    //Current ingredients needed for the recipe
    public List<Item> currentIngredients = new List<Item>();
    //Items we are using for the current recipe
    public List<Item> usingItems = new List<Item>();
    //Item that is going to be used as output
    public Item outputItem;
    [Header("Data")]
    //Current type of the recipe
    public CraftingScriptable.recipeType currentRecipeType = CraftingScriptable.recipeType.All;
    //List of the current items in the crafting slots
    private List<Item> craftingSlotItems = new List<Item>();
    //Bool if the player is able to craft or not
    private bool ableToCraft = false;
    public List<CraftingChangeModifier> currentModifiers;
    //If the player is using the search bar or not
    public bool isTyping;


    //Blocker
    private string searchbarBlocker = "Crafting search bar";
    // Start is called before the first frame update
    void Start()
    {
        validRecipes = recipes;
        currentModifiers = new List<CraftingChangeModifier>();
    }


    // Update is called once per frame
    void Update()
    {
        
        if (searchBar.isFocused)
        {
            if (PlayerHandler.instance.KeyBlockers.Contains(searchbarBlocker))
            {
                //Nothing
            }
            else
            {
                PlayerHandler.instance.KeyBlockers.Add(searchbarBlocker);
            }
        }
        else
        {
            if (PlayerHandler.instance.KeyBlockers.Contains(searchbarBlocker))
            {
                PlayerHandler.instance.KeyBlockers.Remove(searchbarBlocker);
            }
            else
            {
                //Nothing
            }
        }
       
    }


    //Method to display recipes on a side pannel
    public void displayValidRecipes(bool isButton)
    {
        for (int i = 0; i < recipeIconParent.transform.childCount; i++)
        {
            Destroy(recipeIconParent.transform.GetChild(i).gameObject);
        }
        int x = 0;
        int y = 0;
        foreach (CraftingScriptable s in validRecipes)
        {
            if (isButton)
            {
                if (s.recipetype == currentRecipeType || currentRecipeType == CraftingScriptable.recipeType.All)
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

    //Method that displays the current recipe
    public void displayCurrentRecipe()
    {
        //Set the display image
        displayImage.sprite = currentRecipe.result.GetSprite();
        //Set the result text
        setResultDescription(false);
        displayRecipeIngredients();

    }

    //Method that calculates the string for the result description - existing is false if a new recipe is selected, or true if modifying current recipe
    private void setResultDescription(bool existing)
    {
        //Create string to hold data
        string resultString = "";
        //Set initial to name and description
        resultString += "<size=20>" + currentRecipe.result.itemName() + "</size>\n<size=10>" + currentRecipe.itemDescription + "</size>";
        //Add each attribute the item has to the description
        foreach (ItemAttribute Attribute in currentRecipe.result.attributes)
        {
            switch (Attribute.attribute)
            {
                case ItemAttribute.AttributeName.Durability:
                    float maxDurValue = currentRecipe.result.getAttributeValue(ItemAttribute.AttributeName.MaxDurability);
                    float durValue = Attribute.value;
                    //Have to create values to store durability values so we can apply in the correct order - both are already int values
                    float linearDurValue = 0;
                    float percentDurValue = 1;
                    for(int i=0; i<currentModifiers.Count; i++)
                    {
                        if (currentModifiers[i].type == CraftingChangeModifier.changeType.durabilityLinear)
                        {
                            linearDurValue += currentModifiers[i].value;
                        }else if(currentModifiers[i].type == CraftingChangeModifier.changeType.durabilityPercent)
                        {
                            //Make a value like 1.08 for when we multiply
                            percentDurValue += (currentModifiers[i].value / 100);
                        }
                    }
                    
                    //Apply
                    maxDurValue *= percentDurValue;
                    durValue *= percentDurValue;
                    maxDurValue += linearDurValue;
                    durValue += linearDurValue;
                    //Round
                    maxDurValue = Mathf.RoundToInt(maxDurValue);
                    durValue = Mathf.RoundToInt(durValue);
                    
                    resultString += "\n<size=8>Durability: " + durValue + "/" + maxDurValue + "</size>";
                    break;
                case ItemAttribute.AttributeName.Damage:
                    resultString += "\n<size=8>Damage: " + Attribute.value + "</size>";
                    break;
            }

        }
        //Add each modifier the ingredients will contribute
        if(currentModifiers.Count != 0){
            for(int i=0; i<currentModifiers.Count; i++)
            {
                if (currentModifiers[i].value != 0)
                {
                    switch (currentModifiers[i].type)
                    {
                        case CraftingChangeModifier.changeType.durabilityLinear:
                            if (currentModifiers[i].value > 0)
                            {
                                resultString += "\n<color=#1aeb13ff>+ " + currentModifiers[i].value + " Durability</color>";

                            }
                            else
                            {
                                resultString += "\n<color=#ff0000ff>- " + currentModifiers[i].value + " Durability</color>";
                            }
                            break;
                        case CraftingChangeModifier.changeType.durabilityPercent:
                            if (currentModifiers[i].value > 0)
                            {
                                resultString += "\n<color=#1aeb13ff>+ " + currentModifiers[i].value + "% Durability</color>";
                            }
                            else
                            {
                                resultString += "\n<color=#ff0000ff>- " + currentModifiers[i].value + "% Durability</color>";
                            }
                            break;
                    }
                }
            }
        }
        if (existing)
        {
           
            
           

        }
        resultDisplayText.text = resultString;
    }

    //Method that displays the ingredients needed for a recipe
    private void displayRecipeIngredients()
    {
        for (int i = 0; i < ingredientParent.childCount; i++)
        {
            Destroy(ingredientParent.GetChild(i).gameObject);
        }
        for (int x = 0; x < currentRecipe.ingredients.Count; x++)
        {
            Vector3 spawnPos = ingredientParent.position;
            spawnPos.x += (x * 27) * canvas.scaleFactor;
            GameObject ingredient = Instantiate(ingredientTemplate, spawnPos, Quaternion.identity, ingredientParent);
            //Store first child because we have more than one reference
            Transform childImage = ingredient.transform.GetChild(0);
            //Get the text component and set it
            ingredient.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "x" + currentRecipe.ingredients[x].amount.ToString();
            //Set the sprite for the first Icon
            Item blankItem = new Item();
            blankItem.itemType = currentRecipe.ingredients[x].Elements[0].Item;
            childImage.gameObject.GetComponent<Image>().sprite = blankItem.GetSprite();
            childImage.gameObject.name = currentRecipe.ingredients[x].Elements[0].name + " Sprite";
            //Create duplicates of the icon if more than one option
            if (currentRecipe.ingredients[x].Elements.Count > 1)
            {
                for (int i = 1; i < currentRecipe.ingredients[x].Elements.Count; i++)
                {
                    //Create a new image to hold the sprite 
                    Image iconImage = Instantiate(childImage, ingredient.transform.position + new Vector3(Random.Range(-2, 2) * canvas.scaleFactor, (9 * i) * canvas.scaleFactor, 0), Quaternion.identity, ingredient.transform).GetComponent<Image>();
                    //Create a blank item, and pass it the itemType we have so we can get the sprite of the item
                    blankItem = new Item();
                    blankItem.itemType = currentRecipe.ingredients[x].Elements[i].Item;
                    iconImage.sprite = blankItem.GetSprite();
                    iconImage.gameObject.name = currentRecipe.ingredients[x].Elements[i].name + " Sprite";
                }
            }
        }
    }

    //Method that returns all the items in the crafting slots when the inventory is closed or a new recipe is selected
    public void returnCraftingItems()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].heldItem.itemType != Item.ItemType.Blank)
            {
                inventory.AddItem(slots[i].heldItem);
                slots[i].heldItem = inventory.blankItem;
                slots[i].updateSlotValues();
            }
        }
        analyzeItemList();
    }

    //Method to refresh the UI's of the crafting slots
    public void refreshCraftingSlotValues()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].updateSlotValues();
        }
    }

    //Method that calculates if crafting is possible as well as what modifiers to use based on the list
    public void analyzeItemList()
    {
        if (currentRecipe == null)
        {
            return;
        }
        //Define an array of each modifier
        currentModifiers.Clear();
        //Define a new array of the needed items that we can modify
        float[] neededItems = new float[currentRecipe.ingredients.Count];
        //Clear the list of the items we are to be using
        usingItems.Clear();
        //Set the values needed
        for (int i = 0; i < currentRecipe.ingredients.Count; i++)
        {
            neededItems[i] = currentRecipe.ingredients[i].amount;
            
        }
        //Check the item in each slot against that element
        for (int slotIndex = 0; slotIndex < slots.Count; slotIndex++)
        {
            for (int i = 0; i < currentRecipe.ingredients.Count; i++)
            {
                //Nested loop through each element that can be used for that ingredient
                for (int j = 0; j < currentRecipe.ingredients[i].Elements.Count; j++)
                {
                    if (slots[slotIndex].heldItem.itemType == currentRecipe.ingredients[i].Elements[j].Item)
                    {
                        //An item in this slot matches what we need
                        if (neededItems[i] > 0)
                        {
                            Item contributedItem = new Item();
                            contributedItem.itemType = slots[slotIndex].heldItem.itemType;
                            contributedItem.amount = slots[slotIndex].heldItem.amount;
                            //Don't subtract more than we need
                            if (contributedItem.amount > neededItems[i])
                            {
                                contributedItem.amount = Mathf.RoundToInt(neededItems[i]);
                            }
                            usingItems.Add(contributedItem);
                            //If it matches, add it to the modifier array
                            for (int elementModifierIndex = 0; elementModifierIndex < currentRecipe.ingredients[i].Elements[j].modifiers.Length; elementModifierIndex++)
                            {

                                bool modifierExists = false;
                                float modifierChangeAmount = 0;
                                for (int modifierIndex = 0; modifierIndex < currentModifiers.Count; modifierIndex++)
                                {
                                    if (currentModifiers[modifierIndex].type == currentRecipe.ingredients[i].Elements[j].modifiers[elementModifierIndex].type)
                                    {
                                        CraftingChangeModifier modifierToAdd = new CraftingChangeModifier();
                                        modifierToAdd.value = currentRecipe.ingredients[i].Elements[j].modifiers[elementModifierIndex].value;
                                        modifierToAdd.type = currentRecipe.ingredients[i].Elements[j].modifiers[elementModifierIndex].type;
                                        modifierChangeAmount = modifierToAdd.value;
                                        if (slots[slotIndex].heldItem.amount >= neededItems[i])
                                        {
                                            modifierChangeAmount *= (neededItems[i] / currentRecipe.ingredients[i].amount);
                                        }
                                        else
                                        {
                                            modifierChangeAmount *= (neededItems[i] / currentRecipe.ingredients[i].amount) * (slots[slotIndex].heldItem.amount / neededItems[i]);
                                        }
                                        currentModifiers[modifierIndex].value += modifierChangeAmount;
                                        modifierExists = true;
                                    }

                                }
                                if (!modifierExists)
                                {
                                    CraftingChangeModifier modifierToAdd = new CraftingChangeModifier();
                                    modifierToAdd.value = currentRecipe.ingredients[i].Elements[j].modifiers[elementModifierIndex].value;
                                    modifierToAdd.type = currentRecipe.ingredients[i].Elements[j].modifiers[elementModifierIndex].type;

                                    if (slots[slotIndex].heldItem.amount >= neededItems[i])
                                    {
                                        modifierToAdd.value *= (neededItems[i] / currentRecipe.ingredients[i].amount);
                                    }
                                    else
                                    {
                                        modifierToAdd.value *= (neededItems[i] / currentRecipe.ingredients[i].amount) * (slots[slotIndex].heldItem.amount / neededItems[i]);
                                    }

                                    currentModifiers.Add(modifierToAdd);
                                }
                            }
                        }
                        neededItems[i] -= slots[slotIndex].heldItem.amount;


                    }
                }
            }
        }
        //Round modifier values
        for (int i=0; i<currentModifiers.Count; i++)
        {
           currentModifiers[i].value = Mathf.RoundToInt(currentModifiers[i].value);
        }
        //Reset result description
        setResultDescription(true);
        //Check if able to craft
        ableToCraft = true;
        for(int i=0; i<neededItems.Length; i++)
        {
            if (neededItems[i] <= 0)
            {
                //Still fine 
            }
            else
            {
                ableToCraft = false;
            }
        }
        //Set output item with modifiers
        if (ableToCraft)
        {
            outputItem = currentRecipe.result.Clone();

            // outputItem.itemType = currentRecipe.result.itemType;
            //outputItem.amount = currentRecipe.result.amount;
            //outputItem.attributes = currentRecipe.result.attributes;
            float totalDurabilityPercentModifier = 1;
            float totalDurabilityLinearModifier = 0;

            for(int i=0; i<currentModifiers.Count; i++)
            {
                switch (currentModifiers[i].type)
                {
                    case CraftingChangeModifier.changeType.durabilityPercent:
                        totalDurabilityPercentModifier += (currentModifiers[i].value/100);
                        break;
                    case CraftingChangeModifier.changeType.durabilityLinear:
                        totalDurabilityLinearModifier += currentModifiers[i].value;
                        break;
                }
            }
            //Set attributes
            if (outputItem.hasAttribute(ItemAttribute.AttributeName.Durability))
            {
                //Set multiplication first, then add
                outputItem.setAttributeValue(ItemAttribute.AttributeName.Durability, Mathf.RoundToInt(totalDurabilityPercentModifier*outputItem.getAttributeValue(ItemAttribute.AttributeName.Durability)), "=");
                outputItem.setAttributeValue(ItemAttribute.AttributeName.MaxDurability, Mathf.RoundToInt(totalDurabilityPercentModifier * outputItem.getAttributeValue(ItemAttribute.AttributeName.MaxDurability)), "=");
                outputItem.setAttributeValue(ItemAttribute.AttributeName.Durability, Mathf.RoundToInt(totalDurabilityLinearModifier+ outputItem.getAttributeValue(ItemAttribute.AttributeName.Durability)), "=");
                outputItem.setAttributeValue(ItemAttribute.AttributeName.MaxDurability, Mathf.RoundToInt(totalDurabilityLinearModifier+ outputItem.getAttributeValue(ItemAttribute.AttributeName.MaxDurability)), "=");
                //Round to be normal
                
            }
        }
    }

    //Method that crafts the item
    public void craftItem()
    {
        if (ableToCraft)
        {
            for(int i=0; i<usingItems.Count; i++)
            {
                inventory.RemoveItem(usingItems[i], slots);
            }
            inventory.AddItem(outputItem);
            analyzeItemList();
            inventory.refreshSlotValues(slots);
        }
        
    }
   
}
[System.Serializable] 
public class CraftingChangeModifier
{
    public enum changeType
    {
        durabilityLinear,
        durabilityPercent

    }
    public changeType type;
    public float value;
    
}