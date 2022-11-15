using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineableObject : MonoBehaviour
{
    public GameObject hitEffect;
    public float Health;
    private GameObject assets;
    private Hotbarmanager manager;
    [SerializeField] private GameObject objToDestroy;
    public droppedItem[] dropItems;
    [SerializeField] private bool randomizeSize;
    [SerializeField] private float randomSizeMin;
    [SerializeField] private float randomSizeMax;

    [SerializeField] private float dropRange = 2;
    [SerializeField] private float[] dropHeights = { 0, 1 };
    public enum materialMineType
    {
        Pickaxe,
        Axe,
    }
    public float weaknessAmnt;
    public materialMineType[] breakableBy;
    // Start is called before the first frame update
    void Start()
    {
        if (randomizeSize)
        {
            float thisScale = Random.Range(randomSizeMin, randomSizeMax);
            objToDestroy.transform.localScale = new Vector3(thisScale, thisScale, thisScale);
        }
        assets = GameObject.Find("GameManager");
        manager = assets.GetComponent<ItemActivation>().hotbarManager;
       
    }


    // Update is called once per frame
    void Update()
    {
      
        if (Health <= 0)
        {
            foreach (droppedItem item in dropItems)
            {
                for (int i = 0; i < Random.Range(item.minAmnt, item.maxAmnt); i++)
                {
                    droppedItem copyItem = item;
                    copyItem.itemPrefab.GetComponent<PickupableObject>().heldItem.amount = Random.Range(item.objMinAmnt, item.objMaxAmnt);
                    Instantiate(copyItem.itemPrefab, new Vector3(this.transform.position.x + Random.Range(-dropRange, dropRange), this.transform.position.y + Random.Range(dropHeights[0], dropHeights[1]), this.transform.position.z + Random.Range(-dropRange, dropRange)), Quaternion.identity);
                }
            }
           
            Destroy(objToDestroy);
        }
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Miner"))
        {
           
                Instantiate(hitEffect, col.transform.position, Quaternion.identity);
            float damage;

            damage= manager.acslot.GetComponent<inventorySlot>().heldItem.stats.damage;
            foreach (powers pow in manager.acslot.GetComponent<inventorySlot>().heldItem.stats.weaponPowers)
            {
               
                foreach (materialMineType type in breakableBy)
                {
                    
                    if (type.ToString() == pow.name)
                    {
                        damage += (manager.acslot.GetComponent<inventorySlot>().heldItem.stats.damage * weaknessAmnt * pow.power) / 10;
                       
                    }
                  
                }
               
            }

            Health -= damage;
            }
        
         

              
            }
    
        
    
    
    
}
