using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class FireballSpellLogic:MonoBehaviour{
    public KeyCode castKey;
    public Spell spell;
    [SerializeField] private Vector3 spawnOffsetPosition;
    public float timeBeforeCast;
    private ModifiyVFXGraphProperty modifyProperty;
    private Animator anim;
    private void Start()
    {
        //Set offset position of the spell if it needs it
        setInitialPosition();
        //Get the VFX graph component
        modifyProperty = GetComponent<ModifiyVFXGraphProperty>();
        modifyProperty.changeMoveAllow(false);
        //Get animator component
        anim = GetComponent<Animator>();
        //Set the animator to play quicker animations depending on cast time
        anim.speed = timeBeforeCast;
        //Play pullout animation depending on spell type
        switch (spell.type)
        {
            case Spell.SpellType.Fireball:
                anim.Play("FireballPullout");
                break;
            case Spell.SpellType.VoidFireball:
                anim.Play("VoidFireballPullout");
                break;
        }
        //Set delay before casting
        StartCoroutine(SetMainSpawnDelay());
        StartCoroutine(WaitOnPulloutEnd());
    }
    bool thrown = false;
    bool finishedPullout = false;
    void Update()
    {
        //Throw spell
        if (Input.GetKeyDown(castKey)&&!thrown&&finishedPullout)
        {
           // SpellManager.instance.SetPositionToWorld(this.transform);
           // SpellManager.instance.ThrowSpell(this.gameObject, spell);
            thrown = true;
            Vector3 throwDir = SpellManager.instance.cam.transform.forward;
            //Destroy old spell graphics
            SpellManager.instance.AskServerDestroySpellGraphicsServerRPC(NetworkManager.Singleton.LocalClientId, PlayerHandler.instance.gameObject.GetComponent<NetworkObject>());
            SpellManager.instance.ThrowSpellServerRPC(transform.position, spell.SpellNetworkClassToStruct(spell), throwDir, 2500);
            SpellManager.instance.ReEquipLastItem();
            Destroy(this.gameObject);
        }
    }
    private IEnumerator WaitOnPulloutEnd()
    {
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsTag("Idle") == true);
        finishedPullout = true;
        if (spell.type == Spell.SpellType.Fireball)
        {
            modifyProperty.changeMoveAllow(true);
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
    
    private void setInitialPosition()
    {
        PlayerHandler.instance.anim.Play("SpellIdle");
        transform.localPosition = spawnOffsetPosition;
    }
}
