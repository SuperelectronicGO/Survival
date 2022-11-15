using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    public int pageNumber = 1;
    //1 -title, 2- saves
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void toSaves()
    {
        pageNumber = 2;
    }
    public void toTitle()
    {
        pageNumber = 1;
    }
}
