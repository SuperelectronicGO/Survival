using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballSpellLogic:MonoBehaviour{
    public KeyCode castKey;
    [SerializeField] private Vector3 spawnOffsetPosition;
    private void Start()
    {
        setInitialPosition();
    }
    void Update()
    {
        if (Input.GetKeyDown(castKey))
        {
            SpellManager.instance.SetPositionToWorld(this.transform);
            SpellManager.instance.ThrowSpell(this.gameObject);
            
        } 
    }

    private void setInitialPosition()
    {
        PlayerHandler.instance.anim.Play("SpellIdle");
        transform.localPosition = spawnOffsetPosition;
    }




}
