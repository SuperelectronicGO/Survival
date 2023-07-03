using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class FireballSpellLogic : MonoBehaviour{
    public KeyCode castKey;
    public Spell spell;
    [SerializeField] private Vector3 spawnOffsetPosition;
    public float timeBeforeCast;
    private ModifiyVFXGraphProperty modifyProperty;
    private Animator anim;
    /// <summary>
    /// Start sets initial position, VFX propertys, stores references, and plays proper animations
    /// </summary>
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
            ThrowSpell();
        }
    }
    /// <summary>
    /// Throws the spell when the cast key is pressed
    /// </summary>
    private void ThrowSpell()
    {
        //Direction to throw spell
        Vector3 throwDir = SpellManager.instance.cam.transform.forward;
        //Destroy old spell graphics
        //SpellManager.instance.AskServerDestroySpellGraphicsServerRPC(NetworkManager.Singleton.LocalClientId, PlayerHandler.instance.gameObject.GetComponent<NetworkObject>());
        //Invoke throw spell ServerRPC
        SpellManager.instance.ThrowSpellServerRPC(transform.position, spell.SpellNetworkClassToStruct(), throwDir, 2500, PlayerHandler.instance.gameObject.GetComponent<NetworkObject>(), NetworkManager.Singleton.LocalClientId);
        //Destroy graphics/held spell if server
        if (NetworkManager.Singleton.IsServer)
        {
            OnSpellThrown();
        }
        SpellManager.onAfterSpellThrown += OnSpellThrown;
    }
    /// <summary>
    /// Method that runs the logic after a spell is thrown, once it has recieved that the spell has been spawned
    /// </summary>
    public void OnSpellThrown()
    {
        thrown = true;
        //Remove blocker from reference
        PlayerHandler.instance.itemBlockers.Remove(this.gameObject);
        //Re-equip the last held item before the spell was summoned
        SpellManager.instance.ReEquipLastItem();
        SpellManager.onAfterSpellThrown -= OnSpellThrown;
        Destroy(this.gameObject);
    }
    /// <summary>
    /// Coroutine that waits until the pullout animation has finished to run logic
    /// </summary>
    /// <returns>After the pullout has ended</returns>
    private IEnumerator WaitOnPulloutEnd()
    {
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsTag("Idle") == true);
        finishedPullout = true;
        if (spell.type == Spell.SpellType.Fireball)
        {
            modifyProperty.changeMoveAllow(true);
        }
        yield break;
    }
    /// <summary>
    /// Coroutine that sets the spawn delay of a spell's graphics for things to look good
    /// </summary>
    /// <returns>After setting the property</returns>
    private IEnumerator SetMainSpawnDelay()
    {
        yield return new WaitForEndOfFrame();
        if (spell.type == Spell.SpellType.Fireball)
        {
            modifyProperty.SetGraphProperty("SpawnDelay", 1);
        }
        yield break;
    }
    /// <summary>
    /// Method that sets the initial position of the spell
    /// </summary>
    private void setInitialPosition()
    {
        transform.localPosition = spawnOffsetPosition;
        PlayerHandler.instance.anim.Play("SpellIdle");
    }
    /// <summary>
    /// Method that sets the string being used to block player equips
    /// </summary>
    /// <param name="blockerString">The reference to the blocker</param>
    public void SetItemBlocker()
    {
        PlayerHandler.instance.itemBlockers.Add(this.gameObject);
    }
}
