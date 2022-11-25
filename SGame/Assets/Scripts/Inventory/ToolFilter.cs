using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolFilter : MonoBehaviour
{
    public ToolMesh[] tools;
    public Camera camera;
   
 
    private void Start()
    {
       
    }
    private void Update()
    {
       
    }
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
                Debug.Log("Hit " + hitObj.name);
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
