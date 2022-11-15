using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemAssets : MonoBehaviour
{
    public GameObject grenade;
    public static ItemAssets Instance { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [Header("Blank")]
    public Sprite blankSprite;
    [Header("Food")]
    public Sprite acornFlourSprite;
    [Header("Resources")]
    public Sprite smallRockSprite;
    public Sprite rockSprite;
    public Sprite limestoneSprite;
    public Sprite smallLimestoneSprite;
    public Sprite slateSprite;
    public Sprite smallSlatesprite;
    public Sprite acornSprite;
    public Sprite boneSprite;
    public Sprite oakLogSprite;
    public Sprite ashLogSprite;
    public Sprite beechLogSprite;
    public Sprite pineLogSprite;
    public Sprite spruceLogSprite;
    public Sprite smallBoneSprite;
    public Sprite oakStickSprite;
    public Sprite ashStickSprite;
    public Sprite beechStickSprite;
    public Sprite pineStickSprite;
    public Sprite spruceStickSprite;
    public Sprite oakTwigSprite;
    public Sprite ashTwigSprite;
    public Sprite beechTwigSprite;
    public Sprite pineTwigSprite;
    public Sprite spruceTwigSprite;
    public Sprite driedGrassSprite;
    public Sprite twineSprite;
    public Sprite pointedRockSprite;
    public Sprite sharpRockSprite;
    public Sprite flintSprite;
    public Sprite flintShardSprite;
    public Sprite beechNutsSprite;
    public Sprite pineBudsSprite;
    public Sprite spruceBarkSprite;
    public Sprite pineBarkSprite;
    public Sprite pinePitchSprite;
    public Sprite sprucePitchSprite;
    public Sprite spruceConeSprite;
    
   
    [Header("Melee Weapons")]
    
    public Sprite fireaxeSprite;
    [Header("Explosives")]
    public Sprite grenadeSprite;
}
