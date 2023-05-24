using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingAssets : MonoBehaviour
{
    public static BuildingAssets instance { get; private set; }
    void Start()
    {
        if (instance != null) throw new System.Exception("A BuildingAssets instance already exists. Do you have multiple active on accident?");
        instance = this;
    }
    [Header("Index 0 - Broken Colleseum")]
    public GameObject brokenColleseumObject;
}
