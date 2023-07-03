using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class ToolFilter : NetworkBehaviour
{

    public ToolMesh[] tools;
    public Camera playerCamera;

    public int currentDamage = 0;

    //Set the tool/weapon we want to be active
    public void filterTools(Item.ItemType type)
    {
        for (int i = 0; i < tools.Length; i++)
        {
            if (tools[i].itemType == type)
            {
                tools[i].gameObject.SetActive(true);

            }
            else
            {
                tools[i].gameObject.SetActive(false);
            }
        }
    }
    /// <summary>
    /// Uses a raycast to detect a collisions and runs functions based upon it
    /// </summary>
    public void collisionExecution()
    {
        if (!IsOwner) return;
        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 3.5f))
        {
            GameObject hitObj = hit.transform.gameObject;
            if (hitObj.TryGetComponent(out WorldObject worldObj))
            {
                WorldObjectCollision(worldObj);
            }
            else if (hitObj.transform.parent != null)
            {
                if (hitObj.transform.parent.TryGetComponent(out WorldObject worldObjInparent))
                {
                    WorldObjectCollision(worldObjInparent);
                }
            }
            if (hitObj.TryGetComponent(out PlayerHandler handler))
            {
                float baseDamage = PlayerHandler.instance.currentItem.getAttributeValue(ItemAttribute.AttributeName.Damage);
                handler.DamagePlayerServerRPC(handler.GetComponent<Unity.Netcode.NetworkObject>(), baseDamage);
            }
            //Try to spawn particles
            if (hit.transform.TryGetComponent(out MeshFilter hitMeshFilter))
            {
                ObjectParticleCollision(hitMeshFilter, hit);
            }
            else if (hit.transform.parent != null)
            {
                if (hit.transform.parent.TryGetComponent(out MeshFilter hitParentMeshFilter))
                {
                    ObjectParticleCollision(hitParentMeshFilter, hit);
                }
            }
            return;




        }


    }
    /// <summary>
    /// Sets the camera of the tool filter
    /// </summary>
    /// <param name="cameraToSet">The camera reference to set as</param>
    public void SetCamera(Camera cameraToSet)
    {
        playerCamera = cameraToSet;
    }
    /// <summary>
    /// Method that calls when a worldObject is detected
    /// </summary>
    /// <param name="worldObj">The WorldObject reference</param>
    private void WorldObjectCollision(WorldObject worldObj)
    {
        worldObj.objectCollision();
    }
    /// <summary>
    /// Method that calls when an object is hit
    /// </summary>
    /// <param name="hitMeshFilter">The MeshFilter reference</param>
    /// <param name="hit">The RaycastHit reference</param>
    private void ObjectParticleCollision(MeshFilter hitMeshFilter, RaycastHit hit)
    {
        if (MaterialDictionary.instance.RequestParticleSystem(hitMeshFilter, hit.triangleIndex, out GameObject system))
        {
            Quaternion rotQuat = new Quaternion(Quaternion.FromToRotation(transform.up, hit.normal).x, Quaternion.FromToRotation(transform.up, hit.normal).y, Quaternion.FromToRotation(transform.up, hit.normal).z, Quaternion.FromToRotation(transform.up, hit.normal).w);
            Transform systemTransform = Instantiate(system, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal)).transform;
            systemTransform.transform.localPosition += systemTransform.forward * 0.1f;
        }
    }
}
