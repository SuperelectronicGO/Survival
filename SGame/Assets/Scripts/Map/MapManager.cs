using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public bool viewingMap = false;
    public Camera mapCamera;
    
    public Vector3 camMove;

    public UIManager uiManager;

    public List<GameObject> islandList = new List<GameObject>();
    public List<mapPanel> islandPanels = new List<mapPanel>();
    private string selectedIslandString;
    [SerializeField] private GameObject selectedIsland;
    private bool inRangeOfIsland = false;
    private GameObject activePanel;

    // Start is called before the first frame update
    void Start()
    {
        mapCamera.gameObject.SetActive(false);
        viewingMap = false;
        camMove = new Vector3();
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
       
        //Viewing Map or not
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (viewingMap)
            {
                viewingMap = false;
            }
            else
            {
                viewingMap = true;
            }
        }
        if (viewingMap)
        {
            uiManager.mapOpen = true;
        }
        else
        {
            uiManager.mapOpen = false;
        }
        if (viewingMap)
        {
            mapCamera.gameObject.SetActive(true);

            //Camera movement
            {
               
                if (Input.GetKey(KeyCode.W))
                {
                    camMove.z = camMove.z+0.06f;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    camMove.z = camMove.z -0.06f;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    camMove.x = camMove.x-0.06f;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    camMove.x =camMove.x +0.06f;
                }

             
                    if (!(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))&&camMove.x>0)
                    {
                    camMove.x -= 0.08f;
                  
                }
               
                    if (!(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))&&camMove.z>0)
                    {

                    camMove.z -= 0.08f;
                }
                if (!(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))&&camMove.x<0)
                {
                    camMove.x += 0.08f;

                }

                if (!(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))&&camMove.z<0)
                {

                    camMove.z += 0.08f;
                }


                if (!(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)))
                {
                    if (camMove.z < 0.1f && camMove.z > -0.1f)
                    {
                        camMove.z = 0;
                    }
                }

                if (!(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))) { 
                    if (camMove.x < 0.1f && camMove.x > -0.1f)
                    {
                        camMove.x = 0;
                    }
                }
                //Constrain vector to 3
                camMove = Vector3.ClampMagnitude(camMove, 3);
                mapCamera.transform.position += camMove;
               
                
            }
            //Island raycasts
            Ray ray = mapCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit, 10000f))
            {
               

                if (hit.transform.CompareTag("Island"))
                {
                    selectedIslandString = hit.transform.name;
                    if (Input.GetMouseButtonDown(0))
                    {
                        selectedIsland = hit.transform.gameObject;
                    }
                }
                else
                {
                    selectedIslandString = "";
                }
            }
            else
            {
                selectedIslandString = "";
            }
           foreach(GameObject obj in islandList)
            {
                if (obj.name == selectedIslandString)
                {
                    obj.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                }
                else{
                    obj.transform.localScale = new Vector3(1, 1, 1);
                }
            }
            if (Input.GetMouseButtonDown(0) && selectedIslandString != "")
            {
              
                foreach (mapPanel panel in islandPanels)
                {
                    
                    if (panel.islandName == selectedIslandString)
                    {
                        panel.panel.SetActive(true);
                        activePanel = panel.panel;
                       
                    }
                    else
                    {
                        panel.panel.SetActive(false);
                       
                    }
                   
                }
            }
        }
        else
        {
            mapCamera.gameObject.SetActive(false);
        }
     
    
        
    }
    
    private void checkIslandRange()
    {
         
            Vector3 camPos = mapCamera.transform.position;
    camPos.y = 0;
            Vector3 islandPos = selectedIsland.transform.position;
    islandPos.y = 0;
            float distAmnt = Vector3.Distance(camPos, islandPos);
            if (distAmnt > 0 && distAmnt< 60)
            {
                //Do nothing - we are within range
                inRangeOfIsland = true;
            }
            else
{
    //out of range
    inRangeOfIsland = false;
}
        
    }
   
    
}



[System.Serializable]
public class mapPanel
{
    public GameObject panel;
    public string islandName;
}

