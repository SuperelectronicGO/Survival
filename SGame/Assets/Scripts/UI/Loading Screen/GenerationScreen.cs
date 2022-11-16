using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GenerationScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI genStageText;
    [SerializeField] private TextMeshProUGUI genAmountText;

    [SerializeField] private WorldGen gen;
    public string currentGenStage;
    public string genAmount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
   public void refreshGenerationUI()
    {
        genStageText.text = currentGenStage;
        genAmountText.text = genAmount;
    }
}
