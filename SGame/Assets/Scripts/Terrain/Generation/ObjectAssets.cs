using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAssets : MonoBehaviour
{
    public static ObjectAssets instance { get; private set; }
    private void Start()
    {
        instance = this;
    }
    public GameObject LeafyHemlock;
    public GameObject LeafyPine;
    public GameObject ScragglyHemock;
    public GameObject ScragglyPine;
    public GameObject MossyBirch;
    public GameObject DryLeafyHemlock;
    public GameObject DryLeafyPine;
    public GameObject DryScragglyHemlock;
    public GameObject DryScragglyPine;
    public GameObject DeadTree;
    public GameObject RottenStump;
    public GameObject RockOne;
    public GameObject RockTwo;

}
