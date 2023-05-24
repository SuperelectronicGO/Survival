using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class ToolFilter : NetworkBehaviour
{
   
    public ToolMesh[] tools;
    public Camera camera;

    public int currentDamage = 0;
 
     //Set the tool/weapon we want to be active
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
    //Method to use raycasts to detect a collision
    public void collisionExecution()
    {
        if (!IsOwner) return;
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 3.5f))
        {
            GameObject hitObj = hit.transform.gameObject;
            if(hitObj.TryGetComponent(out WorldObject worldObj))
            {
                //Success!

                //Get the rotation at the hit point to spawn particles
                Quaternion rotQuat = new Quaternion(Quaternion.FromToRotation(transform.up, hit.normal).x, Quaternion.FromToRotation(transform.up, hit.normal).y, Quaternion.FromToRotation(transform.up, hit.normal).z, Quaternion.FromToRotation(transform.up, hit.normal).w);
                Instantiate(worldObj.hitParticle, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                worldObj.objectCollision();
                return;
            }
            if (hitObj.TryGetComponent(out PlayerHandler handler))
            {
                //Success!

                //Get the rotation at the hit point to spawn particles
                Quaternion rotQuat = new Quaternion(Quaternion.FromToRotation(transform.up, hit.normal).x, Quaternion.FromToRotation(transform.up, hit.normal).y, Quaternion.FromToRotation(transform.up, hit.normal).z, Quaternion.FromToRotation(transform.up, hit.normal).w);
                Transform ptcl = Instantiate(ParticleAssets.instance.bloodParticle, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal)).transform;
                ptcl.localPosition += ptcl.forward * 0.1f;
                ptcl.localScale = Vector3.one * 0.25f;
                float baseDamage = PlayerHandler.instance.currentItem.getAttributeValue(ItemAttribute.AttributeName.Damage);
                handler.DamagePlayerServerRPC(handler.GetComponent<Unity.Netcode.NetworkObject>(), baseDamage);
                return;
            }

            //No valid collision
            return;

            


        }


    }
}
