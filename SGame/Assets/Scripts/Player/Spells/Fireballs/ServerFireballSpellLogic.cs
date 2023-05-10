using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
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
            int effectIndex = 0;
            switch (spell.type)
            {
                default:
                    effectIndex = 0;
                    break;
                case Spell.SpellType.VoidFireball:
                    effectIndex = 1;
                    break;
            }
            SpellManager.instance.SpawnSpellDeathEffectClientRPC(transform.position, rot, effectIndex);
            hit = true;
            Destroy(this.gameObject);

        }
    }
}
