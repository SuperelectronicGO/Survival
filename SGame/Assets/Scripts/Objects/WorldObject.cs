using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObject : MonoBehaviour
{
    [NonReorderable]
    public dropItem[] droppedItems;
    public float health;
   
    public string breakableBy = "Axe";

    [Header("Effects")]
    public GameObject hitParticle;
    [SerializeField]
    private GameObject deathParticle;
    [SerializeField] private Vector3 effectOffset;
    private PlayerHandler handler;
    void Start()
    {
       
        handler = PlayerHandler.instance;
       
    }
    
    public void objectCollision()
    {
        //Set values
        float baseDamage = handler.currentItem.getAttributeValue(ItemAttribute.AttributeName.Damage);
        float damageMult = handler.currentItem.getAttributeValue(ItemAttribute.AttributeName.Type);
        string hitItemType = handler.currentItem.getAttributeString(ItemAttribute.AttributeName.Type);
       
        //Subtract Health
        if (breakableBy == hitItemType)
        {
            health -= baseDamage * damageMult;
        }
        else
        {
            health -= baseDamage;
        }
        if (health <= 0)
        {
            objectDeath();
        }
        
    }

    private void objectDeath()
    {
        
        for(int i=0; i<droppedItems.Length; i++)
        {
            bool dropitem = true;
            Item item = droppedItems[i].item;
            //Set item amount to the range
            item.amount = Random.Range(Mathf.RoundToInt(droppedItems[i].amount.x), Mathf.RoundToInt(droppedItems[i].amount.y));
            if (droppedItems[i].chance != 1)
            {
                float chance = Random.Range(0, 1);
                if (chance <= droppedItems[i].chance)
                {
                    //Success
                }
                else
                {
                    dropitem = false;
                }
            }
            if (dropitem)
            {
                //Spawn Item
                GameObject itemPrefab = Instantiate(droppedItems[i].itemPrefab, transform.position + new Vector3(Random.Range(-1,1), 3f+ Random.Range(0, 0.5f), Random.Range(-1, 1)), Quaternion.identity);
                //Randomize rotation
                itemPrefab.transform.eulerAngles = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                //Set spawned item's item to what is in the list
                itemPrefab.GetComponent<DroppedItem>().item = item;
            }
        }

        //Remove object
        Instantiate(deathParticle, this.transform.position + effectOffset, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
