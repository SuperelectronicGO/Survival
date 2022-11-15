using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyLayers : MonoBehaviour
{
 
    // Update is called once per frame
    void Update()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.layer != 8 && child.transform.name == "Terrain Chunk") 
            {
                child.gameObject.layer = 8;
                if (child.GetComponent<ObjectGenerator>() == null)
                {
                    child.gameObject.AddComponent<ObjectGenerator>();
                   
                }
                
                
            }
            child.GetComponent<ObjectGenerator>().enabled = true;
            child.GetComponent<ObjectGenerator>().mapGenerator = this.GetComponent<MapGenerator>();
        }
    }
}
