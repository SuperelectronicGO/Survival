using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGenerator : MonoBehaviour
{
    public GameObject objectParent;
    public float testPos = 0.5f;
    public bool currentIslandBeingGenerated;
    [SerializeField] private Vector2 minimumCoordinates;
    [SerializeField] private Vector2 gridSize;
    [Header("Objects")]
    [NonReorderable]
    [SerializeField] private List<GeneratableObject> islandObjects = new List<GeneratableObject>();
    [NonReorderable]
    [SerializeField] private List<GeneratableNoiseObjects> islandNoiseObjects = new List<GeneratableNoiseObjects>();
    /* Private Variables
     * generatingIndex controls which object is being generated (equal to GeneratableObject.orderInGeneration).
     * currentGeneratingObject controls which object is being generated (duh).
     * amountPlaced determines when we call checkForNextGeneration and move onto the next object.
     * finishedGeneration + finishedSmallGeneration is self explaining.
     */
    private int generatingIndex = 0;
    private GeneratableObject currentGeneratingObject;
    private GeneratableNoiseObjects currentGeneratingNoiseObject;
    private int amountPlaced;
    private bool finishedSmallGeneration = false;
    private bool finishedGeneration = false;
    public bool finishedBatching = false;
    public bool startedGenertation=false;
    public float[,] noiseMap;
    public float[,] heatMap;

   
    // Start is called before the first frame update
    void Start()
    {
        checkForNextGeneration();
         

    }

    // Update is called once per frame
    void Update()
    {
        if (startedGenertation)
        {
            if (finishedSmallGeneration && generatingIndex == -1)
            {
                generatingIndex = 0;
                checkForNextGeneration();
            }
            if (!finishedGeneration)
            {
                if (!finishedSmallGeneration)
                {

                    //Define random amount of objects to spawn
                    if (amountPlaced == 0)
                    {
                        currentGeneratingObject.objectAmount.z = Random.Range(Mathf.RoundToInt(currentGeneratingObject.objectAmount.x), Mathf.RoundToInt(currentGeneratingObject.objectAmount.y));
                    }

                    //Iteration
                    for (int i = 0; i < currentGeneratingObject.amountPerPass; i++)
                    {
                        //Generate random placement value

                        Vector3 objectPlacePosition = new Vector3(Random.Range(0, gridSize.x), 500f, Random.Range(0, gridSize.y));
                        Vector3 noisePosition = objectPlacePosition;
                        objectPlacePosition.x += minimumCoordinates.x + transform.parent.transform.position.x;
                        objectPlacePosition.z += minimumCoordinates.y + transform.parent.transform.position.z;





                        //Placing
                        RaycastHit hit;
                        if (Physics.Raycast(objectPlacePosition, -Vector3.up, out hit, 500f))
                        {
                            Vector3 newHitPosition = hit.point;
                            Quaternion rotQuat = new Quaternion(Quaternion.FromToRotation(transform.up, hit.normal).x, Quaternion.FromToRotation(transform.up, hit.normal).y, Quaternion.FromToRotation(transform.up, hit.normal).z, Quaternion.FromToRotation(transform.up, hit.normal).w);

                            newHitPosition.y -= Random.Range(0, currentGeneratingObject.maxYOffset);
                            GameObject obj = Instantiate(currentGeneratingObject.prefab, newHitPosition, Quaternion.identity);
                            float newScale = Random.Range(currentGeneratingObject.size.x, currentGeneratingObject.size.y);
                            obj.transform.localScale = new Vector3(newScale, newScale, newScale);
                            if (!currentGeneratingObject.useDefaultMeterials)
                            {
                                Material newMaterial = currentGeneratingObject.materials[Random.Range(0, currentGeneratingObject.materials.Length)];
                                obj.GetComponent<Renderer>().material = newMaterial;
                            }
                            obj.transform.parent = currentGeneratingObject.parentGameobject;
                            amountPlaced += 1;
                        }

                    }
                    //Check if enough is placed
                    if (amountPlaced >= Mathf.RoundToInt(currentGeneratingObject.objectAmount.z))
                    {
                        generatingIndex += 1;
                        amountPlaced = 0;
                        checkForNextGeneration();
                    }
                }
                else
                {
                    //Define random amount of objects to spawn
                    if (amountPlaced == 0)
                    {

                        currentGeneratingNoiseObject.objectAmount.z = Random.Range(Mathf.RoundToInt(currentGeneratingNoiseObject.objectAmount.x), Mathf.RoundToInt(currentGeneratingNoiseObject.objectAmount.y));
                    }








                    //Iteration
                    for (int i = 0; i < currentGeneratingNoiseObject.amountPerPass; i++)
                    {
                        //Generate random placement value

                        Vector3 objectPlacePosition = new Vector3(Random.Range(0, gridSize.x), 500f, Random.Range(0, gridSize.y));
                        Vector3 noisePosition = objectPlacePosition;
                        objectPlacePosition.x += minimumCoordinates.x;
                        objectPlacePosition.z += minimumCoordinates.y;

                        //Make sure only a few trees generate outside of forests, and gradually thin the edge of forests.
                        int randomChance = Random.Range(0, 1000);
                        //.65
                        if (noiseMap[Mathf.RoundToInt(noisePosition.x), Mathf.RoundToInt(noisePosition.z)] > 0.65f)
                        {
                            int random2 = Random.Range(0, 100);
                            if (110 - noiseMap[Mathf.RoundToInt(noisePosition.x), Mathf.RoundToInt(noisePosition.z)] * 120 > random2)
                            {
                                randomChance = 1000;
                            }
                            else
                            {
                                randomChance = 0;
                            }

                        }



                        if (randomChance > 990)
                        {

                            //Placing
                            RaycastHit hit;
                            if (Physics.Raycast(objectPlacePosition, -Vector3.up, out hit, 500f))
                            {
                                Vector3 newHitPosition = hit.point;
                                Quaternion rotQuat = new Quaternion(Quaternion.FromToRotation(transform.up, hit.normal).x, Quaternion.FromToRotation(transform.up, hit.normal).y, Quaternion.FromToRotation(transform.up, hit.normal).z, Quaternion.FromToRotation(transform.up, hit.normal).w);

                                newHitPosition.y -= Random.Range(0, currentGeneratingNoiseObject.maxYOffset);


                                GameObject placePrefab = null;




                                float mapPosition = heatMap[Mathf.RoundToInt(noisePosition.x), Mathf.RoundToInt(noisePosition.z)];
                                //Selection based upon heatmap

                                //Find the object closest to the heatmap higher than the value, and the object closest to the heatmap lower than the value
                                float minValue = 0;
                                float maxValue = 1;
                                GameObject minObject = null;
                                GameObject maxObject = null;
                                //For failsafe
                                GameObject failsafeMinObject = null;
                                GameObject failsafeMaxObject = null;
                                //Min objects max to use in calculation
                                float minObjectMax = 0;
                                //For clumps
                                float randomAmntForClumpSelection = Random.value;
                                GameObject clumpObj = null;
                                foreach (NoiseObjects noiseObj in currentGeneratingNoiseObject.prefabs)
                                {
                                    if (noiseObj.objectRange.x < randomAmntForClumpSelection && noiseObj.objectRange.y > randomAmntForClumpSelection)
                                    {
                                        clumpObj = noiseObj.prefab;
                                    }
                                    //Set failsafes
                                    if (noiseObj.objectRange.x == 0)
                                    {
                                        failsafeMinObject = noiseObj.prefab;
                                    }
                                    if (noiseObj.objectRange.y == 1)
                                    {
                                        failsafeMaxObject = noiseObj.prefab;
                                    }
                                    //Find median of objects range
                                    float midRange = (noiseObj.objectRange.x + noiseObj.objectRange.y) / 2;
                                    //Determine if is smaller or larger
                                    if (midRange >= mapPosition)
                                    {
                                        //Is mid range smaller than the current max range
                                        if (midRange < maxValue)
                                        {
                                            maxValue = midRange;
                                            maxObject = noiseObj.prefab;
                                        }

                                    }
                                    else
                                    {
                                        //Is mid range smaller than the current min range
                                        if (midRange >= minValue)
                                        {
                                            minValue = midRange;
                                            //minObjectMax = noiseObj.objectRange.y;
                                            minObjectMax = midRange;
                                            minObject = noiseObj.prefab;
                                        }
                                    }
                                }
                                //Make sure no null values
                                if (minValue == 0)
                                {
                                    minObject = failsafeMinObject;
                                }
                                if (maxValue == 1)
                                {
                                    maxObject = failsafeMaxObject;
                                }




                                //Find distance between objects to use for max in random value
                                int distanceBetween = Mathf.RoundToInt((maxValue - minValue) * 100);
                                //Calcluate the amount for the middle object - I.E if the mapPosition is .45 and the middle object max is .4, then there is a .05 result. If the range is between 0 and 0.2, there is a 25% chance of it being that object.
                                float minObjectAmnt = mapPosition - minObjectMax;
                                //Calculate a random number within the range
                                int randomObject = Random.Range(0, distanceBetween);
                                //Check if the min value is within range of the map random number
                                if (randomObject <= minObjectAmnt * 100)
                                {
                                    placePrefab = minObject;
                                }
                                else
                                {
                                    placePrefab = maxObject;
                                }





                                //   Debug.Log("Min object and value: " + minObject.name + ", " + minValue + " Max object and value: " + maxObject.name + ", " + maxValue + " Failsafes: " + failsafeMinObject.name + ", " + failsafeMaxObject.name + "Distance: "+distanceBetween);
                                //  Debug.Log("Min Object Amnt:" + minObjectAmnt + " random object: " + randomObject + " Value and prefab: " + (randomObject <= minObjectAmnt * 100) + ", " + placePrefab.name + ", " +mapPosition);
                                //Randomly make a diff tree to avoid heatmap clumping
                                int randomAmntForClumps = Random.Range(0, 100);
                                if (randomAmntForClumps < 20)
                                {
                                    placePrefab = clumpObj;

                                }





                                GameObject obj = Instantiate(placePrefab, newHitPosition, Quaternion.identity);
                                float newScale = Random.Range(currentGeneratingNoiseObject.size.x, currentGeneratingNoiseObject.size.y);
                                obj.transform.localScale = new Vector3(newScale, newScale, newScale);

                                obj.transform.parent = objectParent.transform;
                                amountPlaced += 1;
                            }
                        }
                    }






                    //Check if enough is placed
                    if (amountPlaced >= Mathf.RoundToInt(currentGeneratingNoiseObject.objectAmount.z))
                    {
                        generatingIndex += 1;
                        amountPlaced = 0;
                        checkForNextGeneration();
                    }
                }


                if (finishedGeneration && !finishedBatching)
                {
                    StaticBatchingUtility.Combine(objectParent);
                    finishedBatching = true;
                    Debug.Log("Batched");
                }
            }
        }
        //Large forests
/*
 *  Vector3 objectPlacePosition = new Vector3(Random.Range(0, gridSize.x), 500f, Random.Range(0, gridSize.y));
        Vector3 noisePosition = objectPlacePosition;
        objectPlacePosition.x += minimumCoordinates.x + transform.parent.transform.position.x;
        objectPlacePosition.z += minimumCoordinates.y + transform.parent.transform.position.z;
*/



}

private void checkForNextGeneration()
{
        if (!finishedSmallGeneration) {
            foreach (GeneratableObject obj in islandObjects)
            {
                if (obj.orderInGeneration == generatingIndex)
                {
                    currentGeneratingObject = obj;
                    return;
                }
            }
        }
        else
        {
            foreach(GeneratableNoiseObjects obj in islandNoiseObjects)
            {
                if (obj.orderInGeneration == generatingIndex)
                {
                    currentGeneratingNoiseObject = obj;
                
                    return;
                }
            }
        }
        if (finishedSmallGeneration)
        {
            finishedGeneration = true;
        }
        else
        {
            generatingIndex = -1;
        }
        finishedSmallGeneration = true;

        


}
}

[System.Serializable]
public class GeneratableObject
{
[Header("Min, Max, Output")]
public Vector3 objectAmount;
[Header("")]
public int amountPerPass;
public GameObject prefab;

public bool useDefaultMeterials;
public Material[] materials;
public Vector2 size;
public float maxYOffset;
public int orderInGeneration;
public Transform parentGameobject;

};
[System.Serializable]
public class GeneratableNoiseObjects
{
[Header("Min, Max, Output")]
public Vector3 objectAmount;
[Header("")]
public int amountPerPass;
    [NonReorderable]
    public NoiseObjects[] prefabs;


public Vector2 size;
public float maxYOffset;
public int orderInGeneration;
public Transform parentGameobject;

};

[System.Serializable]
public class NoiseObjects
{
public GameObject prefab;
public Vector2 objectRange;
};

