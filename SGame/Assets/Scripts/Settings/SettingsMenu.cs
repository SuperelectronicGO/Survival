using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SettingsMenu : MonoBehaviour
{
    [Header("Terrain")]
    public MapMagic.Core.MapMagicObject terrain;

    [Header("Grass Draw Distance")]
    public Slider grassDrawDistanceSlider;
    public Text grassDrawDistanceText;




    [Header("Misc")]
    [SerializeField] private GameObject backgroundObject;

    [SerializeField] private GameObject grass;
    private bool settingsOpen;

    private UIManager uiManager;
    // Start is called before the first frame update
    void Start()
    {
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {

            if (settingsOpen)
            {
                settingsOpen = false;
            }
            else
            {
                settingsOpen = true;
            }
        }
        if (settingsOpen)
        {
           uiManager.settingsOpen = true;
        }
        else
        {
           uiManager.settingsOpen = false;
        }
     
        
        
       
    }

   public void refreshTerrainValues()
    {
        terrain.terrainSettings.detailDistance = grassDrawDistanceSlider.value;
        terrain.ApplyTerrainSettings();
    }
    
}
