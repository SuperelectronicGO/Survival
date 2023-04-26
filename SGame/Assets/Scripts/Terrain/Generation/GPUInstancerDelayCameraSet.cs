using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUInstancerDelayCameraSet : MonoBehaviour
{
    public List<int> crossQuadCounts;
    private void Awake()
    {
        StartCoroutine(WaitUntilHostSpawn());
    }
    private IEnumerator WaitUntilHostSpawn()
    {
        yield return new WaitUntil(() => PlayerNetwork.instance.hostFinishedGenerating == true);
        WorldGen.createManagerForTerrain(GetComponent<Terrain>(), crossQuadCounts);
        Destroy(this);
        yield break;       
    }

}
