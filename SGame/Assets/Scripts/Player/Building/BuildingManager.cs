using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System;

public class BuildingManager : MonoBehaviour
{
    
    public List<GameObject> currentNodes = new List<GameObject>();
    [SerializeField] private GameObject player;
    public Camera playerCam;
    
    public bool buildingEnabled;
   private BuildingPrefabStorage buildPrefabStorage;

    private GameObject selectedObj;
    public GameObject buildingPreview;
   


    //Private variables - set from the prefabs storage component
    private bool flipModel;
    private float buildingRotAmount = 0;
    private bool usesNodes;
    private bool swappedToPosition=false;

    private int maxRotation;
    Vector3 oldPos;
    // Start is called before the first frame update
    void Start()
    {
    
        
        buildPrefabStorage = GameObject.Find("GameManager").GetComponent<BuildingPrefabStorage>();
    }

    // Update is called once per frame
    void Update()
    {
        //Rotate Preview
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            buildingRotAmount -= 1;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            buildingRotAmount += 1;
        }
       
        
        //Run function to set all values
       
       



    
        //Make sure the held item allows building
       

        
        if (buildingPreview!=null) {

            if (!buildingPreview.activeInHierarchy)
            {
                buildingPreview.SetActive(true);
            }
            //Define raycast variables
            RaycastHit previewHit;
            Ray previewRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Remove (Delete) preview if building not enabled
            if (!buildingEnabled)
            {
                Destroy(buildingPreview.gameObject);
            }








            GameObject selectedNode = null;

            
             if (swappedToPosition)
        {
            //If using nodes, reset preview position to avoid it disappearing.
            if (usesNodes)
            {
                RaycastHit resetPreviewPos;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out resetPreviewPos, 25f))
                {
                    buildingPreview.transform.position = resetPreviewPos.point;
                }
            }
            swappedToPosition = false;
        }
            
            //Raycast for building preview
            if (Physics.Raycast(previewRay, out previewHit, 25f))
            {





                //Find closest node if nodes are enabled
                
                float closestAmount = 5;
                if (usesNodes)
                {
                    
                    foreach (GameObject obj in currentNodes)
                    {

                        if (Vector3.Distance(previewHit.point, obj.transform.position) < closestAmount)
                        {
                            closestAmount = Vector3.Distance(previewHit.point, obj.transform.position);
                            selectedNode = obj;
                        }



                  }
                }


                


                //Set positions with nodes
                if (usesNodes && selectedNode != null)
                {
                    


                    //Calculate bounds of wall on the x to find the offset (radius) of the circle around the node. Uses node parents collider (as previews don't have one)
                    float width = selectedNode.transform.parent.transform.Find("Graphics").GetComponent<MeshFilter>().mesh.bounds.size.x*selectedNode.transform.parent.transform.localScale.x;
                   
               

                 //Raycast from the center of the preview position to find the correct Y value for that terrain point
                 RaycastHit hit;
                 Vector3 prevPos = RandomCircle(selectedNode.transform.position, width / 2, buildingPreview.transform.eulerAngles.y - 90);
                 if (Physics.Raycast(new Vector3(prevPos.x, buildingPreview.transform.position.y + 4, prevPos.z), -Vector3.up, out hit, 25f))
                 {
                       
                        prevPos.y = hit.point.y;
                      
                      


                        //Define a new Quaternion from the normals of the raycast hit, and set its Y rotation to zero.
                        Quaternion rotQuat = new Quaternion(Quaternion.FromToRotation(transform.up, hit.normal).x, Quaternion.FromToRotation(transform.up, hit.normal).y, Quaternion.FromToRotation(transform.up, hit.normal).z, Quaternion.FromToRotation(transform.up, hit.normal).w);
                        rotQuat.y = 0;



                        //Apply rotation
                        
                       
                            buildingPreview.transform.rotation = rotQuat * Quaternion.Euler(0, Mathf.Atan2(previewHit.point.x - selectedNode.transform.position.x, previewHit.point.z - selectedNode.transform.position.z) * (180 / Mathf.PI) + 90, 0);
                        
                        //Re-create the position to use the updated rotation, and not the last frames rotation. Not doing this causes the preview to shake when moving.
                      prevPos = RandomCircle(selectedNode.transform.position, width / 2, buildingPreview.transform.eulerAngles.y - 90);
                        prevPos.y = hit.point.y;
                        //Apply position
                        buildingPreview.transform.position = prevPos;
                       

                        //Alternate flipping for each segment if "flipmodel" is checked.
                        if (flipModel)
                        {


                            Vector3 parentScale = buildingPreview.transform.Find("Graphics").localScale;
                                parentScale.x = -selectedNode.transform.parent.Find("Graphics").localScale.x;
                           
                                buildingPreview.transform.Find("Graphics").localScale = parentScale;
                            

                         
                            
                          
                        }
                    }

                 
                }
                else {
                    //Set position without nodes
                    //Create rotation (same as above)
                   
                        Quaternion rotQuat = new Quaternion(Quaternion.FromToRotation(transform.up, previewHit.normal).x, Quaternion.FromToRotation(transform.up, previewHit.normal).y, Quaternion.FromToRotation(transform.up, previewHit.normal).z, Quaternion.FromToRotation(transform.up, previewHit.normal).w);
                        rotQuat.y = 0;
                        //Apply values
                        buildingPreview.transform.rotation = rotQuat * Quaternion.Euler(0, buildingRotAmount, 0);
                        buildingPreview.transform.position = new Vector3(previewHit.point.x, previewHit.point.y, previewHit.point.z);
                    
                   
                }



            }
            else
            {
                buildingPreview.SetActive(false);
            }



            //Actual Placing

            if (Input.GetMouseButtonDown(0) && buildingEnabled && ((buildingPreview.transform.eulerAngles.z < maxRotation || buildingPreview.transform.eulerAngles.z > (360 - maxRotation)) && (buildingPreview.transform.eulerAngles.x < maxRotation || buildingPreview.transform.eulerAngles.x > (360 - maxRotation)))&&buildingPreview.activeInHierarchy)
            {

                //Instantiate selected building at the previews position, and with the previews rotation.
                   GameObject obj =  Instantiate(selectedObj, buildingPreview.transform.position, buildingPreview.transform.rotation);
                if (flipModel)
                {
                    Transform t = obj.transform.Find("Graphics").transform;
                    t.localScale = buildingPreview.transform.Find("Graphics").transform.localScale;
                    
                }
            }
        }
    }

    //Function for calculating a point on a circle with 'radius' radius, 'angle' angle of point (0-360 if equation timesed by 180/PI), and center 'center'.
    Vector3 RandomCircle(Vector3 center, float radius, float angle)
    {
        
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        pos.z = center.z + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        pos.y = center.y;
        return pos;
    }

    //Function for setting build managers values based on the selected item
    public void selectPrefab(Item item)
    {
        //Object that will be selected
        GameObject selectedObject;

        //Iterate through list
        foreach(buildingPrefab pre in buildPrefabStorage.buildingModels)
        {

            if(pre.type == item.itemType)
            {

                selectedObject = pre.prefabModel;
                selectedObj = selectedObject;

                if (buildingPreview == null || buildingPreview.name !=pre.type.ToString()+"(Clone)")
                {
                    if (buildingPreview == null)
                    {
                        //dont destroy
                    }
                    else
                    {
                        Destroy(buildingPreview);

                        buildingRotAmount = 0;
                    }
                    
                    GameObject objToCreate = pre.prefabPreviewModel;
                    objToCreate.name = pre.type.ToString();
                   buildingPreview = Instantiate(objToCreate, new Vector3(0, 0, 0), Quaternion.identity, player.transform);
                    flipModel = pre.flipModel;
                    usesNodes = pre.snapToSimilar;
                    swappedToPosition = true;
                    maxRotation = buildingPreview.GetComponent<SetPreviewMaterial>().maxRotationValue;

                }

                return;
            }
        }

    }
    
}

//Class for "building prefab storage'
[Serializable]
public class buildingPrefab
{
    public Item.ItemType type;
    public GameObject prefabModel;
    public GameObject prefabPreviewModel;
    public bool snapToSimilar;
    public bool flipModel;

}