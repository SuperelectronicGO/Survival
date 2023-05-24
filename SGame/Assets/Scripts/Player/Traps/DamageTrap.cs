using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrap : MonoBehaviour
{
    public void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {

            PlayerHandler.instance.DamagePlayer(0.2f);
        }
    }
}
