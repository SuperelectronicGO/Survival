using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VitalsOverlay : MonoBehaviour
{
    //What percent the health needs to drop below for the UI to start taking effect
    [Range(0,1)]
    [SerializeField] private float threshholdStart = 0.3f;
    //The max alpha value the image can be
    [Range(0,1)]
    [SerializeField] private float maxValue = 0.8f;
    [SerializeField] private Image healthOverlayImage;
    [SerializeField] private Image healthbarFill;
    Color vitalColor;
    
    //Events
    private void OnEnable()
    {
        PlayerHandler._UpdateVitalsUI += UpdateVitalUI;
    }
    private void OnDisable()
    {
        PlayerHandler._UpdateVitalsUI -= UpdateVitalUI;
    }
    private void Start()
    {
        vitalColor = healthOverlayImage.color;
        vitalColor.a = 0;
    }
    private void UpdateVitalUI(float health, float maxHealth)
    {
        //Damage Overlay
        if (health / maxHealth < threshholdStart)
        {
            vitalColor.a = (1 - ((health / maxHealth)/threshholdStart))*maxValue;
            healthOverlayImage.color = vitalColor;
        }
        //Health Bar
        healthbarFill.fillAmount = health / maxHealth;
    }
}

