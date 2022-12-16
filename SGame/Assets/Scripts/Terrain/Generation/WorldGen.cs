
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;


using Random = UnityEngine.Random;
public class WorldGen : MonoBehaviour
{
    
    //Job stuff
    JobHandle generateSplatHandle;
    generateSplat generateSplatJob;

   
    

  

    [Header("Generation Parts")]
    public bool generateTerrain = true;
    public bool textureTerrain = true;
    [Header("Noisemaps")]
    public bool randomSeed = true;

    public int mapWidth;
    public int mapHeight;
    public float tempScale;
    public float humidScale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int tempMapSeed;
    public int humidMapSeed;
    public Vector2 offset;
    public static float[,] tempMap;
    public static float[,] humMap;


    [Header("Data")]
    //Component that controls the generation screen
    [SerializeField] private GenerationScreen genScreen;
    //What seed to use for the world generation
    public int seed;
    //Mapmagic graph used in generation
    public MapMagic.Core.MapMagicObject M_Graph;
    //The parent of all terrains
    public Transform terrainTransformParent;
    //List of all main terrains terrains
    private List<Terrain> terrains = new List<Terrain>();
    //List of all terrain parents
    private List<Transform> terrainParents = new List<Transform>();
    //List of all terrainDatas
    private List<TerrainData> tDatas = new List<TerrainData>();
    //What stage of world generation we are on
    private int orderInWorldGeneration = 0;
    //Whether or not we started generating the mapmagic component
    private bool startedMapmagicWorldGeneration = false;
    //Reference to the riverGeneration component
    private RiverGeneration riverGen;
    //Int of the demensions of the terrainData maps
    private int2 mapDemensions;

    [Header("Biome Data")]
   //Scriptable Object with all the biomes and their settings
    public BiomeStorage biomeStorage;
    //Array storing what terrain tiles have what biome. Y is the index of the terrain tile and X is a 1 or 0 depending on if the biome exists.
    public int[,] biomesPerTerrainTile;
    //Arrays storing the max weights for trees and rocks for each biome, so we don't have to calculate it every tile later
    public int[] maxTreeWeights;
    public int[] maxRockWeights;
   




