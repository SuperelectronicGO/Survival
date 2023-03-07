using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class ModifiyVFXGraphProperty : MonoBehaviour
{
    public VisualEffect asset;
    // Start is called before the first frame update
    void Start()
    {
        asset = GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    
    void Update()
    {
        if (Input.GetAxis("Mouse X") != 0||Input.GetAxis("Mouse Y")!=0)
        {
            asset.SetFloat("MovingModifier", 2);


        }
        else
        {
            asset.SetFloat("MovingModifier", 1);
        }
        
    }
}
