using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPreviewMaterial : MonoBehaviour
{
    public int maxRotationValue = 0;
    [SerializeField] private Material allowedMaterial;
    [SerializeField] private Material bannedMaterial;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((this.transform.eulerAngles.z < maxRotationValue || this.transform.eulerAngles.z > 360-maxRotationValue)&&(this.transform.eulerAngles.x<maxRotationValue||this.transform.eulerAngles.x>360-maxRotationValue))
        {
            Material[] mats = GetComponent<Renderer>().materials;
           for (int i=0; i<GetComponent<Renderer>().materials.Length; i++)
            {
                mats[i] = allowedMaterial;
            }
            GetComponent<Renderer>().materials = mats;


        }
        else
        {
            Material[] mats = GetComponent<Renderer>().materials;
            for (int j = 0; j < GetComponent<Renderer>().materials.Length; j++)
            {
                mats[j] = bannedMaterial;
            }
            GetComponent<Renderer>().materials = mats;
        }

      
    }
}
