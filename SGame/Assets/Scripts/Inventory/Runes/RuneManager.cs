using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneManager : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private Canvas canvas;
    [SerializeField] private List<InventorySlot> slots;
    public static RuneManager instance;
    public Spell[] currentRuneSlotSpells;
    public bool[] slotHasSpell;
    // Start is called before the first frame update
    void Start()
    {
        currentRuneSlotSpells = new Spell[3];
        slotHasSpell = new bool[3];
        instance = this;
        StartCoroutine(checkSlotsForSpells());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void refreshRuneSlotValues()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].updateSlotValues();
        }
    }
    public IEnumerator checkSlotsForSpells()
    {
        for(int i=0; i<slots.Count; i++)
        {
            //Check if slot is dirtied
            if(slots[i].dirtied){

                //Check if we are adding a spell
                if (!slotHasSpell[i]&&slots[i].heldItem.hasAttribute(ItemAttribute.AttributeName.AllowsSpell))
                {
                    Spell spellToAdd =slots[i].heldItem.spell.Clone();
                    currentRuneSlotSpells[i] = spellToAdd;
                    spellToAdd.spellActivationIndex = i;
                    SpellManager.instance.currentSpells.Add(spellToAdd);
                    slotHasSpell[i] = true;
                }
                //Check if we are removing a spell
                else if (slotHasSpell[i]&&!slots[i].heldItem.hasAttribute(ItemAttribute.AttributeName.AllowsSpell))
                {
                    slotHasSpell[i] = false;
                    SpellManager.instance.currentSpells.Remove(currentRuneSlotSpells[i]);
                }
                //Check if we are swapping a spell
                else
                {
                    SpellManager.instance.currentSpells.Remove(currentRuneSlotSpells[i]);
                    Spell spellToAdd = slots[i].heldItem.spell.Clone();
                    currentRuneSlotSpells[i] = spellToAdd;
                    spellToAdd.spellActivationIndex = i;
                    SpellManager.instance.currentSpells.Add(spellToAdd);
                }
             slots[i].dirtied = false;
            }
        }
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(checkSlotsForSpells());
    }
}
