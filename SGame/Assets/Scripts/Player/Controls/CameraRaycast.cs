using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
public class CameraRaycast : NetworkBehaviour
{
    private Item selectedItem;
    public Camera rayCamera;
    [SerializeField] private GameObject selectedObject;
    [SerializeField] private GameObject oldObject;
    private ReturnRaycastData raycastData;
    [SerializeField] private Inventory inventory;
    [SerializeField] private LayerMask ignoreLayers;
    void Update()
    {
        if (!IsOwner) return;
        RaycastHit hit;
        Ray ray = rayCamera.ScreenPointToRay(Input.mousePosition);
        //Raycast to check interactability
        if (Physics.Raycast(ray, out hit, maxDistance:15, layerMask:~ignoreLayers)) {

            //If we hit, check if its the same item we hit previously. If its not, call the function to update the data
            //Might need to be changed later if object values update without player interaction.
            selectedObject = hit.transform.gameObject;
            if (selectedObject != oldObject)
            {
                //Set the old object to be the current one
                oldObject = selectedObject;
                //Check if this object has the returnRaycastData script, and if it does, cache it, and run the raycastDataOnce function
                if (selectedObject.GetComponent<ReturnRaycastData>() != null)
                {
                    raycastData = selectedObject.GetComponent<ReturnRaycastData>();

                    RunRaycastLogicOnce();
                }
                else{
                    raycastData = null;
                    SendDeactivateItemText();
                }
            }
            if (raycastData != null)
            {
                if (raycastData.isItem)
                {
                    CheckItemRaycastOnKeypress();
                }else if (raycastData.isChest)
                {
                    CheckChestRaycastOnKeypress();
                }
            }

        

      

    }

    }
    public delegate void UpdateWorldText(string displayText, GameObject objectToFollow, bool enableText);
    public static event UpdateWorldText onSetWorldText;
    public void RunRaycastLogicOnce()
    {
        if (raycastData != null)
        {
            if (raycastData.isItem)
            {
                Item item = selectedObject.GetComponent<DroppedItem>().item;
                selectedItem = item;
                if (item.Stackable())
                {
                   
                    if (onSetWorldText != null)
                    {
                        onSetWorldText(item.itemName() + " x" + item.amount, selectedObject, true);
                    }
                }
                else
                {
                    
                    if (onSetWorldText != null)
                    {
                        onSetWorldText(item.itemName(), selectedObject, true);
                    }
                }




             
            }

        }
        else
        {
            onSetWorldText(null, null, false);
        }
    }
    public void SendDeactivateItemText()
    {
        onSetWorldText(null, null, false);
    }
    /// <summary>
    /// Attempts to pick up the current item and destroy its object.
    /// </summary>
    public void CheckItemRaycastOnKeypress()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            
            inventory.AddItem(selectedItem);
            PlayerHandler.instance.DestroyItemServerRPC(selectedObject);
            return;
        }
    }
    /// <summary>
    /// Attempts to open the current storage. 
    /// </summary>
    public void CheckChestRaycastOnKeypress()
    {
        if (Input.GetKeyDown(KeyCode.F) && !UIManager.instance.inventoryOpen)
        {
            if (selectedObject.TryGetComponent(out NetworkStorage storage))
            {
                StorageManager.instance.SetCurrentNetworkStorage(storage);
                UIManager.instance.OpenInventory(false, true, chestSlots: storage.slotAmount, storageReference: storage);
            }
            else
            {
                throw new System.NullReferenceException("No NetworkStorage component attachted to this gameobject!");
            }

        }
    }
    public void setInventory(Inventory inv)
    {
        inventory = inv;
    }


}
