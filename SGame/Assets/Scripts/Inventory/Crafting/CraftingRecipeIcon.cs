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
    // Start is called before the first frame update
    void Start()
    {
        cManager = FindObjectOfType<CraftingManager>();   
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Button_UI>().ClickFunc = () =>
        {
            cManager.currentRecipe = recipe;
            cManager.updateCurrentRecipe(true,true);

        };
    }
}
