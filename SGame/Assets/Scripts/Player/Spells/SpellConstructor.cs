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
            case Spell.SpellType.Fireball:
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
                    gameObject.AddComponent<SphereCollider>();
                    gameObject.GetComponent<SphereCollider>().radius = SpellAssets.instance.fireballConstructor.colliderRadius;
                    gameObject.AddComponent<ServerFireballSpellLogic>();
                    gameObject.GetComponent<ServerFireballSpellLogic>().spell = spell.Value;
                    Rigidbody r = gameObject.AddComponent<Rigidbody>();
                    r.isKinematic = false;
                    r.useGravity = true;
                    r.collisionDetectionMode = CollisionDetectionMode.Continuous;

                }
                break;
            case Spell.SpellType.VoidFireball:
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
                    gameObject.AddComponent<SphereCollider>();
                    gameObject.GetComponent<SphereCollider>().radius = SpellAssets.instance.voidFireballConstructor.colliderRadius;
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
}
