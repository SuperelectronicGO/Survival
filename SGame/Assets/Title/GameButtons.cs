using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameButtons : MonoBehaviour
{
    private string currentGameState = "MainTitle";
    private bool settingsConfirmed;

    [SerializeField] private GameObject mainTitleObjects;
    [SerializeField] private GameObject worldGenerationObjects;
    [SerializeField] private GameObject saveSelectObjects;



    public void Update()
    {
        if (currentGameState == "MainTitle")
        {
            mainTitleObjects.SetActive(true);
            worldGenerationObjects.SetActive(false);
            saveSelectObjects.SetActive(false);
        }
        else if(currentGameState=="WorldGeneration")
        {
            mainTitleObjects.SetActive(false);
            worldGenerationObjects.SetActive(true);
            saveSelectObjects.SetActive(false);
        }
        else if (currentGameState == "SaveSelect")
        {
            mainTitleObjects.SetActive(false);
            worldGenerationObjects.SetActive(false);
            saveSelectObjects.SetActive(true);
        }

           if (transform.Find("ScreenCover").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsTag("LoadWorld"))
        {
            SceneManager.LoadScene("Main");
        }
    }


    public  void closeGame()
    {
        Application.Quit();
    }

    public void toMainTitle()
    {
       
            
       
            currentGameState = "MainTitle";
        
    }

    public void onNewSaveSelect()
    {
        currentGameState = "WorldGeneration";
    }

    public void toSaveSelect()
    {
        currentGameState = "SaveSelect";
    }

    public void confirmWorldGenerationSettings()
    {
        transform.Find("ScreenCover").GetComponent<Animator>().Play("Fade");
    }
}
