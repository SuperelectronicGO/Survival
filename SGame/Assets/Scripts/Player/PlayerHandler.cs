using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine.SceneManagement;
public class PlayerHandler : NetworkBehaviour
{
    //Declare a singleton for this script
    public static PlayerHandler instance { get; set; }

    [Header("Inventory Logic")]

    //The inventory component
    public Inventory inventory;

    //Component to manage building
    public BuildingManager buildingManager;

    //Current slot we have equipped
    [HideInInspector] public ISInterface currentSlot;

    //Gameobject with the animator component
    public GameObject toolAnchor;

    //Component to control the current tool equipped
    [SerializeField] private ToolFilter toolFilter;

    //Animator for the player
    public Animator anim;

    //Hotbar manager
    public HotbarManager hotbarManager;

    //Action blockers
    [HideInInspector] public List<GameObject> mouseBlockers = new List<GameObject>();
    [HideInInspector] public List<GameObject> KeyBlockers = new List<GameObject>();
    [HideInInspector] public List<GameObject> itemBlockers = new List<GameObject>();
    public bool ableToMouseLook = true;
    public bool ableToKey = true;
    public bool ableToItem = true;

    /* * * Network Variables * * */
    //Health
    [SerializeField] private NetworkVariable<float> playerHealth = new NetworkVariable<float>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    //Max health
    [SerializeField] private NetworkVariable<float> playerMaxHealth = new NetworkVariable<float>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    //Current Item we have equipped
    public Item currentItem;
    [SerializeField] private NetworkVariable<ItemNetworkStruct> currentItemNetworkStruct = new NetworkVariable<ItemNetworkStruct>(new ItemNetworkStruct(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    //Don't do shit here for other scripts, sometimes it isn't set yet
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            StartCoroutine(CheckLockConditions());
        }
        //Subscrive to the currentItemNetworkStruct changed event
        currentItemNetworkStruct.OnValueChanged += (ItemNetworkStruct previousValue, ItemNetworkStruct newValue) =>
        {
            //If not the owner, run logic when the current Item is changed 
            /* -Set the current Item
             * -Filter the current tool, if one is equipped */
            if (!IsOwner)
            {
                currentItem = newValue.ToClass();
                OnEquipNotUserOwned(currentItem);
            }
        };
        //Set the animator component
        anim = toolAnchor.GetComponent<Animator>();
        if (!IsOwner)
        {
            currentItem = currentItemNetworkStruct.Value.ToClass();
            OnEquipNotUserOwned(currentItem);
            transform.GetChild(0).GetComponent<AudioListener>().enabled = false;
            GetComponent<KinematicCharacterController.KinematicCharacterMotor>().enabled = false;
            GetComponent<PlayerCharacterController>().enabled = false;
        }
    }
    private void Awake()
    {

        
        if (!IsOwner)
        {
            gameObject.AddComponent<NonHostComponents>();
        }

    }
    void Update()
    {
        /*If owner, and active item changes, run logic to
        * -Equip the item
        * -Set the current Item and call the ServerRPC to call the Client RPC to sync the active item
        * -Change crosshair if needed
        * -Use item if mouse pressed
        */
        if (IsOwner)
        {
            //If the current item we have equipped isn't the one in our active slot, then equip the new item, and play a pullout animation if needed
            if (currentItem != currentSlot.heldItem)
            {
                OnEquip(currentSlot.heldItem);
                currentItem = currentSlot.heldItem;
                currentItemNetworkStruct.Value = currentItem.ToStruct();
                CrosshairManager.instance.ChangeCrosshairOnItem(currentItem);
            }
            if (Input.GetMouseButtonDown(0) && ableToMouseLook)
            {
                UseItem(currentSlot.heldItem);
            }
        }


        
    }

