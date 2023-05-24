using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneSlot : ISInterface
{
   public override void LeftClickMouseFull()
    {
        if (!inventory.mouseItem.hasAttribute(ItemAttribute.AttributeName.AllowsSpell))
        {
            return;
        }
        base.LeftClickMouseFull();
    }
}
