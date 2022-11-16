using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InventoryAlphaImageSetter : MonoBehaviour
{
    private Image image;
    [SerializeField] private Inventory inventory;
    private Color col;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        col = image.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (inventory.setAlpha)
        {
            Color newCol = col;
            newCol.a *= inventory.inventoryAlphaValue;
            image.color = newCol;

           
        }
    }
}
