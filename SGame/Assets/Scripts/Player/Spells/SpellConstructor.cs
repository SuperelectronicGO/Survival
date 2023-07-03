using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class SpellConstructor : NetworkBehaviour
{
    public NetworkVariable<SpellNetworkStruct> spell = new NetworkVariable<SpellNetworkStruct>();
    GameObject graphics;
    public override void OnNetworkSpawn()
    {
        if (!IsServer) StartCoroutine(WaitUntilSpellRecievedSpawnGraphics());
    }
    /// <summary>
    /// Coroutine that waits 0.1 seconds to enable the trail so it doesn't flash in the players face
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayTrailEnable()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        graphics.transform.GetChild(0).gameObject.SetActive(true);
        yield break;
    }
    /// <summary>
    /// Constructs the spell depending on the type. Clients construct graphics while the server constructs the entire spell
    /// </summary>
    private void ConstructSpell()
    {
        switch (spell.Value.type)
        {
            case 1: //Fireball spell ID is 1
                //Spawn graphics
                graphics = Instantiate(SpellAssets.instance.fireballSpellGraphics, this.transform.position, Quaternion.identity, this.transform);
                Destroy(graphics.GetComponent<Animator>());
                //Enable trail
                if (IsServer)
                {
                    graphics.transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    StartCoroutine(DelayTrailEnable());
                }
                //If server, add components
                if (IsServer)
                {
                    DelayColliderAddition(1);
                    gameObject.AddComponent<ServerFireballSpellLogic>();
                    gameObject.GetComponent<ServerFireballSpellLogic>().spell = spell.Value;
                    Rigidbody r = gameObject.AddComponent<Rigidbody>();
                    r.isKinematic = false;
                    r.useGravity = true;
                    r.collisionDetectionMode = CollisionDetectionMode.Continuous;

                }
                break;
            case 2: //Void fireball spell ID is 2
                //Spawn graphics
                graphics = Instantiate(SpellAssets.instance.voidFireballSpellGraphics, this.transform.position, Quaternion.identity, this.transform);
                Destroy(graphics.GetComponent<Animator>());
                //Enable trail
                if (IsServer)
                {
                    graphics.transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    StartCoroutine(DelayTrailEnable());
                }
                //If server, add components
                if (IsServer)
                {
                    DelayColliderAddition(2);
                    gameObject.AddComponent<ServerFireballSpellLogic>();
                    gameObject.GetComponent<ServerFireballSpellLogic>().spell = spell.Value;
                    Rigidbody r = gameObject.AddComponent<Rigidbody>();
                    r.isKinematic = false;
                    r.useGravity = true;
                    r.collisionDetectionMode = CollisionDetectionMode.Continuous;

                }
                break;
        }
    }
    private IEnumerator WaitUntilSpellRecievedSpawnGraphics()
    {
        yield return new WaitUntil(() => spell.Value.type != 0);
        ConstructSpell();
        yield break;
    }
    /// <summary>
    /// Spawns the spell
    /// </summary>
    /// <param name="spellReference">The spell to spawn with</param>
    public void SpawnSpell(SpellNetworkStruct spellReference)
    {

        GetComponent<NetworkObject>().Spawn();
        spell.Value = spellReference;
        ConstructSpell();
        
    }
    /// <summary>
    /// Coroutine that delays the addition of colliders for a brief moment to prevent random collisions
    /// </summary>
    /// <param name="spellType">The type of spell to construct a collider for</param>
    /// <returns>After creating the collider</returns>
    private IEnumerator DelayColliderAddition(byte spellType)
    {
        yield return new WaitForSecondsRealtime(0.2f);
        switch (spellType)
        {
            case 1:
                gameObject.AddComponent<SphereCollider>();
                gameObject.GetComponent<SphereCollider>().radius = SpellAssets.instance.fireballConstructor.colliderRadius;
                break;
            case 2:
                gameObject.AddComponent<SphereCollider>();
                gameObject.GetComponent<SphereCollider>().radius = SpellAssets.instance.voidFireballConstructor.colliderRadius;
                break;
        }
        yield break;
    }
}
