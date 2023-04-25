using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GenerationScreen : MonoBehaviour
{
    public GameObject canvasObject;

    [SerializeField] private TextMeshProUGUI genAmountText;
    [SerializeField] private Image fillBar;

    [SerializeField] private WorldGen gen;
    public int genAmount;
   public void RefreshGenerationUI()
    {
        genAmountText.text = $"{genAmount}/53";
        float genAm = genAmount;
        fillBar.fillAmount = (float)(genAm / 53);
    }
}
