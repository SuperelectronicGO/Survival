using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolFilter : MonoBehaviour
{
   
    public ToolMesh[] tools;
    public Camera camera;

    public int currentDamage = 0;
 
 
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

    public void objectInteraction()
    {


    }
    public void collisionExecution()
    {

        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit,3.5f))
        {
            GameObject hitObj = hit.transform.gameObject;
            if(hitObj.TryGetComponent(out WorldObject worldObj))
            {
                //Success!

                //Get the rotation at the hit point to spawn particles
                Quaternion rotQuat = new Quaternion(Quaternion.FromToRotation(transform.up, hit.normal).x, Quaternion.FromToRotation(transform.up, hit.normal).y, Quaternion.FromToRotation(transform.up, hit.normal).z, Quaternion.FromToRotation(transform.up, hit.normal).w);
                Instantiate(worldObj.hitParticle, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal), worldObj.transform);



                worldObj.objectCollision();
            }
            else
            {
                //Didn't hit a mineable object
                Debug.Log("Hit failed");
                return;
                
            }

            


        }


    }
}
