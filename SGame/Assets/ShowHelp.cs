using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
public class ShowHelp : MonoBehaviour
{
    [SerializeField] private GameObject helpScreen;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Button_UI>().MouseOverFunc = () =>
        {
            if (!helpScreen.activeInHierarchy)
            {
                helpScreen.SetActive(true);
            }
        };
        GetComponent<Button_UI>().MouseOutOnceFunc = () =>
        {
            helpScreen.SetActive(false);
        };
    }
}
