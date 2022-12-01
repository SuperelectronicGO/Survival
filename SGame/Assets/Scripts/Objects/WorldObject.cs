using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObject : MonoBehaviour
{
    public float health;
    public GameObject hitParticle;
    public string breakableBy = "Axe";


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


        //Remove object
        Destroy(this.gameObject);
    }
}
