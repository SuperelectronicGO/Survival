using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraRaycast : MonoBehaviour
{
    public Camera camera;
    public GameObject pickupText;
    public GameObject assets;
    void Update()
    {
        
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
           
            Transform objectHit = hit.transform;
            if (objectHit.tag == "pickup")
            {
                
                if (!pickupText.activeInHierarchy)
                {
                    pickupText.SetActive(true);
                }
                GameObject obj = objectHit.gameObject;
               string displayText = obj.GetComponent<PickupableObject>().heldItem.itemName() + " x"+obj.GetComponent<PickupableObject>().heldItem.amount.ToString();
                pickupText.GetComponent<Text>().text = displayText;

                if (Input.GetKeyDown(KeyCode.F))
                {
                    assets.GetComponent<ItemAdder>().addItem(obj.GetComponent<PickupableObject>().heldItem);
                    Destroy(obj.gameObject);
                }

            }
            else
            {
                if (pickupText.activeInHierarchy)
                {
                    pickupText.SetActive(false);
                }
            }
           
        }
    }
}
