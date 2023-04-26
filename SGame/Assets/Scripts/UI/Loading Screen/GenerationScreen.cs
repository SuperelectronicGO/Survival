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
    [SerializeField] private GameObject loadingBarParent;
    [SerializeField] private GameObject waitForHostText;

    [SerializeField] private WorldGen gen;
    public int genAmount;
   public void RefreshGenerationUI()
    {
        genAmountText.text = $"{genAmount}/53";
        float genAm = genAmount;
        fillBar.fillAmount = (float)(genAm / 53);
    }
    public void SetWaitingForHost()
    {
        loadingBarParent.SetActive(false);
        waitForHostText.SetActive(true);
    }
}
