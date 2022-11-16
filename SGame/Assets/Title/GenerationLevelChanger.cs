using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GenerationLevelChanger : MonoBehaviour
{
    public int generationLevel = 0;
    public Vector2 maxGenerationAmount = new Vector2(-2, 2);
    public string[] labels = { "Sparse", "Less", "Default", "More", "Dense" };


    


    private GameObject textLabel;
   
    // Start is called before the first frame update
    void Start()
    {
        textLabel = transform.Find("LabelAmount").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        textLabel.GetComponent<Text>().text = labels[generationLevel + Mathf.Abs(Mathf.RoundToInt(maxGenerationAmount.x))];
    }
    public void addToGeneration()
    {
        if (generationLevel < maxGenerationAmount.y)
        {
            generationLevel += 1;
        }
       
    }
    public void subtractFromGeneration()
    {
        if (generationLevel > maxGenerationAmount.x)
        {
            generationLevel -= 1;
        }

    }

}
