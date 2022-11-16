using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightingAsset : MonoBehaviour
{


    //How much of an effect the script has on the lights intensity
    [Range(0, 1)]
    [SerializeField] private float lightingIntensityEffect;

    //Two colors and the output
    [SerializeField] private Color colorOne;
    [SerializeField] private Color colorTwo;
    [SerializeField] private Color outputColor;
    //Bias between the two colors
    [Range(0, 1)]
    [SerializeField] private float colorBias;
    //Speed at which the lighting changes back to normal
    [Range(0,5)]
    [SerializeField] private float lightingChangeSpeed=1;
    [SerializeField] private float defaultLightIntensity;
    //Value for storing current light data
    private float lightingEffectedAmount;
    [SerializeField] private lightingTypes lightingType;
    private enum lightingTypes
    {
        None,
        Sharp,
        
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
   
    void Update()
    {
        

        if (lightingType == lightingTypes.Sharp)
        {
            if (lightingEffectedAmount < 0)
            {
                lightingEffectedAmount = 0;
            }
            if (lightingEffectedAmount > 1)
            {
                lightingEffectedAmount = 1;
            }
            lightingEffectedAmount -= lightingChangeSpeed;
            if (Random.Range(0, 1) < 0.001f&&Random.Range(0,1)<0.5f)
            {
          
                    lightingEffectedAmount += Random.Range(0.4f, 1 - lightingEffectedAmount);
                }
            
            colorBias = lightingEffectedAmount;
        }
        //Apply values
        outputColor = Color.Lerp(colorOne, colorTwo, colorBias);
        GetComponent<Light>().color = outputColor;
        GetComponent<Light>().intensity = defaultLightIntensity + colorBias*2;
    }


}
