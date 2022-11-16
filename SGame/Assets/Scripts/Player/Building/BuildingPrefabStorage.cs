using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPrefabStorage : MonoBehaviour
{
  
    public static BuildingPrefabStorage Instance { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }
    [NonReorderable]
    public List<buildingPrefab> buildingModels = new List<buildingPrefab>();
}
