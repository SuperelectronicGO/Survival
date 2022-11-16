using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpToIsland : MonoBehaviour
{
    [SerializeField] private Camera mapCam;
    [SerializeField] private GameObject island;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Jump()
    {
        mapCam.transform.position = new Vector3(island.transform.position.x, mapCam.transform.position.y, island.transform.position.z-20);
    }
}
