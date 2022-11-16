using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.UI;
public class DisplayTooltip : MonoBehaviour
{
    [SerializeField] private Text descriptionText;
    [SerializeField] private string description;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Button_UI>().MouseOverOnceFunc = () =>
        {
            descriptionText.text = description;

        };
        GetComponent<Button_UI>().MouseOutOnceFunc = () =>
        {
            descriptionText.text = "";
        };
    }
}
