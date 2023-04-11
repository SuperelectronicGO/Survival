using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;
public class ItemSpawnStart : NetworkBehaviour
{
    private Item item;
    private bool recievedItemType = false;
    public override void OnNetworkSpawn()
    {
        //On spawn, wait until the item is set, then create the values needed for this item, and destroy this script
       // StartCoroutine(RunCreateDroppedItemOnValueSet());
       
    }
    public void RecieveItemType(Item recievedItem)
    {
        item = recievedItem;
        //recievedItemType = true;
        CreateDroppedItem();
        Destroy(this);
        
    }
    private IEnumerator RunCreateDroppedItemOnValueSet()
    {
        yield return new WaitUntil(() => recievedItemType = true);
        CreateDroppedItem();
        Destroy(this);
        yield break;
    }
    public void CreateDroppedItem()
    {
        //Find this items constructor
        ItemSpawnScriptable itemConstructor = null;
        switch (item.itemType)
        {
            case Item.ItemType.OakLog:
                itemConstructor = ItemAssets.Instance.oakLogSpawnScriptable;
                break;
            default:
                itemConstructor = ItemAssets.Instance.bagScriptable;
                break;

        }
        if (itemConstructor == null) { throw new NullReferenceException(); }
        //Create the GameObject
        GameObject g = this.gameObject;
        //Set transform values
        g.transform.localScale = itemConstructor.spawnedScale;
        g.transform.localEulerAngles = itemConstructor.spawnedRotation;
        //g.transform.position = positionToSpawn;
        //Add mesh filter and renderer, and feed mesh and materials
        MeshFilter meshFilter = g.AddComponent<MeshFilter>();
        meshFilter.mesh = itemConstructor.mesh;
        MeshRenderer meshRenderer = g.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterials = itemConstructor.materials;
        //Add rigidbody and set materials
        Rigidbody r = g.AddComponent<Rigidbody>();
        r.useGravity = true;
        r.isKinematic = false;
        //Add collider
        switch (itemConstructor.colliderType)
        {
            case ItemSpawnScriptable.ColliderType.box:
                BoxCollider boxCol = g.AddComponent<BoxCollider>();
                boxCol.center = itemConstructor.colliderOffset;
                boxCol.size = itemConstructor.colliderScale;
                break;
            case ItemSpawnScriptable.ColliderType.capsule:
                CapsuleCollider capsuleCol = g.AddComponent<CapsuleCollider>();
                capsuleCol.center = itemConstructor.colliderOffset;
                capsuleCol.radius = itemConstructor.colliderRadius;
                capsuleCol.height = itemConstructor.colliderHeight;
                capsuleCol.direction = itemConstructor.colliderDirection;
                break;
            case ItemSpawnScriptable.ColliderType.sphere:
                SphereCollider sphereCol = g.AddComponent<SphereCollider>();
                sphereCol.center = itemConstructor.colliderOffset;
                sphereCol.radius = itemConstructor.colliderRadius;
                break;
            case ItemSpawnScriptable.ColliderType.cylinder:
                Debug.LogError("Collider Type not implemented yet");
                break;
        }
        //Set object name
        this.gameObject.name = "Dropped " + item.itemName() + " x" + item.amount;

    }
}
