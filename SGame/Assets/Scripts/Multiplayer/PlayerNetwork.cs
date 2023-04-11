using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerNetwork : MonoBehaviour
{

    public static PlayerNetwork instance;
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
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void assignPlayerStartValues(GameObject playerObj)
    {
        crosshairManager = playerObj.GetComponent<CrosshairManager>();
        PlayerHandler.instance = playerObj.GetComponent<PlayerHandler>();
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
   
}
