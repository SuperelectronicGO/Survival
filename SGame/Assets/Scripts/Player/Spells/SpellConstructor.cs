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
        //Debug.Log(spell.type);
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
                    Rigidbody r = gameObject.AddComponent<Rigidbody>();
                    r.isKinematic = false;
                    r.useGravity = true;
                    r.collisionDetectionMode = CollisionDetectionMode.Continuous;
                    gameObject.AddComponent<ServerFireballSpellLogic>();
                    gameObject.GetComponent<ServerFireballSpellLogic>().spell = spell.Value;


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
                    Rigidbody r = gameObject.AddComponent<Rigidbody>();
                    r.isKinematic = false;
                    r.useGravity = true;
                    r.collisionDetectionMode = CollisionDetectionMode.Continuous;
                    gameObject.AddComponent<ServerFireballSpellLogic>();
                    gameObject.GetComponent<ServerFireballSpellLogic>().spell = spell.Value;


                }
                break;
        }
    }
    private IEnumerator DelayTrailEnable()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        graphics.transform.GetChild(0).gameObject.SetActive(true);
        yield break;
    }
}
