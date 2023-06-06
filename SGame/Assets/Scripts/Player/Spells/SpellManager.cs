using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Mathematics;
public class SpellManager : NetworkBehaviour
{
    [SerializeField] private KeyCode spellKeyOne, spellKeyTwo, spellKeyThree;
    public static SpellManager instance { get; private set; }
    public GameObject cam;
    [SerializeField] private AnimationCurve chargeGraph;
    public float currentRetractSpeed, maxCrosshairExpansion;
    [NonReorderable]
    public List<Spell> currentSpells = new List<Spell>();
    //Data
    private float chargeAmount = 0;
    private CrosshairManager crosshairManager;
    [SerializeField] private HotbarManager hotbarManager;
    [SerializeField] private PlayerHandler player;
    [SerializeField] private Transform spellAnchor;
    //Network object of the player
    private NetworkObject playerNetworkObject;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            instance = this;
        }
        playerNetworkObject = GetComponent<NetworkObject>();
    }

    //When we load in, if we are not the owner of this object, and it has a spell equipped, spawn its graphics
    private void Start()
    {
        //Spawns spell graphics upon connection if applicable
        if (!IsOwner)
        {
            if (GetComponent<PlayerHandler>().currentItem.hasAttribute(ItemAttribute.AttributeName.AllowsSpell))
            {
                GameObject g;
                switch (GetComponent<PlayerHandler>().currentItem.spell.type)
                {
                    case Spell.SpellType.Fireball:
                        g = Instantiate(SpellAssets.instance.fireballSpellGraphics, spellAnchor.transform.position, Quaternion.identity, spellAnchor.transform);
                        GetComponent<NonHostComponents>().currentSpellGraphics = g;
                        break;
                    case Spell.SpellType.VoidFireball:
                        g = Instantiate(SpellAssets.instance.voidFireballSpellGraphics, spellAnchor.transform.position, Quaternion.identity, spellAnchor.transform);
                        GetComponent<NonHostComponents>().currentSpellGraphics = g;
                        break;
                }
                
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            //Handle charging spells
            if (player.ableToMouseLook && Input.GetMouseButton(1) && !(chargeAmount > 99.9f) && player.currentItem.hasAttribute(ItemAttribute.AttributeName.AllowsSpell))
            {
                OnSpellChargeChange(true);
            }
            else if (chargeAmount > 0 && !Input.GetMouseButton(1))
            {
                OnSpellChargeChange(false);
            }


            #region Check for spell activation keypresses
            if (!player.currentItem.hasAttribute(ItemAttribute.AttributeName.AllowsSpell))
            {
                if (Input.GetKeyDown(spellKeyOne))
                {
                    CheckForSpellEquip(0);
                }
                else if (Input.GetKeyDown(spellKeyTwo))
                {
                    CheckForSpellEquip(1);
                }
                else if (Input.GetKeyDown(spellKeyThree))
                {
                    CheckForSpellEquip(2);
                }
            }
            #endregion
        }
    }

    #region General functions
    /// <summary>
    /// Method that re-equips the last used item after a spell has been used
    /// </summary>
    public void ReEquipLastItem()
    {
        hotbarManager.ReSelectSlot();
    }
    /// <summary>
    /// Method that checks if a spell rune exists to use at the given idnex
    /// </summary>
    /// <param name="index">The index to check for a rune</param>
    public void CheckForSpellEquip(int index)
    {
        for(int i=0; i < currentSpells.Count; i++)
        {
            if(currentSpells[i].spellActivationIndex == index)
            {
                EquipSpell(currentSpells[i]);
            }
        }
    }
    /// <summary>
    /// Method that equips a spell depending on the type. Adds a itemBlocker to the player.
    /// </summary>
    /// <param name="spell">The type of spell to equip</param>
    public void EquipSpell(Spell spell)
    {
        GameObject g;
        switch (spell.type)
        {
            case Spell.SpellType.Fireball:
                
                //Make sure anchor position is 0,0,0
                spellAnchor.localPosition = Vector3.zero;
                g = Instantiate(SpellAssets.instance.HeldFireballSpell, spellAnchor.position, Quaternion.identity, spellAnchor);
                FireballSpellLogic spellLogicReference = g.GetComponent<FireballSpellLogic>();
                spellLogicReference.spell = spell;
                spellLogicReference.timeBeforeCast = spell.getAttributeValue(SpellAttribute.AttributeType.summonTime);
                spellLogicReference.SetItemBlocker("Fireball");
                //If the activation index was one of the three keys tied to runes, set the rune slot as the active player slot so we don't use tools as well
                player.SetActiveSlot(RuneManager.instance.GetSpellSlot(spell.spellActivationIndex));
                //Deselect all hotbar slots
                hotbarManager.DeselectAllSlots();
                AskServerSpawnSpellGraphicsServerRPC(NetworkManager.LocalClientId, spell.SpellNetworkClassToStruct(spell), GetComponent<NetworkObject>());
                return;
            case Spell.SpellType.VoidFireball:
                
                //Make sure anchor position is 0,0,0
                spellAnchor.localPosition = Vector3.zero;
                g = Instantiate(SpellAssets.instance.HeldVoidFireballSpell, spellAnchor.position, Quaternion.identity, spellAnchor);
                g.GetComponent<FireballSpellLogic>().spell = spell;
                g.GetComponent<FireballSpellLogic>().timeBeforeCast = spell.getAttributeValue(SpellAttribute.AttributeType.summonTime);
                //If the activation index was one of the three keys tied to runes, set the rune slot as the active player slot so we don't use tools as well
                player.SetActiveSlot(RuneManager.instance.GetSpellSlot(spell.spellActivationIndex));
                //Deselect all hotbar slots
                hotbarManager.DeselectAllSlots();
                //Spawn graphics for other clients
                AskServerSpawnSpellGraphicsServerRPC(NetworkManager.LocalClientId, spell.SpellNetworkClassToStruct(spell), GetComponent<NetworkObject>());
                return;
        }
    }
   
    //Method called to update spell charge values
    private void OnSpellChargeChange(bool positive)
    {
        if (positive)
        {
            float amount = chargeGraph.Evaluate(chargeAmount / 100);
            chargeAmount += amount*player.currentItem.spell.getAttributeValue(SpellAttribute.AttributeType.chargeSpeed);
            if (chargeAmount > 100)
            {
                chargeAmount = 100;
            }
           
            
        }
        else
        {
            chargeAmount -= currentRetractSpeed;
        }
        if (chargeAmount < 0)
        {
            chargeAmount = 0;
            CrosshairManager.instance.updateCrosshair(CrosshairManager.CrosshairType.Fireball, CrosshairManager.ChangeMode.Reset, 0, 0);
        }
        else
        {
            CrosshairManager.instance.updateCrosshair(CrosshairManager.CrosshairType.Fireball, CrosshairManager.ChangeMode.Lerp, chargeAmount, maxCrosshairExpansion);
        }
       
    }
    #endregion

    #region Setup functions
    /// <summary>
    /// Method that sets the HotbarManager of the SpellManager
    /// </summary>
    /// <param name="manager">The HotbarManager reference</param>
    public void setHotbarManager(HotbarManager manager)
    {
        hotbarManager = manager;
    }
    /// <summary>
    /// Method that sets the PlayerHandler of the SpellManager
    /// </summary>
    /// <param name="handler">The PlayerManager reference</param>
    public void setPlayer(PlayerHandler handler)
    {
        player = handler;
    }
    #endregion

    #region Network RPCs
    /// <summary>
    /// ServerRPC that asks the server to throw the spell
    /// </summary>
    /// <param name="originalPosition">The position the spell is at when thrown</param>
    /// <param name="spell">The type of spell being thrown</param>s
    /// <param name="throwDirection">The direction the spell should travel in</param>
    /// <param name="force">The amount of force to throw the spell with</param>
    [ServerRpc]
    public void ThrowSpellServerRPC(Vector3 originalPosition, SpellNetworkStruct spell, Vector3 throwDirection, float force)
    {
        //Instantiate the spell template and spawn it
        GameObject g = Instantiate(SpellAssets.instance.spellTemplate, new Vector3(originalPosition.x, originalPosition.y, originalPosition.z), Quaternion.identity);
        g.GetComponent<SpellConstructor>().SpawnSpell(spell);
        //Enable the first child - it should be the trail component
        g.transform.GetChild(0).gameObject.SetActive(true);
        //Set the property of the shader graph to show its been thrown
        g.transform.GetChild(0).GetComponent<ModifiyVFXGraphProperty>().SendVFXGraphEvent("Detatch");
        g.GetComponent<Rigidbody>().AddForce(throwDirection * force, ForceMode.Force);
    }
    /// <summary>
    /// ClientRPC that spawns the spell graphics before being thrown on the client
    /// </summary>
    /// <param name="senderId">The network Id of the player spawning the spell</param>
    /// <param name="spellStruct">The spell being spawned</param>
    /// <param name="playerObject">The player object to spawn the spell on</param>
    [ClientRpc]
    public void SpawnSpellGraphicsClientRPC(ulong senderId, SpellNetworkStruct spellStruct, NetworkObjectReference playerObject)
    {
        if (playerObject.TryGet(out NetworkObject obj))
        {
            Debug.Log("recieved!");
            //Make sure the owner of the spell doesn't get targeted by the ClientRPC
            if (NetworkManager.LocalClientId != senderId)
            {
                Debug.Log("We are switching");
                GameObject g;
                switch (spellStruct.type)
                {

                    case Spell.SpellType.Fireball:
                        g = Instantiate(SpellAssets.instance.fireballSpellGraphics, obj.GetComponent<SpellManager>().spellAnchor.transform.position, Quaternion.identity, obj.GetComponent<SpellManager>().spellAnchor.transform);
                        obj.GetComponent<NonHostComponents>().currentSpellGraphics = g;
                        break;
                    case Spell.SpellType.VoidFireball:
                        g = Instantiate(SpellAssets.instance.voidFireballSpellGraphics, obj.GetComponent<SpellManager>().spellAnchor.transform.position, Quaternion.identity, obj.GetComponent<SpellManager>().spellAnchor.transform);
                        obj.GetComponent<NonHostComponents>().currentSpellGraphics = g;
                        break;
                }
                Debug.Log("Spawned a spell of type " + spellStruct.type.ToString());
            }
        }
        
        
    }
    /// <summary>
    /// The ServerRPC that runs the Spawn Spell Graphics Client RPC
    /// </summary>
    /// <param name="senderId">The network Id of the player spawning the spell</param>
    /// <param name="spellStruct">The spell being spawned</param>
    /// <param name="playerObject">The player object to spawn the spell on</param>
    [ServerRpc(RequireOwnership = false)]
    public void AskServerSpawnSpellGraphicsServerRPC(ulong senderId, SpellNetworkStruct spellStruct, NetworkObjectReference playerObject)
    {
        SpawnSpellGraphicsClientRPC(senderId, spellStruct, playerObject);
    }
    /// <summary>
    /// Client RPC that destroys the graphics of a spell when thrown
    /// </summary>
    /// <param name="senderId">The network Id of the player throwing the spell</param>
    /// <param name="playerObject">The player object that houses the spell graphics</param>
    [ClientRpc]
    public void DestroySpellGraphicsClientRPC(ulong senderId, NetworkObjectReference playerObject)
    {
        if (senderId != NetworkManager.LocalClientId)
        {
            if(playerObject.TryGet(out NetworkObject obj))
            {
                obj.GetComponent<NonHostComponents>().DestroyGraphics();
            }
        }
    }
    /// <summary>
    /// Server RPC that runs the DestroySpellGraphics Client RPC
    /// </summary>
    /// <param name="senderId">The network Id of the player throwing the spell</param>
    /// <param name="playerObject">The player obhect that houses the spell graphics</param>
    [ServerRpc(RequireOwnership = false)]
    public void AskServerDestroySpellGraphicsServerRPC(ulong senderId, NetworkObjectReference playerObject)
    {
        DestroySpellGraphicsClientRPC(senderId, playerObject);
    }
    /// <summary>
    /// Client RPC that spawns a spell death effect
    /// </summary>
    /// <param name="spawnPosition">The position to spawn the effect at</param>
    /// <param name="spawnRotation">The rotation to spawn the effect at</param>
    /// <param name="effectIndex">The index of the effect in the array of death effects</param>
    [ClientRpc]
    public void SpawnSpellDeathEffectClientRPC(Vector3 spawnPosition, Quaternion spawnRotation, int effectIndex)
    {
        Instantiate(SpellAssets.instance.spellEffects[effectIndex], spawnPosition, spawnRotation);
    }
    #endregion
}

