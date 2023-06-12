using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Mathematics;
using System;
public class ServerFireballSpellLogic : NetworkBehaviour
{
    public SpellNetworkStruct spell;
    private Rigidbody r;
    [SerializeField] private int playerLayerIndex = 9;
    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Rigidbody>();
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
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != playerLayerIndex)
        {
            ContactPoint contact = collision.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            byte effectIndex = 0;
            switch (spell.type)
            {
                default:
                    effectIndex = 0;
                    break;
                case 2: //Void fireball index = 2
                    effectIndex = 1;
                    break;
            }

            NetworkHalf3 spawnPosition = new NetworkHalf3();
            spawnPosition.x.data.Value = (half)transform.position.x;
            spawnPosition.y.data.Value = (half)transform.position.y;
            spawnPosition.z.data.Value = (half)transform.position.z;
            Debug.LogError("Asked to despawn spells");
            SpellManager.instance.SpawnSpellDeathEffectClientRPC(spawnPosition, rot, effectIndex);
            hit = true;
            GetComponent<NetworkObject>().Despawn(true);

        }
    }
}
