using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballSpellLogic:MonoBehaviour{
    public KeyCode castKey;
    private void Start()
    {
       
    }
    void Update()
    {
        if (Input.GetKeyDown(castKey))
        {
            SpellManager.instance.SetPositionToWorld(this.transform);
            SpellManager.instance.ThrowSpell(this.gameObject);
            
        } 
    }





}