#region Class/Struct data
/// <summary>
/// The spell class containing data about the spell
/// </summary>
[System.Serializable]
public class Spell
{
    //Each spell keypress corresponds to a certian activationIndex - for example, the three rune slots are 0[Q], 1[E], and 2[C]
    public int spellActivationIndex;
    public enum SpellType
    {
        None,
        Fireball,
        VoidFireball
    }
    public SpellType type;
    [NonReorderable]
    public List<SpellAttribute> attributes = new List<SpellAttribute>();

    public bool hasAttribute(SpellAttribute.AttributeType attribute)
    {
        foreach(SpellAttribute a in attributes)
        {
            if (a.attribute == attribute)
            {
                return true;
            }
        }
        return false;
    }
    public float getAttributeValue(SpellAttribute.AttributeType attribute)
    {
        foreach (SpellAttribute a in attributes)
        {
            if (a.attribute == attribute)
            {
                return a.value;
            }
        }
        Debug.LogError("Attribute does not exist");
        return 0;
        
    }

    /// <summary>
    /// Method that converts a spell in class form to its struct equivelent
    /// </summary>
    /// <param name="spell"></param>
    /// <returns>A copy of the spell in struct form</returns>
    public SpellNetworkStruct SpellNetworkClassToStruct(Spell spell)
    {
        //Check if spell is null, and throw if it is
        if(spell == null)
        {
            throw new System.NullReferenceException();
        }
        //Define a new SpellNetworkStruct
        SpellNetworkStruct spellStruct = new SpellNetworkStruct
        {
            type = spell.type,
            attributeTypes = new SpellAttribute.AttributeType[spell.attributes.Count],
            attributeValues = new float[spell.attributes.Count]
        };
        //Copy over spell attribute values
        for(int i=0; i<spell.attributes.Count; i++)
        {
            spellStruct.attributeTypes[i] = spell.attributes[i].attribute;
            spellStruct.attributeValues[i] = spell.attributes[i].value;
        }
        //Return the final struct
        return spellStruct;
    }
    /// <summary>
    /// Method that converts a spell in struct form to its class equivelent
    /// </summary>
    /// <param name="spellStruct"></param>
    /// <returns>A copy of the spell struct as a class</returns>
    public Spell SpellNetworkStructToClass(SpellNetworkStruct spellStruct)
    {
        //Create a new spell
        Spell spell = new Spell();
        spell.type = spellStruct.type;
        spell.attributes = new List<SpellAttribute>();
        for(int i=0; i<spellStruct.attributeValues.Length; i++)
        {
            spell.attributes.Add(new SpellAttribute { attribute = spellStruct.attributeTypes[i], value = spellStruct.attributeValues[i] });
        }
        //Return the spell
        return spell;
    }

}
/// <summary>
/// Class that can be used to apply modifiers/additional data to a spell
/// </summary>
[System.Serializable]
public class SpellAttribute{
     public enum AttributeType
     {
    chargeSpeed,
    summonTime,
    damage,

     }
    public AttributeType attribute;
    public float value;
}

/// <summary>
/// Spell class in struct form so it can be used over the network
/// </summary>
[System.Serializable]
public struct SpellNetworkStruct : INetworkSerializable
{
    public Spell.SpellType type;
    //Attribute types and values
    public SpellAttribute.AttributeType[] attributeTypes;
    public float[] attributeValues;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref type);
        serializer.SerializeValue(ref attributeTypes);
        serializer.SerializeValue(ref attributeValues);
    }
}
#endregion

