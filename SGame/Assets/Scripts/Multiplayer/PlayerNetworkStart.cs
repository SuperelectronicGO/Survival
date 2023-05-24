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
           PlayerNetwork.instance.AssignPlayerStartValues(this.gameObject);
           Debug.Log("Called value assign");
        }
  //      Destroy(this);

    }
  

}
