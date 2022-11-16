using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CameraRaycast : MonoBehaviour
{
    public Camera camera;
    public TextMeshProUGUI displayText;
    public GameObject selectedObject;
    [SerializeField] private Inventory inventory;
    void Update()
    {

        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit)) {


            selectedObject = hit.transform.gameObject;



        }

        if (selectedObject!=null) { 
        if (selectedObject.GetComponent<ReturnRaycastData>() != null)
        {
            ReturnRaycastData rayData = selectedObject.GetComponent<ReturnRaycastData>();
            if (rayData.isItem)
            {
                Item item = selectedObject.GetComponent<DroppedItem>().item;
                if (item.Stackable()) {
                    displayText.text = item.itemName() + " x" + item.amount;
                }
                else {
                    displayText.text = item.itemName();
                }




                if (Input.GetKeyDown(KeyCode.F)) {
                    inventory.AddItem(selectedObject.GetComponent<DroppedItem>().item);
                    Destroy(selectedObject);
                }
            }

        }
        else
        {
            selectedObject = null;
            displayText.text = string.Empty;
        }

    }

    }
}
