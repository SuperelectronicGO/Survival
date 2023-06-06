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
        RunCameraRaycast();
    }
    /// <summary>
    /// Method that uses a raycast to check for interactability in the world
    /// </summary>
    private void RunCameraRaycast()
    {
        RaycastHit hit;
        Ray ray = rayCamera.ScreenPointToRay(Input.mousePosition);
        //Raycast to check interactability
        if (Physics.Raycast(ray, out hit, maxDistance: 15, layerMask: ~ignoreLayers))
        {
            //Store hit object
            selectedObject = hit.transform.gameObject;
            //Check if a different object is hit than last frame
            if (selectedObject != oldObject)
            {
                //If it is, set that object as the new one
                oldObject = selectedObject;
                //Check if this object has the returnRaycastData script, and if it does, cache it, and run the raycastDataOnce function
                if (selectedObject.TryGetComponent(out ReturnRaycastData data))
                {
                    //If hit, assign data reference and run raycast logic
                    raycastData = data;
                    RunRaycastLogicOnce();
                }
                else
                {
                    //If not hit, set data to null and deactivate any item text
                    raycastData = null;
                    SendDeactivateItemText();
                }
            }



            if (raycastData != null)
            {
                if (raycastData.isItem)
                {
                    CheckItemRaycastOnKeypress();
                }
                else if (raycastData.isChest)
                {
                    CheckChestRaycastOnKeypress();
                }
            }





        }
        else
        {
            raycastData = null;
            if (selectedObject != null)
            {
                SendDeactivateItemText();
                selectedObject = null;
            }
        }
    }
    #region World text events
    public delegate void UpdateWorldText(string displayText, GameObject objectToFollow, bool enableText);
    public static event UpdateWorldText onSetWorldText;
    #endregion
    /// <summary>
    /// Method that handles logic for checking raycast data 
    /// </summary>
    public void RunRaycastLogicOnce()
    {
        //If raycast data isn't null, set text
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
            else if (raycastData.isChest)
            {
                onSetWorldText("Chest", selectedObject, true);
            }

        }
        else
        {
            SendDeactivateItemText();
        }
    }
    /// <summary>
    /// Method that sends the event to deactive the world item text.
    /// </summary>
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
    /// <summary>
    /// Method that sets the inventory reference
    /// </summary>
    /// <param name="inv">The inventory reference to set as</param>
    public void setInventory(Inventory inv)
    {
        inventory = inv;
    }


}
