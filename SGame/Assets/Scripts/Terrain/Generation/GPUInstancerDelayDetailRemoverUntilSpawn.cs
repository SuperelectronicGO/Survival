using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GPUInstancer;

public class GPUInstancerDelayDetailRemoverUntilSpawn : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(WaitUntilHostSpawn());
    }
    private IEnumerator WaitUntilHostSpawn()
    {
        
        yield return new WaitUntil(() => PlayerNetwork.instance.hostFinishedGenerating == true);
        GetComponent<GPUInstancerInstanceRemover>().enabled = true;
        yield return new WaitForSecondsRealtime(6);
        Destroy(this.gameObject);
        yield break;
    }
}
