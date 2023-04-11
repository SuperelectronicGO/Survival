using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerNetworkStart : NetworkBehaviour
{
 
    public override void OnNetworkSpawn()
    {
       if (IsOwner)
       {
           PlayerNetwork.instance.assignPlayerStartValues(this.gameObject);
           Debug.Log("Called value assign");
          
       }
    }
    private void Update()
    {
        if (IsOwner)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                this.transform.position = new Vector3(6027, 115, 5217);
            }
        }
    }

}
