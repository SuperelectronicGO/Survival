using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODitem : MonoBehaviour
{

    private GameObject ItemActivatorObject;
    private LODmanager activationScript;
    // Start is called before the first frame update
    void Start()
    {
        ItemActivatorObject = GameObject.Find("GameManager");
        activationScript = ItemActivatorObject.GetComponent<LODmanager>();
        if (activationScript.activatorItems != null)
        {
            activationScript.activatorItems.Add(new ActivatorItem { item = this.gameObject, itempos = transform.position });




        }
    }

}
