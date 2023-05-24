using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestSlot : ISInterface
{
    private int storageCorrespondIndex;
    public override void LeftClickFunction()
    {
        base.LeftClickFunction();
        StorageManager.instance.SetCurrentStorageValue(storageCorrespondIndex, heldItem);
    }
    public override void RightClickFunction()
    {
        base.RightClickFunction();
        StorageManager.instance.SetCurrentStorageValue(storageCorrespondIndex, heldItem);
    }
    public void SetStorageIndex(int index)
    {
        storageCorrespondIndex = index;
    }
}
