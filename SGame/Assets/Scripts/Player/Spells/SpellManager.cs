using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    [SerializeField] private KeyCode spellKeyOne, spellKeyTwo, spellKeyThree;
    public static SpellManager instance;
    [SerializeField] private GameObject cam;
    [SerializeField] private AnimationCurve chargeGraph;
    public float currentChargeSpeedDivisor, currentRetractSpeed, maxCrosshairExpansion;
    [NonReorderable]
    public List<Spell> currentSpells = new List<Spell>();
    //Data
    private float chargeAmount = 0;
    private CrosshairManager crosshairManager;
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        
         
    }

    // Update is called once per frame
    void Update()
    {
        //Handle charging spells
        if (PlayerHandler.instance.ableToMouseLook && Input.GetMouseButton(1) && !(chargeAmount > 99.9f))
        {
            OnSpellChargeChange(true);
        }else if (chargeAmount > 0&&!Input.GetMouseButton(1))
        {
            OnSpellChargeChange(false);
        }
        //Check for spell activation keypresses

    }


    public void equipSpell(Spell spell)
    {

    }
    public void SetPositionToWorld(Transform t)
    {
        t.SetParent(null, true);
    }
    public void ThrowSpell(GameObject g)
    {
        
        g.GetComponent<Rigidbody>().isKinematic = false;
        g.GetComponent<SphereCollider>().enabled = true;
        Vector3 throwDir = cam.transform.forward;
        //throwDir.y = -throwDir.y;
        g.GetComponent<Rigidbody>().AddForce(throwDir * 2500, ForceMode.Force);

    }
    private void OnSpellChargeChange(bool positive)
    {
        if (positive)
        {
            float amount = chargeGraph.Evaluate(chargeAmount / 100);
            chargeAmount += amount*currentChargeSpeedDivisor;
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
        Fireball
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
    chargeTime,
    cooldownTime,
    damage,

     }
    public AttributeType attribute;
    public float value;
}


