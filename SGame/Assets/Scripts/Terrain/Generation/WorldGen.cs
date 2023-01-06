
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using GPUInstancer;

using Random = UnityEngine.Random;
public class WorldGen : MonoBehaviour
{

    //Jobhandles for the generateSplat job and the generateDetails job
    JobHandle generateSplatHandle;
    JobHandle generateDetailsHandle;
    //Store references
    generateSplat generateSplatJob;
    generateDetails generateDetailsJob;
   
    

  

    [Header("Generation Parts")]
    public bool generateTerrain = true;
    public bool textureTerrain = true;
    [Header("Noisemap Settings")]
    public bool randomSeed = true;
    public NoisemapSettingsScriptable tempMapSettings;
    public NoisemapSettingsScriptable humidMapSettings;
    public NoisemapSettingsScriptable biomeDensityMapSettings;

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
    public List<int[]> biomesPerTerrainTile = new List<int[]>();
    //Arrays storing the max weights for all objects in each biome, so we don't have to calculate it every tile later
    public int[] maxObjectWeights;
    //Lists of the temperature and humidity maps
    List<float[,]> tMapList = new List<float[,]>();
    List<float[,]> hMapList = new List<float[,]>();
    //List of biome splatMaps
    List<float[,,]> biomeSplatList = new List<float[,,]>();
    //List of biome density maps
    List<float[,]> biomeDensityMapList = new List<float[,]>();
    public Texture2D healthyDryNoiseTexture;







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
            tempMapSettings.seed = Random.Range(0, 100000);
            humidMapSettings.seed = Random.Range(0, 100000);
        }

        // int2 mapDemensions = new int2(mapWidth, mapHeight);
        // tempMap = BetterNoise.GenerateNoiseMap(mapDemensions, (uint)tempMapSeed, tempScale, octaves, persistance, lacunarity, offset);
        //  humMap = BetterNoise.GenerateNoiseMap(mapDemensions, (uint)humidMapSeed, humidScale, octaves, persistance, lacunarity, offset);
        //  riverGen = GetComponent<RiverGeneration>();
        //Generate octave offsets
        var randomTempSeed = new Unity.Mathematics.Random((uint)tempMapSettings.seed);
        var randomHumidSeed = new Unity.Mathematics.Random((uint)humidMapSettings.seed);
        float2[] tempOffsets = new float2[tempMapSettings.octaves];
        float2[] humidOffsets = new float2[humidMapSettings.octaves];
        for (int i = 0; i < tempMapSettings.octaves; i++)
        {
            //X
            tempOffsets[i].x = randomTempSeed.NextInt(-100000, 100000);
            //Y
            tempOffsets[i].y = randomTempSeed.NextInt(-100000, 100000);
        }
        for (int i = 0; i < humidMapSettings.octaves; i++)
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

        if (Input.GetKeyDown(KeyCode.Y))
        {
            StartCoroutine(GPUIIntitialization());
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            // textureWorld();
            //StartCoroutine(textureTerrains());
            StartCoroutine(initialTerrainGeneration());
          
          
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            // textureWorld();
            StartCoroutine(generateObjects());


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



    //Setup method to initialize and generate certain values
    private void beforeTerrainGeneration()
    {
        var randomTempSeed = new Unity.Mathematics.Random((uint)tempMapSettings.seed);
        var randomHumidSeed = new Unity.Mathematics.Random((uint)humidMapSettings.seed);
        float2[] tempOffsets = new float2[tempMapSettings.octaves];
        float2[] humidOffsets = new float2[humidMapSettings.octaves];
        //Give random octave offsets
        for (int i = 0; i < tempMapSettings.octaves; i++)
        {
            //X
            tempOffsets[i].x = randomTempSeed.NextInt(-100000, 100000);
            //Y
            tempOffsets[i].y = randomTempSeed.NextInt(-100000, 100000);
        }
        for (int i = 0; i < humidMapSettings.octaves; i++)
        {
            //X
            humidOffsets[i].x = randomHumidSeed.NextInt(-100000, 100000);
            //Y
            humidOffsets[i].y = randomHumidSeed.NextInt(-100000, 100000);
        }
    }
    //Method that generates the initial tiles
    bool moveNext = false;
    private IEnumerator initialTerrainGeneration()
    {
        int num = 0;
        beforeTerrainGeneration();
        while (num < tDatas.Count)
        {
            StartCoroutine(generateTerrainTile(terrains[num], terrainParents[num], num));
            yield return new WaitUntil(() => moveNext = true);
            yield return new WaitForSeconds(1f);
            num += 1;
            moveNext = false;
            Debug.Log("Finished tile " + (num-1));
        }


        yield break;
    }



    private IEnumerator GPUIIntitialization()
    {
        //Current terrain number
        int iterationNumber = 0;
        //Temporary reference to the current detail prototype scriptable object
        GPUInstancerDetailPrototype currentDetailPrototype = null;
       
        
        while (iterationNumber < terrains.Count)
        {


                GameObject g = new GameObject();
                g.SetActive(false);
                g.name = "GPUI for " + terrainParents[iterationNumber].name;
                g.transform.parent = terrains[iterationNumber].transform;
                GPUInstancerDetailManager detailManager = g.AddComponent(typeof(GPUInstancerDetailManager)) as GPUInstancerDetailManager;
                detailManager.AddTerrain(terrains[iterationNumber]);
                GPUInstancerAPI.SetupManagerWithTerrain(detailManager, terrains[iterationNumber]);
                detailManager.runInThreads = true;
                detailManager.isFrustumCulling = true;
                detailManager.isOcclusionCulling = true;
                detailManager.terrainSettings = ScriptableObject.CreateInstance<GPUInstancerTerrainSettings>();
                detailManager.terrainSettings.maxDetailDistance = 500;
                detailManager.terrainSettings.autoSPCellSize = true;
                for (int i = 0; i < detailManager.prototypeList.Count; i++)
                {
                currentDetailPrototype = detailManager.prototypeList[i] as GPUInstancerDetailPrototype;
                currentDetailPrototype.quadCount = 4;
                }
                g.SetActive(true);
                yield return new WaitForEndOfFrame();
                iterationNumber += 1;
            
        }


        yield break;
    }

    //Job that compares the temperature and humidity maps to a biomes parameters to determine what areas of the map are that biome
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

    //Job that generates the detail maps for a terrain tile
    [BurstCompile]
    private struct generateDetails : IJobParallelFor
    {
        //NativeArray storing the detail map in 1D
        public NativeArray<int> collapsedDetailMap;
        //NativeArray storing the biome texture map in 1D
        public NativeArray<float> tileBiomeMap;

        public void Execute(int i)
        {
          //  if (tileBiomeMap[i] != 0)
          //  {
                collapsedDetailMap[i] = 2;
          //  }
        }


    }
 

    //Method that generates a terrain tile
    private IEnumerator generateTerrainTile(Terrain terrain, Transform terrainParent, int tileNumber)
    {
        #region References
        //Vector2 to hold the offset of the tile so it generates at the correct spot
        int2 tileOffest;
        //Array size is equal to the demensions of the splatmap
        int arraySize = 1025 * 1025;
        mapDemensions = new int2(1025, 1025);
        float offsetScale = 1.024f;
        //Create nativearrays to hold the values for our floats
        NativeArray<float> tempNative;
        NativeArray<float> humidNative;
        //List of NativeArrays that hold the splatmaps of each biome
        List<NativeArray<float>> biomeNativeArrays = new List<NativeArray<float>>();
        //Set aside memory for these arrays
        tempNative = new NativeArray<float>(arraySize, Allocator.Persistent);
        humidNative = new NativeArray<float>(arraySize, Allocator.Persistent);
        //Create a 3d array to hold the biome texture values for this map
        float[,,] splatMap = new float[1025, 1025, biomeStorage.biomes.Length];
        //Reference to the GPUI detail prototype
        GPUInstancerDetailPrototype currentDetailPrototype = null;
        //List of detail prototypes for this terrain
        List<DetailPrototype> terrainDetailPrototypes = new List<DetailPrototype>();
        #endregion





        //Log this tile in the terrain tile biome list
        biomesPerTerrainTile.Add(new int[biomeStorage.biomes.Length]);

        #region Texturing
        //Get the offset of this tile in the world
        tileOffest = new int2();
        tileOffest.x = Mathf.RoundToInt(terrainParent.transform.position.x * offsetScale);
        tileOffest.y = Mathf.RoundToInt(terrainParent.transform.position.z * offsetScale);
        //Generate a temperature map for that tile and add it to the list
        tMapList.Add(TerrainNoise.GenerateNoiseMap(mapDemensions, (uint)tempMapSettings.seed, tempMapSettings.scale, tempMapSettings.octaves, tempMapSettings.persistance, tempMapSettings.lacunarity, tileOffest));
        //Wait in order to not completely stall the main thread whilst all complete
        yield return new WaitForSeconds(0.03f);
        //Generate a humidity map for that tile and add it to the list
        hMapList.Add(TerrainNoise.GenerateNoiseMap(mapDemensions, (uint)humidMapSettings.seed, humidMapSettings.scale, humidMapSettings.octaves, humidMapSettings.persistance, humidMapSettings.lacunarity, tileOffest));
        yield return new WaitForSeconds(0.03f);

        //Iterate through the noisemaps and feed that value into the nativeArrays so it can travel between threads
        for (int y = 0; y < 1025; y++)
        {
            for (int x = 0; x < 1025; x++)
            {
                tempNative[(y * 1025) + x] = tMapList[tileNumber][x, y];
                humidNative[(y * 1025) + x] = hMapList[tileNumber][x, y];



            }
            if (y % 250 == 0)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        for (int m = 0; m < biomeStorage.biomes.Length; m++)
        {
            
            biomeNativeArrays.Add(new NativeArray<float>(arraySize, Allocator.Persistent));


            generateSplatJob = new generateSplat()
            {
                positions = biomeNativeArrays[m],
                tempPositions = tempNative,
                humPositions = humidNative,
                minHum = biomeStorage.biomes[m].minHum,
                maxHum = biomeStorage.biomes[m].maxHum,
                minTemp = biomeStorage.biomes[m].minTemp,
                maxTemp = biomeStorage.biomes[m].maxTemp,

            };


            //Schedule the job
            generateSplatHandle = generateSplatJob.Schedule(biomeNativeArrays[m].Length, 128);
            //Pause to not stutter frames
            yield return new WaitUntil(() => generateSplatHandle.IsCompleted == true);
            generateSplatHandle.Complete();
            yield return new WaitForEndOfFrame();
            

            //Copy float values to terrain array
            for (int y = 0; y < 1025; y++)
            {
                for (int x = 0; x < 1025; x++)
                {



                    splatMap[y, x, m] = generateSplatJob.positions[(y * 1025) + x];
                    if (splatMap[y, x, m] != 0)
                    {
                        //Update the array of what tiles have what biome at biome M for X and terrain CURRENTTERRAININDEX for Y
                        biomesPerTerrainTile[tileNumber][m] = 1;
                    }

                }
                if (y % 250 == 0)
                {
                    yield return new WaitForEndOfFrame();
                }
            }


            yield return new WaitForEndOfFrame();

            

        }

        //Update terrain tile
        tDatas[tileNumber].SetAlphamaps(0, 0, splatMap);
        //Store SplatMap
        biomeSplatList.Add(splatMap);
        Debug.Log("got here");



        //Dispose of NativeArrays
        
        tempNative.Dispose();
        humidNative.Dispose();
        
        #endregion

        #region Details

        //Get a list of all the detail prototypes we will use for this terrain
        for(int i=0; i<biomeStorage.biomes.Length; i++)
        {
            //Skip this biome if it doesn't exist on the tile
            if (biomesPerTerrainTile[tileNumber][i] == 0)
            {
                continue;
            }

            //If it does, add each detail in this biome to the list of details
            for(int j=0; j<biomeStorage.biomes[i].details.Length; i++)
            {
                //Fill in settings
                DetailPrototype proto = new DetailPrototype();
                DetailObjectScriptable settings = biomeStorage.biomes[i].details[j];
                proto.healthyColor = settings.healthyColor;
                proto.dryColor = settings.dryColor;
                proto.holeEdgePadding = settings.holeEdgePadding;
                proto.minHeight = settings.minHeight;
                proto.maxHeight = settings.maxHeight;
                proto.minWidth = settings.minWidth;
                proto.maxWidth = settings.maxWidth;
                proto.noiseSeed = settings.noiseSeed;
                proto.noiseSpread = settings.noiseSpread;
                proto.usePrototypeMesh = settings.usePrototypeMesh;
                if (proto.usePrototypeMesh)
                {
                    proto.prototype = settings.prototypeMesh;
                }
                else
                {
                    proto.prototypeTexture = settings.prototypeTexture;
                }
                
                terrainDetailPrototypes.Add(proto);
            }

        }
        //Assign detail prototypes to this terrains as an array
        tDatas[tileNumber].detailPrototypes = new DetailPrototype[0];
        tDatas[tileNumber].detailPrototypes = terrainDetailPrototypes.ToArray();
        
            //Create a new gameobject to be the detail manager
            GameObject GPUIManagerObject = new GameObject();
            //Disable the object so we can set all of the references and so generation is done before it enables
            GPUIManagerObject.SetActive(false);
            //Name the detail manager to be the GPUI for that terrain tile
            GPUIManagerObject.name = "GPUI for " + terrains[tileNumber].name;
            //Set the detail manager to be the child of its terrain
            GPUIManagerObject.transform.parent = terrains[tileNumber].transform;
            //Add the GPUInstancerDetailManager class to the object
            GPUInstancerDetailManager detailManager = GPUIManagerObject.AddComponent(typeof(GPUInstancerDetailManager)) as GPUInstancerDetailManager;
            //Set the terrain of the manager
            detailManager.AddTerrain(terrains[tileNumber]);
            //Call the GPUInstancer API function to sync the terrain with the manager
            GPUInstancerAPI.SetupManagerWithTerrain(detailManager, terrains[tileNumber]);
            //Fill in settings to the detail manager
            detailManager.runInThreads = true;
            detailManager.isFrustumCulling = true;
            detailManager.isOcclusionCulling = true;
            detailManager.terrainSettings.maxDetailDistance = 500;
            detailManager.terrainSettings.autoSPCellSize = true;

            //Generate a new scriptableObject to set as the managers terrain settings
            detailManager.terrainSettings = ScriptableObject.CreateInstance<GPUInstancerTerrainSettings>();


            //Setup each detail protoype on the manager. Later, each of these will inherit its values from my own scriptableObjects
            for (int i = 0; i < detailManager.prototypeList.Count; i++)
            {
                currentDetailPrototype = detailManager.prototypeList[i] as GPUInstancerDetailPrototype;
                currentDetailPrototype.quadCount = 4;
                
            }

            //Generate the detail maps for each of the details
            for (int i = 0; i < biomeStorage.biomes.Length; i++)
            {
                //Check to see if this terrain tile has this biome
                if (biomesPerTerrainTile[tileNumber][i] == 0)
                {
                    //Biome doesn't exist on this tile, skip and don't generate a map for it
                    continue;
                }


                //If the biome does exist, create a job to set its details
                //Define a NativeArray that will hold the collapsed detail map
                NativeArray<int> collapsedDetailMap = new NativeArray<int>(tDatas[tileNumber].detailHeight * tDatas[tileNumber].detailWidth, Allocator.Persistent);
                generateDetailsJob = new generateDetails
                {
                    collapsedDetailMap = collapsedDetailMap,
                    tileBiomeMap = biomeNativeArrays[i]
                };
                //Schedule the job
                generateDetailsHandle = generateDetailsJob.Schedule(tDatas[tileNumber].detailHeight * tDatas[tileNumber].detailWidth, 128);
                //Wait until the job is completed
                yield return new WaitUntil(() => generateDetailsHandle.IsCompleted == true);
                //Call the job to force completion because Unity throws an error if an attempt is made to access te jobs values without a Complete() call
                generateDetailsHandle.Complete();
                //Wait till the end of the frame
                yield return new WaitForEndOfFrame();

                //Define a temporary detail map
                int[,] DetailMap = new int[tDatas[tileNumber].detailHeight, tDatas[tileNumber].detailWidth];
                //Construct the NativeArray into a 2D map
                for (int y = 0; y < tDatas[tileNumber].detailHeight; y++)
                {
                    for (int x = 0; x < tDatas[tileNumber].detailWidth; x++)
                    {
                        //Flip Y and X because had to do it with texturing I guess
                        DetailMap[y, x] = generateDetailsJob.collapsedDetailMap[y * tDatas[tileNumber].detailHeight + x];
                    }
                }
            //Set the detail layer - only layer 0 for now
            if (tDatas[tileNumber].detailPrototypes.Length != 0)
            {
                tDatas[tileNumber].SetDetailLayer(0, 0, 0, DetailMap);
            }
                //Dispose of the detail map
                collapsedDetailMap.Dispose();
            }


            //After everything, enable the GPUIManager
            GPUIManagerObject.SetActive(true);
            //Pause till the end of the frame
            yield return new WaitForEndOfFrame();
            //Dispose of the biomes maps
            for (int i = 0; i < biomeNativeArrays.Count; i++)
            {
                biomeNativeArrays[i].Dispose();
            }

            #endregion
        
        //For mass tile generation
        moveNext = true;
        yield break;
    }






  

    //Method that generates all the objects that a tile will have
    private IEnumerator generateObjects()
    {
        //Calculate the max weights from each biome
        maxObjectWeights = new int[biomeStorage.biomes.Length];
        //Iterate to set the max weights for each biome
        for (int i = 0; i < biomeStorage.biomes.Length; i++)
        {
            for(int j = 0; j<biomeStorage.biomes[i].biomeObjects.Length; j++) {
                maxObjectWeights[i]+=biomeStorage.biomes[i].biomeObjects[j].weight;
            }
        }
        //How many times to run the loop initially
        int iterations = tDatas.Count;
        //Current index we are on
        int currentTile = 0;
        //Int to store how far the loop should progress before spawning again
        int density = 8;
        //Ints and bool to store if biome blending occurs
        int[] tempBiomeBlend = new int[biomeStorage.biomes.Length];
        bool doBlend = false;
        float blendAmount = 0;
        //Demensions of the biome density map
        int2 densityMapDimensions = new int2(257, 257);
        //Offset of each tile
        int2 tileOffset;
        float offsetScale = 1.024f;
        while (currentTile < iterations)
        {
            //Define the new tileOffset - division by 4 is because each tile is 256x256, not 1024x1024
            tileOffset = new int2();
            tileOffset.x = Mathf.RoundToInt(terrainParents[currentTile].transform.position.x * offsetScale)/4;
            tileOffset.y = Mathf.RoundToInt(terrainParents[currentTile].transform.position.z * offsetScale)/4;
            //Store the location of this tile
            Vector3 updatedTilePosition = terrains[currentTile].transform.position;
            //Define an array to store the current list of spawnable objects
            GenerateableObject[] tileObjects = { };
            //Int to keep track of the biome number selected. -1 So it throws an error if something doesn't work.
            int biomeIndex=-1;
            //Generate a noisemap for the density of this tile
            biomeDensityMapList.Add(TerrainNoise.GenerateNoiseMap(densityMapDimensions, (uint)biomeDensityMapSettings.seed, biomeDensityMapSettings.scale, biomeDensityMapSettings.octaves, biomeDensityMapSettings.persistance, biomeDensityMapSettings.lacunarity, tileOffset));
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
                        if (biomesPerTerrainTile[currentTile][i] == 0)
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
                    //If blending, decide a biome that will take prevelance at this spot (weighted by biome splatmap #)
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
                    //As long as there is actually a biome, generate
                    if (biomeIndex != -1)
                    {

                       
                        //Check that there are objects in this biome
                        if (biomeStorage.biomes[biomeIndex].biomeObjects.Length == 0)
                        {
                            //No objects in this biome
                            continue;
                        }
                        else {
                        //Define random chance for this object to appear
                        int randomChance = Random.Range(0, 1000);
                            /* The odds of this object appearing depend on the chance for its category defined above.
                             * This number is multiplied by the value of the biome density map + 0.25 (normalized between 0 and 1)
                               to decide if an object is actually spawned at this spot. The biome density X and Y are divided by 4 as it samples a smaller map. */
                            if (randomChance < biomeStorage.biomes[biomeIndex].biomeDensity * Mathf.Lerp(0, 1.25f, (biomeDensityMapList[currentTile][x/4, y/4] + 0.25f))) 
                            {
                                //Pick a random object from the list depending on weights
                                int randomWeight = Random.Range(0, maxObjectWeights[biomeIndex]);
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

                                //Store a reference to the tiles normals
                                float newX = x;
                                float newY = y;
                                Vector3 spotNormal = tDatas[currentTile].GetInterpolatedNormal(newX/1025,newY/1025);
                                //Set the position of where the object should spawn
                                Vector3 spawnedPosition = new Vector3(updatedTilePosition.x + (x / 1.025f), tDatas[currentTile].GetHeight(x,y)+selectedOb.yOffset, updatedTilePosition.z + (y / 1.025f));
                                //Change spawn position Y depending on normals so roots and things don't stick out
                                if (selectedOb.offsetHeightByNormals)
                                {
                                    //100 is the multiple so it doesn't stick out of the terrain on large slopes
                                    spawnedPosition.y -= (1 - Mathf.Abs(spotNormal.y)) * 100;
                                }
                                //Rotate objects
                                Vector3 objectRotation = Vector3.zero;
                                if (selectedOb.alignNormals)
                                {
                                    objectRotation = spotNormal;
                                   // objectRotation.x *= -1;
                                    //objectRotation.z *= -1;
                                }
                                //Actually spawn the object
                                GameObject spawnedObject = Instantiate(selectedOb.prefab, spawnedPosition, Quaternion.FromToRotation(transform.up, objectRotation));
                                //Scale the objects
                                float randomAmnt = Random.Range(0f, 1f);
                                float newObjectScale = selectedOb.scale.Evaluate(randomAmnt);
                                spawnedObject.transform.localScale = new Vector3(newObjectScale, newObjectScale, newObjectScale);
                                spawnedObject.name = spotNormal.ToString();
                                
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("Biome index at -1, no biomes exists at this spot");
                    }
                    if (y % 800 == 0)
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




