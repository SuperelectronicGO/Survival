using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI tooltipText;
    public RectTransform background;

    string oldQuote = string.Empty;
    public RectTransform tooltipRect;
    
    // Start is called before the first frame update
    void Start()
    {
        tooltipText.text = string.Empty;
        tooltipRect = tooltipText.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {


       
        
      
      
    }

    
}
