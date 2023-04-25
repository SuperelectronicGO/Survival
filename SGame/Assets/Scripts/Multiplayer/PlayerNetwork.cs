using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
public class PlayerNetwork : NetworkBehaviour
{
    public bool testing;
    public static PlayerNetwork instance;
    [SerializeField] private GameObject playerPrefab;
    //Store a reference to every type the player will need upon spawning
    [Header("Main Canvases")]
    [SerializeField] private Canvas mainCanvasOne;
    [SerializeField] private Canvas mainCanvasTwo;
    [SerializeField] private Canvas networkCanvas;
    [SerializeField] private CraftingManager craftingManager;
    [SerializeField] private Inventory inventory;
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private HotbarManager hotbarManager;
    [SerializeField] private UIManager uiManager;
    private CrosshairManager crosshairManager;
    [Header("Crosshair Manager")]
    [SerializeField] private Canvas crosshairCanvas;
    [SerializeField] private Image mainCrosshairComponent;
    [SerializeField] private Image secondaryCrosshairComponent;
    [Header("World Text")]
    [SerializeField] private WorldItemTextManager worldTextManager;

    private void Start()
    {
        instance = this;
        /*Steamworks.Friend[] lobbyMembers = GameNetworkManager.Instance.GetCurrentPlayers();
        for(int i=0; i<lobbyMembers.Length; i++)
        {
            if (lobbyMembers[i].Id == Steamworks.SteamClient.SteamId)
            {

            }
        }*/

        // SpawnPlayerServerRPC(NetworkManager.Singleton.LocalClientId);
        
            
             
    }
    public void SpawnPlayer()
    {
        SpawnPlayerServerRPC(NetworkManager.LocalClientId);
        Debug.Log("Spawned player");
        NetworkManagerUI.instance.spawnCamera.gameObject.SetActive(false);
    }
    public void SpawnPlayerTest()
    {
        Debug.Log(NetworkManager.LocalClientId);
        StartCoroutine(funnyTest());
        
    }
    public IEnumerator funnyTest()
    {
        
        yield return new WaitForSeconds(2);
        SpawnPlayerServerRPC(NetworkManager.LocalClientId);

        NetworkManagerUI.instance.spawnCamera.gameObject.SetActive(false);
        yield break;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void assignPlayerStartValues(GameObject playerObj)
    {
        PlayerHandler.instance = playerObj.GetComponent<PlayerHandler>();
        crosshairManager = playerObj.GetComponent<CrosshairManager>();
        instance.hotbarManager = hotbarManager;
        mainCanvasOne.gameObject.SetActive(true);
        mainCanvasTwo.gameObject.SetActive(true);
        networkCanvas.gameObject.SetActive(false);
        playerObj.GetComponent<MovePlayer>().cManager = craftingManager;
        playerObj.GetComponent<PlayerHandler>().inventory = inventory;
        playerObj.GetComponent<PlayerHandler>().buildingManager = buildingManager;
        playerObj.GetComponent<SpellManager>().setHotbarManager(hotbarManager);
        crosshairManager.canvas = crosshairCanvas;
        crosshairManager.crosshair = mainCrosshairComponent;
        crosshairManager.secondaryCrosshairComponent = secondaryCrosshairComponent;
        playerObj.transform.GetChild(0).GetComponent<MouseLook>().uiManager = uiManager;
        playerObj.transform.GetChild(0).GetComponent<CameraRaycast>().setInventory(inventory);
        playerObj.GetComponent<SpellManager>().setPlayer(playerObj.GetComponent<PlayerHandler>());
        inventory.player = playerObj;
        hotbarManager.player = playerObj.GetComponent<PlayerHandler>();
        buildingManager.setPlayer(playerObj);
        buildingManager.playerCam = playerObj.transform.GetChild(0).GetComponent<Camera>();
        inventory.refreshSlotValues(inventory.slots);
        hotbarManager.FirstTimeSlotEnable();
        //Send crosshair manager setup right after OnEnable
        crosshairManager.LateSetup();
        //Set player head of world text manager
        worldTextManager.SetPlayerHead(playerObj.transform.GetChild(0));
        
    }

    //ServerRpc to spawn the player
    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayerServerRPC(ulong ownerId)
    {
        GameObject play = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        play.GetComponent<NetworkObject>().SpawnAsPlayerObject(ownerId);
    }
   
}
