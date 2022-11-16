using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolFilter : MonoBehaviour
{
   public ToolMesh[] tools;
    public void filterTools(Item.ItemType type)
    {
        for(int i=0; i<tools.Length; i++)
        {
            if(tools[i].itemType == type)
            {
                tools[i].gameObject.SetActive(true);
            }
            else
            {
                tools[i].gameObject.SetActive(false);
            }
        }
    }

}
