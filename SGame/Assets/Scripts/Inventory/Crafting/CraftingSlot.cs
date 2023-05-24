using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSlot : ISInterface
{
    public override void LeftClickFunction()
    {
        base.LeftClickFunction();
        inventory.cManager.analyzeItemList();
    }
    public override void RightClickFunction()
    {
        base.RightClickFunction();
        inventory.cManager.analyzeItemList();
    }
}
