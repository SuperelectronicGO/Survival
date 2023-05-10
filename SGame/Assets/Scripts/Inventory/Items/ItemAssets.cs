using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemAssets : MonoBehaviour
{
  
    public GameObject grenade;
    public static ItemAssets Instance { get; private set; }
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [Header("Blank")]
    public Sprite blankSprite;
    
    [Header("Buildings")]
    public Sprite campfireSprite;
    public Sprite firepitSprite;
    public Sprite rockWallSprite;
    public Sprite stakeWallSprite;
    [Header("Resources")]
   
    public Sprite ashLogSprite;
    public Sprite beechLogSprite;
    public Sprite birchLogSprite;
    public Sprite oakLogSprite;
    public Sprite spruceLogSprite;
    public Sprite oakStickSprite;
    public Sprite limestoneRockSprite;
    public Sprite fieldstoneRockSprite;
    public Sprite slateRockSprite;
    public Sprite shellSpriteOne;
    public Sprite shellSpriteTwo;

    [Header("Tools")]
    public Sprite stoneHatchetSprite;
    public Sprite shellAxeSprite;
    [Header("Melee Weapons")]
    public Sprite ironSwordSprite;

    [Header("Runes")]
    public Sprite fireGrenadeRuneSprite;
    public Sprite eyeOfTheHunterRuneSprite;
    public Sprite voidFireballRuneSprite;




    [Header("")]
    [Header("Default Item Template")]
    public GameObject itemTemplate;


    [Header("")]
    [Header("")]
    [Header("Constructors")]
    public ItemSpawnScriptable oakLogSpawnScriptable;
    public ItemSpawnScriptable bagScriptable;

}
