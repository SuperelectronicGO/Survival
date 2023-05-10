using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GPUInstancer;
public class GPUInstancerDelayDetailRemoverUntilTerrainActivation : MonoBehaviour
{
    private Transform targetTransform;
    private void OnEnable()
    {
        StartCoroutine(SetDelayDetails());
    }
    private IEnumerator SetDelayDetails()
    {
        //Enable the GPUInstancer detail remover
        targetTransform.GetComponent<GPUInstancerInstanceRemover>().enabled = true;
        //Wait for 5 seconds so things can load
        yield return new WaitForSecondsRealtime(5);
        //Destroy the detail remover colliders on the target gameObject
        Destroy(targetTransform.gameObject);
        //Destroy this component on the terrain
        Destroy(this);
        //End coroutine
        yield break;
    }
    public void SetTargetObject(Transform t)
    {
        targetTransform = t;
    }
}

