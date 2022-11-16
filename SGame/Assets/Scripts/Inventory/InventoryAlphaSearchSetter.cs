using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryAlphaSearchSetter : MonoBehaviour
{
    private Text text;
    [SerializeField] private Inventory inventory;
    private Color col;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        col = text.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (inventory.setAlpha)
        {
            Color newCol = col;
            newCol.a *= inventory.inventoryAlphaValue;
            text.color = newCol;


        }
    }
}
