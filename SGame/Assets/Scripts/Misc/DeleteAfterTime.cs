using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAfterTime : MonoBehaviour
{
    [SerializeField] private float seconds;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Wait());   
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSecondsRealtime(seconds);
        Destroy(this.gameObject);
        yield break;
    }
}
