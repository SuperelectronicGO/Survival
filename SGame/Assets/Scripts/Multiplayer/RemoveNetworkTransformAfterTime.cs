using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
public class RemoveNetworkTransformAfterTime : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(RemoveNetworkTransform());   
    }
    private IEnumerator RemoveNetworkTransform()
    {
        yield return new WaitForSecondsRealtime(1);
        Destroy(GetComponent<NetworkTransform>());
        Destroy(this);
        yield break;
    }
}
