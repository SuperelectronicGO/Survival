using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    [SerializeField] private KeyCode spellKeyOne, spellKeyTwo, spellKeyThree;
    public static SpellManager instance { get; private set; }
    [SerializeField] private GameObject cam;
    [SerializeField] private AnimationCurve chargeGraph;
    public float currentRetractSpeed, maxCrosshairExpansion;
    [NonReorderable]
    public List<Spell> currentSpells = new List<Spell>();
    //Data
    private float chargeAmount = 0;
    private CrosshairManager crosshairManager;
    [SerializeField] private HotbarManager hotbarManager;
    private PlayerHandler player;
    [SerializeField] private Transform spellAnchor;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        player = PlayerHandler.instance;
         
    }

    // Update is called once per frame
    void Update()
    {
        //Handle charging spells
        if (player.ableToMouseLook && Input.GetMouseButton(1) && !(chargeAmount > 99.9f)&&player.currentItem.hasAttribute(ItemAttribute.AttributeName.AllowsSpell))
        {
            OnSpellChargeChange(true);
        }else if (chargeAmount > 0&&!Input.GetMouseButton(1))
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
    //Method to re-equip the last slot after a spell is finished
    public void ReEquipLastItem()
    {
        hotbarManager.ReSelectSlot();
    }
    //Method to check if we have a spell rune with the correct index
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
    //Method handling the different values when equipping spells
    public void EquipSpell(Spell spell)
    {
        GameObject g;
        switch (spell.type)
        {
            case Spell.SpellType.Fireball:
                //Make sure anchor position is 0,0,0
                spellAnchor.localPosition = Vector3.zero;
                g = Instantiate(ItemAssets.Instance.fireballSpellObject, spellAnchor.position, Quaternion.identity, spellAnchor);
                g.GetComponent<FireballSpellLogic>().spell = spell;
                g.GetComponent<FireballSpellLogic>().timeBeforeCast = spell.getAttributeValue(SpellAttribute.AttributeType.summonTime);
                //If the activation index was one of the three keys tied to runes, set the rune slot as the active player slot so we don't use tools as well
                player.SetActiveSlot(RuneManager.instance.GetSpellSlot(spell.spellActivationIndex));
                //Deselect all hotbar slots
                hotbarManager.DeselectAllSlots();
                return;
            case Spell.SpellType.VoidFireball:
                //Make sure anchor position is 0,0,0
                spellAnchor.localPosition = Vector3.zero;
                g = Instantiate(ItemAssets.Instance.voidFireballSpellObject, spellAnchor.position, Quaternion.identity, spellAnchor);
                g.GetComponent<FireballSpellLogic>().spell = spell;
                g.GetComponent<FireballSpellLogic>().timeBeforeCast = spell.getAttributeValue(SpellAttribute.AttributeType.summonTime);
                //If the activation index was one of the three keys tied to runes, set the rune slot as the active player slot so we don't use tools as well
                player.SetActiveSlot(RuneManager.instance.GetSpellSlot(spell.spellActivationIndex));
                //Deselect all hotbar slots
                hotbarManager.DeselectAllSlots();
                return;
        }
    }
    //Method that just switches the spell object from local to world space by unparenting
    public void SetPositionToWorld(Transform t)
    {
        t.SetParent(null, true);
    }
    //Method used to throw a spell object
    public void ThrowSpell(GameObject g, Spell spell)
    {
        Vector3 throwDir = cam.transform.forward;
        switch (spell.type) {
            
                case Spell.SpellType.Fireball:
                //Allow the spell to be affected by gravity
                g.GetComponent<Rigidbody>().isKinematic = false;
                //Enable the collider on the spell
                g.GetComponent<SphereCollider>().enabled = true;
                //The first child should be the trail component
                g.transform.GetChild(0).gameObject.SetActive(true);
                //Set the property of the shader graph to show its been thrown
                g.GetComponent<ModifiyVFXGraphProperty>().SendVFXGraphEvent("Detatch");
                g.GetComponent<Rigidbody>().AddForce(throwDir * 2500, ForceMode.Force);
                //Reset charge amount
                chargeAmount = 0;
                break;
            case Spell.SpellType.VoidFireball:
                //Allow the spell to be affected by gravity
                g.GetComponent<Rigidbody>().isKinematic = false;
                //Enable the collider on the spell
                g.GetComponent<SphereCollider>().enabled = true;
                //The first child should be the trail component
                g.transform.GetChild(0).gameObject.SetActive(true);
                //Set the property of the shader graph to show its been thrown
                g.GetComponent<ModifiyVFXGraphProperty>().SendVFXGraphEvent("Detatch");
                g.GetComponent<Rigidbody>().AddForce(throwDir * 2500, ForceMode.Force);
                //Reset charge amount
                chargeAmount = 0;
                break;
            default:
                break;
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
   

}
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
}
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


