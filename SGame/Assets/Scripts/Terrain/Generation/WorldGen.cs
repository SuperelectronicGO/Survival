
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections.LowLevel;
using Unity.Collections.LowLevel.Unsafe;

using Random = UnityEngine.Random;
public class WorldGen : MonoBehaviour
{
    public bool h;
    public MapDisplay a;
    public MapDisplay b;
    public MapDisplay c;
    public MapDisplay d;
    public bool drawMap;
    

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
    [SerializeField] private GenerationScreen genScreen;
    public int seed;
    [NonReorderable]
    public Biome[] biomes;
    public MapMagic.Core.MapMagicObject M_Graph;
    public Transform terrainTransformParent;



    private List<Terrain> terrains = new List<Terrain>();
    private List<Transform> terrainParents = new List<Transform>();
    private List<TerrainData> tDatas = new List<TerrainData>();
   
    private int orderInWorldGeneration = 0;
    private bool startedMapmagicWorldGeneration = false;
    private RiverGeneration riverGen;


    private int2 mapDemensions;
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

       


        TerrainLayer[] layers = new TerrainLayer[biomes.Length];
        for(int i=0; i<biomes.Length; i++)
        {
            layers[i] = biomes[i].biomeLayer;
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
        if (Input.GetKeyDown(KeyCode.Z))
        {
          
            d.DrawNoiseMap(tempMap);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
         
            d.DrawNoiseMap(humMap);
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
        public NativeArray<float> positions;
        
        public NativeArray<float> tempPositions;
       
        public NativeArray<float> humPositions;
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
               
                float tempAmount = 1;
                float humAmount = 1;
                if (tempPositions[i] < (minTemp + difAmount))
                {
                    tempAmount = 1-(((minTemp + difAmount) - tempPositions[i]) * changeMult);
                   // tempAmount = 0;
                }else if (tempPositions[i] > (maxTemp - difAmount))
                {
                     tempAmount = (maxTemp + difAmount - tempPositions[i]) * changeMult;
                    //tempAmount = 0;
                }

                if (humPositions[i] < (minHum + difAmount))
                {
                       humAmount = 1 - (((minHum + difAmount) - humPositions[i]) * changeMult);
                    //humAmount = 1;
                }
                else if (humPositions[i] > (maxHum - difAmount))
                {
                       humAmount = (maxHum + difAmount - humPositions[i]) * changeMult;
                  //  humAmount = 1;
                }


                if (tempPositions[i] > minTemp + difAmount && tempPositions[i] < maxTemp - difAmount && humPositions[i] > minHum + difAmount && humPositions[i] < maxHum - difAmount)
                {
                   positions[i] = 1;
                }
                else
                {
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

                //potentially have to set positions[i]=0;






            













            }
}

   
    private IEnumerator textureTerrains()
    {
      
        //Generate octave offsets
        var randomTempSeed = new Unity.Mathematics.Random((uint)tempMapSeed);
        var randomHumidSeed = new Unity.Mathematics.Random((uint)humidMapSeed);
        float2[] tempOffsets = new float2[octaves];
        float2[] humidOffsets = new float2[octaves];
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

      

        //List of temperature and humidity maps for each terrain tile
        List<float[,]> tMapList = new List<float[,]>();
        List<float[,]> hMapList = new List<float[,]>();

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
                Debug.Log(tileOffest);
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

            if (generationStage == 2)
            {
                
                //Create nativearrays to hold the values for our floats
                NativeArray<float> tempNative;
                NativeArray<float> humidNative;
                //Set aside memory for these arrays
                tempNative = new NativeArray<float>(arraySize, Allocator.Persistent);
                humidNative = new NativeArray<float>(arraySize, Allocator.Persistent);
                //Create a 3d array to hold the values for this map
                float[,,] splatMap = new float[1025,1025, biomes.Length];
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
                for (int m = 0; m < biomes.Length; m++)
                {
                    NativeArray<float> biomeNativeSplatMap;
                    biomeNativeSplatMap = new NativeArray<float>(arraySize, Allocator.Persistent);

                    generateSplatJob = new generateSplat()
                    {
                        positions = biomeNativeSplatMap,
                        tempPositions = tempNative,
                        humPositions = humidNative,
                        minHum = biomes[m].minHum,
                        maxHum = biomes[m].maxHum,
                        minTemp = biomes[m].minTemp,
                        maxTemp = biomes[m].maxTemp
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

                            
                            //next try looking at results of heightmap gen next to eachother
                            splatMap[y,x, m] = generateSplatJob.positions[(y * 1025) + x];
                            
                            
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

    //OLD method to create biome textures. Hangs the main thread for minutes. This one worked ;/
    public void textureWorld()
    {
     //  StartCoroutine("textureTerrains");
       
       // /*
        setBiomeSplats();
            for (int i = 0; i < tDatas.Count; i++)
            {
            
                float[,,] splatMap = new float[tDatas[i].alphamapWidth, tDatas[i].alphamapHeight, tDatas[i].alphamapLayers];
                for (int y = 0; y < tDatas[i].alphamapHeight; y++)
                {
                    for (int x = 0; x < tDatas[i].alphamapWidth; x++)
                    {
                        for (int j = 0; j < biomes.Length; j++)
                        {
                            splatMap[y, x, j] = biomes[j].splatMap[x + Mathf.RoundToInt(terrainParents[i].transform.position.x * 1.024f), y + Mathf.RoundToInt(terrainParents[i].transform.position.z * 1.024f)];

                        }
                    }
                }
                tDatas[i].SetAlphamaps(0, 0, splatMap);
            
            
            }
     //   */
       
    }
    public void setBiomeSplats()
    {

        //Set the humidity and temperature arrays into nativearrays (2d to 1d)

        NativeArray<float> tempNative;
        NativeArray<float> humidNative;
       
        int arraySize = mapWidth * mapHeight;
        tempNative = new NativeArray<float>(arraySize, Allocator.TempJob);
        humidNative = new NativeArray<float>(arraySize, Allocator.TempJob);
        


        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                tempNative[y * mapHeight + x] = tempMap[x, y];
                humidNative[y * mapHeight + x] = humMap[x, y];

            }
        }
        
        for (int i = 0; i < biomes.Length; i++)
        {
            biomes[i].splatMap = new float[mapWidth, mapHeight];
            //Repeat for each biome
            NativeArray<float> biomeNativeSplatMap;
            biomeNativeSplatMap = new NativeArray<float>(arraySize, Allocator.TempJob);



            generateSplatJob = new generateSplat()
            {
                positions = biomeNativeSplatMap,
                tempPositions = tempNative,
                humPositions = humidNative,
                minHum = biomes[i].minHum,
                maxHum = biomes[i].maxHum,
                minTemp = biomes[i].minTemp,
                maxTemp = biomes[i].maxTemp
            };
            generateSplatHandle = generateSplatJob.Schedule(biomeNativeSplatMap.Length, 32);

            generateSplatHandle.Complete();

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    biomes[i].splatMap[x,y] = generateSplatJob.positions[y * mapHeight + x];
                }
            }

            biomeNativeSplatMap.Dispose();












        }
        tempNative.Dispose();
        humidNative.Dispose();


    }
   
}


// Biome class
[System.Serializable]
public class Biome
{
    public enum BiomeType
    {
        Swamp,
        Snow,
        Plains,
        Forest,
        Rocks,
        Rainforest,
        Desert,
        
    }
    public BiomeType biomeType;
    public TerrainLayer biomeLayer;
    public float minTemp;
    public float maxTemp;
    public float minHum;
    public float maxHum;

    public float[,] splatMap;
    
}
