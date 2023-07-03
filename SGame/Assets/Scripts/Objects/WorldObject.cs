using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class WorldObject : NetworkBehaviour
{
    [NonReorderable]
    public dropItem[] droppedItems;
    public NetworkVariable<float> health = new NetworkVariable<float>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public string breakableBy = "Axe";

    [Header("Effects")]
    public GameObject hitParticle;
    [SerializeField]
    private GameObject deathParticle;
    [SerializeField] private Vector3 effectOffset;
    private void Start()
    {
        health.OnValueChanged += (float oldValue, float newValue) =>
        {
            if (newValue < 0)
            {
                objectDeath();
            }
        };

    }
    public void objectCollision()
    {
        float baseDamage = PlayerHandler.instance.currentItem.getAttributeValue(ItemAttribute.AttributeName.Damage);
        float damageMult = PlayerHandler.instance.currentItem.getAttributeValue(ItemAttribute.AttributeName.Type);
        DamageObject(40);

    }

    private void objectDeath()
    {
        Instantiate(deathParticle, this.transform.position + effectOffset, Quaternion.identity);
        if (IsServer)
        {
            /*
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
            */
            //Remove object

            GetComponent<NetworkObject>().Despawn();
        }
    }
    /// <summary>
    /// Method that handles object damage dealing
    /// </summary>
    /// <param name="amount">The amount of damage dealt to the object</param>
    public void DamageObject(float amount)
    {
        if (IsServer)
        {
            health.Value -= amount;

        }
        else
        {
            DamageObjectServerRpc(amount);
        }
    }
    /// <summary>
    /// Server RPC invoked by clients to damage an object
    /// </summary>
    /// <param name="amount">The amount of damage dealt to the object</param>
    [ServerRpc(RequireOwnership = false)]
    public void DamageObjectServerRpc(float amount)
    {
        health.Value -= amount;
    }
}
