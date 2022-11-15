using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkDisplay : MonoBehaviour
{
    
    public GameObject player;

    void Update()
    {
        foreach (Transform child in transform)
        {
            
            if(Mathf.Abs(Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z), new Vector2(child.transform.position.x+250, child.transform.position.z+250))) <= 500){
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