    private void fuckStolkey(int funny, string funn2)
    {

    }
    /// <summary>
    /// Method that drops an item by invoking server RPCs
    /// </summary>
    /// <param name="item">The item to drop</param>
    public void DropItem(Item item)
    {

        DropItemServerRPC(item.ToStruct());
        return;
    }
    /// <summary>
    /// Method that is called when an item is equipped
    /// </summary>
    /// <param name="item">The item that was equipped</param>
    public void OnEquip(Item item)
    {
        toolFilter.filterTools(item.itemType);
        bool build = item.hasAttribute(ItemAttribute.AttributeName.EnablesBuilding);
        buildingManager.buildingEnabled = build;
        if (build)
        {
            buildingManager.selectPrefab(item);
        }

        switch (item.itemType)
        {
            default:
                return;
            case Item.ItemType.StoneHatchet:
            case Item.ItemType.ShellAxe:

                anim.Play("Pullout");


                return;
        }

    }
    /// <summary>
    /// Method that is called when a connected, non-owned player equips an item
    /// </summary>
    /// <param name="item">The item that was equipped</param>
    public void OnEquipNotUserOwned(Item item)
    {
        toolFilter.filterTools(item.itemType);
        switch (item.hasAttribute(ItemAttribute.AttributeName.AllowsSpell))
        {
            default:
            case false:
                break;
            case true:
                break;
        }
    }
    /// <summary>
    /// Method that sets the active slot the player is using
    /// </summary>
    /// <param name="slot"></param>
    public void SetActiveSlot(ISInterface slot)
    {
        currentSlot = slot;
    }
    /// <summary>
    /// Method that controls what happens when the player uses an item
    /// </summary>
    /// <param name="item">The item to check logic for</param>
    public void UseItem(Item item)
    {
        switch (item.itemType)
        {
            default:
                return;
            case Item.ItemType.StoneHatchet:
            case Item.ItemType.ShellAxe:
                if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Idle"))
                {
                    anim.Play("Swing");
                }

                return;
        }
    }
    /// <summary>
    /// Coroutine that runs forever checking if certain actions are blocked by something
    /// </summary>
    public IEnumerator CheckLockConditions()
    {
        //Check if mouse is blocked
        if (mouseBlockers.Count != 0)
        {
            ableToMouseLook = false;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            ableToMouseLook = true;
            Cursor.lockState = CursorLockMode.Locked;
            
        }
        //Check if keys are blocked
        if (KeyBlockers.Count != 0)
        {
            ableToKey = false;
        }
        else
        {
            ableToKey = true;
        }
        //Check if items are blocked
        if(itemBlockers.Count != 0)
        {
            ableToItem = false;
        }
        else
        {
            ableToItem = true;
        }
        //Wait for 0.05 seconds, then check again
        yield return new WaitForSecondsRealtime(0.05f);
        StartCoroutine(CheckLockConditions());
    }
    /// <summary>
    /// Method that sets the current item being used by the player
    /// </summary>
    /// <param name="itemToUse">The item to set as</param>
    public void SetActiveItem(Item itemToUse)
    {
        currentItem = itemToUse;
    }

    #region Damage event references
    public delegate void UpdateVitalsUI(float health, float maxHealth);
    public static event UpdateVitalsUI _UpdateVitalsUI;
    #endregion
    /// <summary>
    /// Method that damages the player
    /// </summary>
    /// <param name="amount">The amount to damage the player by</param>
    public void DamagePlayer(float amount)
    {
            playerHealth.Value -= amount;
            _UpdateVitalsUI(playerHealth.Value, playerMaxHealth.Value);
    }

    #region Server RPCs
    /// <summary>
    /// Method that drops an item by spawning it on the server
    /// </summary>
    /// <param name="itemStruct">The item data to construct the item with</param>
    [ServerRpc]
    public void DropItemServerRPC(ItemNetworkStruct itemStruct)
    {

        //Get the spawn position for this item
        Vector3 spawnPos = this.transform.position + (this.transform.forward * 2);
        spawnPos.y += .5f;
        GameObject droppedItem = Instantiate(ItemAssets.Instance.itemTemplate, spawnPos, Quaternion.identity);
        droppedItem.GetComponent<NetworkObject>().Spawn(true);
        droppedItem.GetComponent<DroppedItem>().SetItemStructValue(itemStruct);
    }
    /// <summary>
    /// Method that destroys the given item on the server
    /// </summary>
    /// <param name="objectToDestroy">The reference to the object to destroy</param>
    [ServerRpc]
    public void DestroyItemServerRPC(NetworkObjectReference objectToDestroy)
    {
        if (objectToDestroy.TryGet(out NetworkObject obj))
        {
            obj.Despawn(true);
        }
    }
    
    // **** Rewrite to be more efficient ****


    [ServerRpc(RequireOwnership = false)]
    //Damage the player on the server - this is for when we are attacked by creatures or other players
    public void DamagePlayerServerRPC(NetworkObjectReference playerToDamage, float amount)
    {
        if (playerToDamage.TryGet(out NetworkObject obj))
        {
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] {obj.OwnerClientId}
                }
            };
            DamagePlayerClientRPC(amount, clientRpcParams);
        }
    }
    #endregion

    #region Client RPCs
    [ClientRpc]
    private void DamagePlayerClientRPC(float amount, ClientRpcParams clientRpcParams = default)
    {
        DamagePlayer(amount);
    }
    #endregion




}
