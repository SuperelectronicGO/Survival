using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGeneration : MonoBehaviour
{
    
    public int terrainWidth;
    public int terrainLength;
    
  


    [SerializeField] private int testCounter;
    public int maxAmnt = 250;
  
    


    public float[,] noiseMap;
    public float[,] heatMap;

    [Header("Trees")]
    [SerializeField] private GameObject spruceTree;
    [SerializeField] private GameObject beechTree;


    [Header("Small Items")]
    [SerializeField] private GameObject fieldstoneRock;
    [SerializeField] private GameObject oakStick;
    [SerializeField] private GameObject oakTwig;

    private GameObject[] smallerObjects = { null, null, null};
    // Start is called before the first frame update
    void Start()
    {
        smallerObjects[0] = fieldstoneRock;
        smallerObjects[1] = oakStick;
        smallerObjects[2] = oakTwig;

        placeObjects();
       

    }

    // Update is called once per frame
    void Update()
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        for(int i=0; i<5; i++)
        {
            RaycastHit hit;
            if (testCounter <= 0)
            {
                Vector3 place = new Vector3(Random.Range(0, terrainLength), transform.position.y, Random.Range(0, terrainWidth));
                int x = Mathf.RoundToInt(place.x);
                int z = Mathf.RoundToInt(place.z);
                if(Physics.Raycast(new Vector3(place.x + this.transform.position.x, this.transform.position.y, place.z + this.transform.position.z), -Vector3.up, out hit))
                {
                    Debug.Log(new Vector3(this.transform.position.x + x, hit.point.y + 2, this.transform.position.z + z) +", "+ hit.transform.tag);
                    if (hit.transform.CompareTag("Terrain"))
                    {

                        GameObject objectToPlace = null;
                        objectToPlace = smallerObjects[Random.Range(0, smallerObjects.Length)];
                        Debug.Log(new Vector3(this.transform.position.x + x, hit.point.y + 2, this.transform.position.z + z) + ", " + objectToPlace.name);
                        Instantiate(objectToPlace, new Vector3(this.transform.position.x + x, hit.point.y+2, this.transform.position.z + z), Quaternion.identity);
                        testCounter++;
                    }
                    }
            }
        }



        //Spawn Large Objects
        for (int i = 0; i < 5; i++)
        {
            RaycastHit hit;



            //Physics.Raycast(new Vector3(transform.position.x + Random.Range(-terrainLength, terrainLength), transform.position.y, transform.position.z + Random.Range(-terrainWidth, terrainWidth)), -Vector3.up, out hit)
            if (testCounter < maxAmnt&&testCounter>0)
            {
                Vector3 place = new Vector3(Random.Range(0, terrainLength), transform.position.y, Random.Range(0, terrainWidth));
                int x = Mathf.RoundToInt(place.x);
                int z = Mathf.RoundToInt(place.z);


                if (noiseMap[x, z] > 0.6f)
                {
                    if (Physics.Raycast(new Vector3(place.x + this.transform.position.x, this.transform.position.y, place.z + this.transform.position.z), -Vector3.up, out hit))
                    {
                        float randomValue = Random.Range(0, 100);
                        if (noiseMap[x, z] * 100 > randomValue)
                        {
                            if (hit.transform.CompareTag("Terrain"))
                            {
                                GameObject objectToPlace=null;
                                if (heatMap[x, z] > 0.5)
                                {
                                    objectToPlace = spruceTree;
                                }
                                else
                                {
                                    objectToPlace = beechTree;
                                }
                                Instantiate(objectToPlace, new Vector3(this.transform.position.x + x, hit.point.y, this.transform.position.z + z), Quaternion.identity);
                                testCounter++;

                            }
                        }

                    }
                }

            }
        }

       
      

    }
    void placeObjects()
    {
       
      
    }
    
}
