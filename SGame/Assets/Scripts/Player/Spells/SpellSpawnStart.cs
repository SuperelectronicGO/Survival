using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class SpellSpawnStart : NetworkBehaviour
{
    private Spell spell;
    private bool recievedSpell = false;
    public override void OnNetworkSpawn()
    {
      
    }
    public void RecievedSpellType(Spell recievedSpell)
    {
        spell = recievedSpell;
        CreateSpell();
        Destroy(this);
    }
    public void CreateSpell()
    {

    }

}
