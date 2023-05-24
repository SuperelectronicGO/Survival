using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLayoutAssets : MonoBehaviour
{
    public static RandomLayoutAssets instance { get; private set; }
    private void Start()
    {
        instance = this;
    }
    public RandomLayoutScriptable ColleseumLayout;
}
