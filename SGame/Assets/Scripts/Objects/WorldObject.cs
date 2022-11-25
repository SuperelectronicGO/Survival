using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObject : MonoBehaviour
{
    [SerializeField] private float health;



  
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("PlayerTool"))
        {
            Debug.Log("Hit " + name);
        }
    }
}
