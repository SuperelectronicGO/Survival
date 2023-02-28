using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GPUInstancer;
public class GPUInstancerSetupOnTerrainActivation : MonoBehaviour
{
    public bool enabledFirstTime = false;
    public List<int> crossQuadCounts;
    private void OnEnable()
    {
        if (!enabledFirstTime)
        {
            WorldGen.createManagerForTerrain(GetComponent<Terrain>(), crossQuadCounts);
            enabledFirstTime = true;
        }
    }
    
}
