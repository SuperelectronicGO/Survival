using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRotates : MonoBehaviour
{
    public GameObject parentsnap;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
     //   float width = GetComponent<MeshFilter>().mesh.bounds.size.x*transform.localScale.x;
     //   Vector3 newpos = parentsnap.transform.position;
     //   newpos.x = newpos.x + (width / 2);
     //   transform.position = newpos;
    }

    // Update is called once per frame
    void Update()
    {
        //parentsnap.transform.LookAt(player.transform);
         //transform.RotateAround(parentsnap.transform.position,Vector3.up, parentsnap.transform.rotation.y);

     

       // transform.rotation = parentsnap.transform.rotation;
    }
}
