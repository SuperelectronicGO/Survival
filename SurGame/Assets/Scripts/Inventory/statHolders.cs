using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class statHolders : MonoBehaviour
{
    public Sprite damage;
    public Sprite axePower;
    public Sprite pickaxePower;
    public Sprite blank;

   
    public void displayStats(string[] stats, GameObject selectedObj)
    {
        eraseStats();
        float number = 1;
        foreach (string stat in stats)
        {
            switch (stat)
            {
                case "Damage":
                    this.transform.Find("Icon" + number.ToString()).GetComponent<Image>().sprite = damage;
                    this.transform.Find("T" + number.ToString()).GetComponent<Text>().text = selectedObj.GetComponent<inventorySlot>().heldItem.stats.damage.ToString();

                    number += 1;
                    break;
                case "AxePower":
                    this.transform.Find("Icon" + number.ToString()).GetComponent<Image>().sprite = axePower;
                    this.transform.Find("T" + number.ToString()).GetComponent<Text>().text = selectedObj.GetComponent<inventorySlot>().heldItem.stats.weaponPowers[0].power.ToString();
                    number += 1;
                  
                    break;
                case "PickaxePower":
                    this.transform.Find("Icon" + number.ToString()).GetComponent<Image>().sprite = pickaxePower;
                    this.transform.Find("T" + number.ToString()).GetComponent<Text>().text = selectedObj.GetComponent<inventorySlot>().heldItem.stats.weaponPowers[0].power.ToString();
                    number += 1;
                    break;
            }
        }
    }
    public void eraseStats()
    {
      
        for(int i = 0; i < transform.childCount; i++) {
            if (transform.GetChild(i).gameObject.GetComponent<Image>() != null)
            {
                transform.GetChild(i).gameObject.GetComponent<Image>().sprite = blank;
            }
            if (transform.GetChild(i).gameObject.GetComponent<Text>() != null)
            {
                transform.GetChild(i).gameObject.GetComponent<Text>().text = "";
            }
        }
      
    }
}
