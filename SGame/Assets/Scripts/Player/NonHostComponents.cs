using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonHostComponents : MonoBehaviour
{
    public GameObject currentSpellGraphics = null;
    public void DestroyGraphics()
    {
        Destroy(currentSpellGraphics);
        currentSpellGraphics = null;
    }
}
