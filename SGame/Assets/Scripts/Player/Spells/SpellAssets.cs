using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellAssets : MonoBehaviour
{
   public static SpellAssets instance { get; private set; }
    private void Awake()
    {
        instance = this;
    }
    public GameObject spellTemplate;
    [Header("Graphics")]
    public GameObject fireballSpellGraphics;
    public GameObject voidFireballSpellGraphics;
    [Header("Prefabs")]
    public GameObject HeldFireballSpell;
    public GameObject HeldVoidFireballSpell;
    [Header("Constructors")]
    public SpellSpawnScriptable fireballConstructor;
    public SpellSpawnScriptable voidFireballConstructor;
    [Header("Effects")]
    public GameObject[] spellEffects;

    
}
