using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballSpellLogic:MonoBehaviour{
    public KeyCode castKey;
    public Spell spell;
    [SerializeField] private Vector3 spawnOffsetPosition;
    [SerializeField] private GameObject deathEffect;
    private Rigidbody r;
    [SerializeField] private int playerLayerIndex;
    public float timeBeforeCast;
    private ModifiyVFXGraphProperty modifyProperty;
    private Animator anim;
    private void Start()
    {
        setInitialPosition();
        r = GetComponent<Rigidbody>();
        modifyProperty = GetComponent<ModifiyVFXGraphProperty>();
        modifyProperty.changeMoveAllow(false);
        anim = GetComponent<Animator>();
        Debug.Log(timeBeforeCast);
        anim.speed = timeBeforeCast;
        Debug.Log(anim.speed);
        switch (spell.type)
        {
            case Spell.SpellType.Fireball:
                anim.Play("FireballPullout");
                break;
            case Spell.SpellType.VoidFireball:
                anim.Play("VoidFireballPullout");
                break;
        }
            

    }
    bool thrown = false;
    bool finishedPullout = false;
    void Update()
    {
        if (spell.type == Spell.SpellType.Fireball)
        {
            StartCoroutine(SetMainSpawnDelay());
        }
        if (finishedPullout==false)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Idle"))
            {
                finishedPullout = true;
                if (spell.type == Spell.SpellType.Fireball)
                {
                    modifyProperty.changeMoveAllow(true);
                }
            }
        }
        if (Input.GetKeyDown(castKey)&&!thrown&&finishedPullout)
        {
            SpellManager.instance.SetPositionToWorld(this.transform);
            SpellManager.instance.ThrowSpell(this.gameObject, spell);
            thrown = true;
            SpellManager.instance.ReEquipLastItem();
        }
    }
    private IEnumerator SetMainSpawnDelay()
    {
        yield return new WaitForEndOfFrame();
        if (spell.type == Spell.SpellType.Fireball)
        {
            modifyProperty.SetGraphProperty("SpawnDelay", 1);
        }
        yield break;
    }
    private bool hit = false;
    Vector3 velocity = Vector3.zero;
    Vector3 oldVelocity = Vector3.zero;
    private void FixedUpdate()
    {
        if (!hit)
        {
            oldVelocity = velocity;
            velocity = r.velocity;
        }
    }

    private void setInitialPosition()
    {
        PlayerHandler.instance.anim.Play("SpellIdle");
        transform.localPosition = spawnOffsetPosition;
    }
    GameObject dEffect;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != playerLayerIndex)
        {
            ContactPoint contact = collision.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            dEffect = Instantiate(deathEffect, this.transform.position, rot);
            StartCoroutine(FireballDeath());
            hit = true;
            
        }
    }
    private IEnumerator FireballDeath()
    {
        //Destroy Rigidbody and sphere collider, and send the hit event + size change to VFX graph
        Destroy(GetComponent<SphereCollider>());
        Destroy(r);
        GetComponent<ModifiyVFXGraphProperty>().SendVFXGraphEvent("Hit");
        if (spell.type == Spell.SpellType.Fireball)
        {
            GetComponent<ModifiyVFXGraphProperty>().SetGraphProperty("Size", 0);
        }
        transform.GetChild(0).gameObject.SetActive(false);
        Destroy(transform.GetChild(1).gameObject);
        yield return new WaitForSeconds(2.5f);
        Destroy(dEffect);
        Destroy(this.gameObject);
        yield break;
        
    }


}
