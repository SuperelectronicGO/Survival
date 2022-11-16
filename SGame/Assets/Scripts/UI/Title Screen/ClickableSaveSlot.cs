using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.SceneManagement;
public class ClickableSaveSlot : MonoBehaviour
{
    public GameObject slot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        slot.transform.localScale-= new Vector3(0.005f,0.005f,0.005f);
        if (slot.transform.localScale.x < 1)
        {
            slot.transform.localScale = new Vector3(1, 1, 1);
        }
        if (slot.transform.localScale.x > 1.1f)
        {
            slot.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
        GetComponent<Button_UI>().MouseOverFunc = () =>
        {
            slot.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
        };
        GetComponent<Button_UI>().ClickFunc = () =>
        {
            SceneManager.LoadScene("Main");
        };
    }
}