    //Lists of the temperature and humidity maps
    List<float[,]> tMapList = new List<float[,]>();
    List<float[,]> hMapList = new List<float[,]>();
    List<float[,,]> biomeSplatList = new List<float[,,]>();
    // Start is called before the first frame update
    void Start()
    {
       //Set terrain Biomes
        for (int i = 0; i < terrainTransformParent.childCount; i++)
        {
           
                terrains.Add(terrainTransformParent.GetChild(i).transform.Find("Main Terrain").gameObject.GetComponent<Terrain>());
                terrainParents.Add(terrainTransformParent.GetChild(i));
               
                tDatas.Add(terrainTransformParent.GetChild(i).transform.Find("Main Terrain").gameObject.GetComponent<Terrain>().terrainData);

            
        }

       


        TerrainLayer[] layers = new TerrainLayer[biomeStorage.biomes.Length];
      
            for (int i = 0; i < biomeStorage.biomes.Length; i++)
            {
                layers[i] = biomeStorage.biomes[i].biomeLayer;
            }
        
        for(int j=0; j<tDatas.Count; j++)
        {
            tDatas[j].terrainLayers = layers;
        }

        if (randomSeed)
        {
            tempMapSeed = Random.Range(0, 100000);
            humidMapSeed = Random.Range(0, 100000);
        }

        // int2 mapDemensions = new int2(mapWidth, mapHeight);
        // tempMap = BetterNoise.GenerateNoiseMap(mapDemensions, (uint)tempMapSeed, tempScale, octaves, persistance, lacunarity, offset);
        //  humMap = BetterNoise.GenerateNoiseMap(mapDemensions, (uint)humidMapSeed, humidScale, octaves, persistance, lacunarity, offset);
        //  riverGen = GetComponent<RiverGeneration>();
        //Generate octave offsets
        var randomTempSeed = new Unity.Mathematics.Random((uint)tempMapSeed);
        var randomHumidSeed = new Unity.Mathematics.Random((uint)humidMapSeed);
        float2[] tempOffsets = new float2[octaves];
        float2[] humidOffsets = new float2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            //X
            tempOffsets[i].x = randomTempSeed.NextInt(-100000, 100000);
            //Y
            tempOffsets[i].y = randomTempSeed.NextInt(-100000, 100000);
        }
        for (int i = 0; i < octaves; i++)
        {
            //X
            humidOffsets[i].x = randomHumidSeed.NextInt(-100000, 100000);
            //Y
            humidOffsets[i].y = randomHumidSeed.NextInt(-100000, 100000);
        }



        
        mapDemensions = new int2(1024, 1024);
        
        
   
    }

    // Update is called once per frame
    void Update()
    {
      
        if (Input.GetKeyDown(KeyCode.U))
        {
            // textureWorld();
              StartCoroutine(textureTerrains());
          
          
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            // textureWorld();
            StartCoroutine(generateObjects());


        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            int2 mapDemensions = new int2(mapWidth, mapHeight);
            tempMap = BetterNoise.GenerateNoiseMap(mapDemensions, (uint)tempMapSeed, tempScale, octaves, persistance, lacunarity, offset);
            humMap = BetterNoise.GenerateNoiseMap(mapDemensions, (uint)humidMapSeed, humidScale, octaves, persistance, lacunarity, offset);
        }
        if (orderInWorldGeneration == 0)
        {
            if (generateTerrain)
            {
                if (startedMapmagicWorldGeneration && !M_Graph.IsGenerating())
                {
                    Debug.Log("Finished mapmagic generation");
                    orderInWorldGeneration = 1;
                }
            }
            else
            {
                orderInWorldGeneration = 1;
                Debug.Log("Skipped mapmagic generation");
            }

        }
        if (textureTerrain)
        {
            if (orderInWorldGeneration == 1)
            {

            }
        }
        else
        {
            orderInWorldGeneration = 2;
            Debug.Log("Skipped texturing terrain");
        }



        // riverGen.GenerateRiverComponents();
        //Debug.Log("Generated river components");
        //orderInWorldGeneration = 2;
        
        
    }


    public void startGen()
    {
        seed = Random.Range(0, 100000);
        M_Graph.graph.random = new Den.Tools.Noise(seed, 32768);
        M_Graph.Refresh();
        startedMapmagicWorldGeneration = true;

    }

   // Method that compares the temperature and humidity maps to a biomes parameters to determine what areas of the map are that biome
   [BurstCompile]
    private struct generateSplat : IJobParallelFor { 
        //The final list for the biomes splatmap in 1D
        public NativeArray<float> positions;
        //The temperature map in 1D
        public NativeArray<float> tempPositions;
       //The humidity map in 1D
        public NativeArray<float> humPositions;
        //Specific settings for that biome
        public float minHum;
        public float maxHum;
        public float minTemp;
        public float maxTemp;
        

        public void Execute(int i)
    {
            
            
            float difAmount = 0.025f;
            float changeMult = 20;
            if(tempPositions[i]>(minTemp-difAmount)&&tempPositions[i]<(maxTemp+difAmount)&& humPositions[i] > (minHum-difAmount) && humPositions[i] < (maxHum+difAmount))
            {
                //Default to both at 1
                float tempAmount = 1;
                float humAmount = 1;
                //Check if on the verge of min temp
                if (tempPositions[i] < (minTemp + difAmount))
                {
                    tempAmount = 1-(((minTemp + difAmount) - tempPositions[i]) * changeMult);
                }
                //Check if on the verge of max temp
                else if (tempPositions[i] > (maxTemp - difAmount))
                {
                     tempAmount = (maxTemp + difAmount - tempPositions[i]) * changeMult;
                }

                //Check if on the verge of min humidity
                if (humPositions[i] < (minHum + difAmount))
                {
                       humAmount = 1 - (((minHum + difAmount) - humPositions[i]) * changeMult);
                }
                //Check if on the verge of max humidity
                else if (humPositions[i] > (maxHum - difAmount))
                {
                       humAmount = (maxHum + difAmount - humPositions[i]) * changeMult;
                }

                //If in range and not on the verge, set that we do have a biome here
                if (tempPositions[i] > minTemp + difAmount && tempPositions[i] < maxTemp - difAmount && humPositions[i] > minHum + difAmount && humPositions[i] < maxHum - difAmount)
                {
                    //Update positions map
                    positions[i] = 1;
                    
                    
                }
                else
                {
                    //If both are not equal (meaning one was on the verge), blend
                    if (tempAmount != 1 && humAmount != 1)
                    {
                        positions[i] = tempAmount * humAmount;
                        
                    }
                    else
                    {
                        if (tempAmount > humAmount)
                        {
                            positions[i] = humAmount;
                            
                        }
                        else
                        {
                            
                            positions[i] = tempAmount;
                            
                        }
                    }
                    
                           
                }
                
                }

            }
}
 

   
    private IEnumerator textureTerrains()
    {
        //Create a new array to store the biomes per terrain tile, X is biomes and Y is tiles
        biomesPerTerrainTile = new int[biomeStorage.biomes.Length, terrains.Count];
        //Generate octave offsets
        var randomTempSeed = new Unity.Mathematics.Random((uint)tempMapSeed);
        var randomHumidSeed = new Unity.Mathematics.Random((uint)humidMapSeed);
        float2[] tempOffsets = new float2[octaves];
        float2[] humidOffsets = new float2[octaves];
        //Give random octave offsets
        for(int i=0; i<octaves; i++)
        {
            //X
            tempOffsets[i].x = randomTempSeed.NextInt(-100000, 100000);
            //Y
            tempOffsets[i].y = randomTempSeed.NextInt(-100000, 100000);
        }
        for (int i = 0; i < octaves; i++)
        {
            //X
            humidOffsets[i].x = randomHumidSeed.NextInt(-100000, 100000);
            //Y
            humidOffsets[i].y = randomHumidSeed.NextInt(-100000, 100000);
        }

      

       
        

        /*What stage of generation we are on - 
         * 1. Generate temperature and humidity maps for each tile
         * 2. Evaluate the biomes for each map and update terrain splatmap
         */
        int generationStage = 1;
        //What index (terrain tile) we are generating the map for
        int currentMapIndex = 0;
        //What index (terrain tile) we are setting the splatmap for
        int currentTerrainIndex = 0;
        //Vector2 to hold the offset of the tile so it generates at the correct spot
        int2 tileOffest;
        //How many times to do the function
        int iterations = tDatas.Count;
        //Array size is equal to the demensions of the splatmap
        int arraySize = 1025*1025;
        mapDemensions = new int2(1025,1025);
        float offsetScale = 1.024f;
       
        
        while (currentTerrainIndex < iterations)
        {

            //Loop through all terrains at the beginning of the game and generate maps for them
            if (generationStage == 1)
            {
                //Update generation screen
                genScreen.currentGenStage = "Biome noisemaps";
                genScreen.genAmount = "(" + currentMapIndex + "/" + iterations + ")";
                genScreen.refreshGenerationUI();
                //Get the current offset of this terrain tile
                tileOffest = new int2();
               
                tileOffest.x = Mathf.RoundToInt(terrainParents[currentMapIndex].transform.position.x * offsetScale);
                tileOffest.y = Mathf.RoundToInt(terrainParents[currentMapIndex].transform.position.z * offsetScale);
                
                //Generate a temperature map for that tile and add it to the list
                tMapList.Add(TerrainNoise.GenerateNoiseMap(mapDemensions, (uint)tempMapSeed, tempScale, octaves, persistance, lacunarity, tileOffest));
                //Wait in order to not completely stall the main thread whilst all complete
                yield return new WaitForSeconds(0.02f);
                //Generate a humidity map for that tile and add it to the list
                hMapList.Add(TerrainNoise.GenerateNoiseMap(mapDemensions, (uint)humidMapSeed, humidScale, octaves, persistance, lacunarity, tileOffest));

              
                //Increment the current tile by one
                currentMapIndex += 1;
                //Update generation screen
                genScreen.genAmount = "("+currentMapIndex + "/" + iterations+")";
                genScreen.refreshGenerationUI();
                //Wait again in order to not stall the main thread
                yield return new WaitForSeconds(0.02f);

              
                //Check if map has been generated for all tiles, if so move on to the next tile and update the generation screen
                if (currentMapIndex == tDatas.Count)
                {
                //    a.DrawNoiseMap(p);
                    genScreen.currentGenStage = "Biome textures";
                    genScreen.genAmount = "(" + currentTerrainIndex + "/" + iterations + ")";
                    genScreen.refreshGenerationUI();
                    
                    generationStage = 2;
                   
                }

              }
            //Evaluate on a per biome basis and set splatmaps
            if (generationStage == 2)
            {
                
                //Create nativearrays to hold the values for our floats
                NativeArray<float> tempNative;
                NativeArray<float> humidNative;
                //Set aside memory for these arrays
                tempNative = new NativeArray<float>(arraySize, Allocator.Persistent);
                humidNative = new NativeArray<float>(arraySize, Allocator.Persistent);
                
                //Create a 3d array to hold the values for this map
                float[,,] splatMap = new float[1025,1025, biomeStorage.biomes.Length];
                //Iterate through the noisemaps and feed that value into the nativeArrays so it can travel between threads
                for (int y = 0; y < 1025; y++)
                {
                    for (int x = 0; x <1025; x++)
                    {
                        tempNative[(y * 1025) + x] = tMapList[currentTerrainIndex][x,y];
                        humidNative[(y * 1025) + x] = hMapList[currentTerrainIndex][x, y];
                        
                           
                        
                    }
                    if (y % 250 == 0)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }
                
                //After filling the nativearrays, call for the job to start evaluation
                for (int m = 0; m < biomeStorage.biomes.Length; m++)
                {
                    NativeArray<float> biomeNativeSplatMap;
                    biomeNativeSplatMap = new NativeArray<float>(arraySize, Allocator.Persistent);
                    

                    generateSplatJob = new generateSplat()
                    {
                        positions = biomeNativeSplatMap,
                        tempPositions = tempNative,
                        humPositions = humidNative,
                        minHum = biomeStorage.biomes[m].minHum,
                        maxHum = biomeStorage.biomes[m].maxHum,
                        minTemp = biomeStorage.biomes[m].minTemp,
                        maxTemp = biomeStorage.biomes[m].maxTemp,
                        
                    };
                   

                    //Schedule the job
                    generateSplatHandle = generateSplatJob.Schedule(biomeNativeSplatMap.Length, 128);
                    //Pause to not stutter frames
                    yield return new WaitForSeconds(0.1f);
                    //Force job to complete if it isn't yet
                    generateSplatHandle.Complete();
                    
                    //Copy float values to terrain array
                    for(int y=0; y<1025; y++)
                    {
                        for(int x=0; x<1025; x++)
                        {

                            
                           
                            splatMap[y,x, m] = generateSplatJob.positions[(y * 1025) + x];
                            if (splatMap[y, x, m] != 0)
                            {
                                //Update the array of what tiles have what biome at biome M for X and terrain CURRENTTERRAININDEX for Y
                                biomesPerTerrainTile[m, currentTerrainIndex] = 1;
                            }
                            
                        }
                        if (y % 250 == 0)
                        {
                            yield return new WaitForEndOfFrame();
                        }
                    }
                   

                      yield return new WaitForEndOfFrame();
                    
                    biomeNativeSplatMap.Dispose();
                    
                }

              
                //Update terrain tile
                tDatas[currentTerrainIndex].SetAlphamaps(0,0,splatMap);
                //Store SplatMap
                biomeSplatList.Add(splatMap);
                    
                if(tDatas[currentTerrainIndex]!=terrainParents[currentTerrainIndex].transform.Find("Main Terrain").GetComponent<Terrain>().terrainData)
                {
                    Debug.Log("no match");
                }
                yield return new WaitForEndOfFrame();
                tempNative.Dispose();
                humidNative.Dispose();
                

                //Move on to the text terrain tile
                currentTerrainIndex += 1;
                
                //Update generation screen values
                genScreen.genAmount = "(" + currentTerrainIndex + "/" + iterations + ")";
                genScreen.refreshGenerationUI();
               

            }
           
        }
        //Dispose of the nativearrays - This will only happen after every chunk has been generated (unity will hold these values in memory until then)

        
        yield break;
    }

    private IEnumerator generateObjects()
    {
        
        //Calculate the max weights from each biome
        maxTreeWeights = new int[biomeStorage.biomes.Length];
        maxRockWeights = new int[biomeStorage.biomes.Length];
        for(int i=0; i<biomeStorage.biomes.Length; i++)
        {
            for(int j=0; j<biomeStorage.biomes[i].biomeObjects.Length; j++)
            {
                switch (biomeStorage.biomes[i].biomeObjects[j].objType)
                {
                    case GenerateableObject.typeOfObject.Tree:
                        maxTreeWeights[i] += biomeStorage.biomes[i].biomeObjects[j].weight;
                        break;
                    case GenerateableObject.typeOfObject.Rock:
                        maxRockWeights[i] += biomeStorage.biomes[i].biomeObjects[j].weight;
                        break;

                }
            }
        }
        //How many times to run the loop initially
        int iterations = tDatas.Count;
        //Current index we are on
        int currentTile = 0;
        //Int to store how far the loop should progress before spawning again
        int density = 5;
        //Ints and bool to store if biome blending occurs
        int[] tempBiomeBlend = new int[biomeStorage.biomes.Length];
        bool doBlend = false;
        float blendAmount = 0;
        while (currentTile < iterations)
        {

            //Store the location of this tile
            Vector3 updatedTilePosition = terrains[currentTile].transform.position;
            //Define an array to store the current list of spawnable objects
            GenerateableObject[] tileObjects = { };
            //Int to keep track of the biome number selected. -1 So it throws an error if something doesn't work.
            int biomeIndex=-1;
            //Loop through points on the heightmap to spawn objects
            for (int y = 0; y < tDatas[currentTile].heightmapResolution; y += density)
            {
                for (int x = 0; x < tDatas[currentTile].heightmapResolution; x += density)
                {
                    tempBiomeBlend = new int[biomeStorage.biomes.Length];
                    doBlend = false;
                    blendAmount = 0;
                    //Loop through biomes to check which ones exist
                    for (int i = 0; i < biomeStorage.biomes.Length; i++)
                    {
                        //Check if that biome exists on this terrain tile. If it doesn't, continue and skip this tile.
                        if (biomesPerTerrainTile[i, currentTile] == 0)
                        {
                          //Do nothing
                        }
                        else
                        {
                            //Check biome splat map at this position
                            if (biomeSplatList[currentTile][y,x, i] != 0)
                            {
                                if (biomeSplatList[currentTile][y, x, i] == 1)
                                {
                                    //The biome is not blended
                                    tileObjects = biomeStorage.biomes[i].biomeObjects;
                                    biomeIndex = i;
                                }
                                else
                                {
                                    //The biome is blended, add its weight to the blend array and blendAmount, and mark that we must blend
                                    tempBiomeBlend[i] = 1;
                                    doBlend = true;
                                    blendAmount += biomeSplatList[currentTile][y, x, i];
                                }
                            }
                        }
                    }
                    if (doBlend)
                    {
                        float thisBlend = 0;
                        float blendNumber = Random.Range(0, blendAmount);
                        for(int i=0; i<biomeStorage.biomes.Length; i++)
                        {
                            if (biomeSplatList[currentTile][y, x, i]+thisBlend > blendNumber)
                            {
                                tileObjects = biomeStorage.biomes[i].biomeObjects;
                                biomeIndex = i;
                            }
                            else
                            {
                                thisBlend += biomeSplatList[currentTile][y, x, i];
                                break;
                            }
                        }
                    }
                  
                    if (biomeIndex != -1)
                    {
                        //Will need to be done for each type of objects, AS WELL AS RANDOM CHANCE PER TYPE
                        if (biomeStorage.biomes[biomeIndex].biomeObjects.Length == 0)
                        {
                            //No objects in this biome
                            continue;
                        }
                        else {
                        //Define random chance for this object to appear
                        int randomChance = Random.Range(0, 1000);
                            if (randomChance < 35)
                            {

                                //Pick a random object from the list depending on weights
                                int randomWeight = Random.Range(0, maxTreeWeights[biomeIndex]);
                                //Store the object we want
                                GenerateableObject selectedOb = null;
                                //Iterate through list until we find that point. Int is temporary storage of index
                                int tempIndex = 0;
                                for (int i = 0; i < tileObjects.Length; i++)
                                {
                                    if (tempIndex + tileObjects[i].weight > randomWeight)
                                    {
                                        //This object is the index we want
                                        selectedOb = tileObjects[i];
                                        break;
                                    }
                                    else
                                    {
                                        //Not at the index yet, add the failed objects weight to tempIndex
                                        tempIndex += tileObjects[i].weight;

                                    }
                                }
                                //Log error if things got messed up
                                if (selectedOb == null)
                                {
                                    //How the fuck did we get here?
                                    Debug.LogError("Finished iteration and selected object to spawn is null.");
                                }

                                //Set the position of where the object should spawn 
                                //MIGHT HAVE TO CHANGE TO GET HEIGHT AT WORLD COORDS NOT TERRAIN SPECIFIC ONES
                                Vector3 spawnedPosition = new Vector3(updatedTilePosition.x + (x / 1.025f), tDatas[currentTile].GetHeight(x,y), updatedTilePosition.z + (y / 1.025f));

                                //Actually spawn the object
                                GameObject spawnedObject = Instantiate(selectedOb.prefab, spawnedPosition, Quaternion.identity);
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("Biome index at -1, no biomes exists at this spot");
                    }
                    if (y % 500 == 0)
                    {
                        //Later maybe have options in settings to have faster or slower gen depending on how you want your performance
                        yield return new WaitForEndOfFrame();
                    }
                }
            }







            yield return new WaitForSeconds(0.1f);
            
            
           
            currentTile += 1;
            Debug.Log("Finished tile" + currentTile);
        }





        yield break;
    }
    
}



