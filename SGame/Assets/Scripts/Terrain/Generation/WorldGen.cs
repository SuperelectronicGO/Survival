using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using GPUInstancer;
using Unity.Netcode;
using Random = UnityEngine.Random;
public class WorldGen : NetworkBehaviour
{

    public bool testing;
    List<Material> blendMats = new List<Material>();



    public static WorldGen instance { get; private set; }
    [Header("Speed")]
    public GenerationSpeedSettings currentSpeedSettings;
    #region Jobs
    //Jobhandles for the generateSplat job and the generateDetails job
    JobHandle generateSplatHandle;
    JobHandle generateDetailsHandle;
    //Store references
    generateSplat generateSplatJob;
    generateDetails generateDetailsJob;
    #endregion
    //TODO - DISPLAY THESE!
    public float[,] splatMapDis = new float[1025, 1025];
    public int[,] detailMapDis;
    public MapDisplay d;

    [Header("Generation Parts")]
    public bool generateTerrain = true;
    public bool textureTerrain = true;

    #region noisemap settings
    [Header("Noisemap Settings")]
    public bool randomSeed = true;
    public NoisemapSettingsScriptable tempMapSettings;
    public NoisemapSettingsScriptable humidMapSettings;
    public NoisemapSettingsScriptable biomeDensityMapSettings;
    public VoronoiSettingsScriptable subBiomeMapSettings;


    #endregion

    [Header("Data")]
    private bool generateOnLoad = false;
    //Player reference
    [SerializeField] private Transform player;
    //Terrain material
    public Material terrainMaterial;
    //Custom blend material
    public Material terrainBlendMaterial;
    //Component that controls the generation screen
    [SerializeField] private GenerationScreen genScreen;
    //What seed to use for the world generation
    public int seed;
    //Mapmagic graph used in generation
    public MapMagic.Core.MapMagicObject M_Graph;
    //The parent of all terrains
    public Transform terrainTransformParent;
    //How tall the terrains are
    [SerializeField] private int terrainHeight = 800;
    //How large one dimension of the terrains heightmap is
    [SerializeField] private int terrainHeightmapLength = 1025;
    //List of all main terrains 
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
    //Array storing what terrain tiles have what biome. LIST index is the tile number, Y is the biome, X is the subbiome
    public List<int[,]> biomesPerTerrainTile = new List<int[,]>();
    //Healthy/Dry noise texture and normals for GPUI
    [SerializeField]
    private static Texture2D GPUInstancerHealthyDryNoiseTexture;
    [SerializeField]
    private static Texture2D GPUInstancerWindWaveNormalTexture;
    //Arrays storing the max weights for all objects in each biome, so we don't have to calculate it every tile later
    public int[,] maxObjectWeights;
    [Header("Building Data")]
    [NonReorderable]
    public List<BuildingData> buildings = new List<BuildingData>();

    #region temporary data (voronoi)
    private bool voronoiFinishCompletion = false;
    #endregion






    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        //Set terrain Biomes
        for (int i = 0; i < terrainTransformParent.childCount; i++)
        {

            terrains.Add(terrainTransformParent.GetChild(i).transform.Find("Main Terrain").gameObject.GetComponent<Terrain>());
            terrainParents.Add(terrainTransformParent.GetChild(i));

            tDatas.Add(terrainTransformParent.GetChild(i).transform.Find("Main Terrain").gameObject.GetComponent<Terrain>().terrainData);
            //set material
            //terrains[i].materialTemplate = terrainMaterial;
        }

        TerrainLayer[] layers = new TerrainLayer[biomeStorage.biomes.Length];
        int maxSubLength = -10;
        for (int i = 0; i < biomeStorage.biomes.Length; i++)
        {
            for (int sub = 0; sub < biomeStorage.biomes[i].subBiomes.Length; sub++)
            {
                layers[i] = biomeStorage.biomes[i].subBiomes[sub].biomeLayer;
                if (biomeStorage.biomes[i].subBiomes[sub].biomeObjects.Length > maxSubLength)
                {
                    maxSubLength = biomeStorage.biomes[i].subBiomes[sub].biomeObjects.Length;
                }
            }
        }

        maxObjectWeights = new int[maxSubLength + 1, biomeStorage.biomes.Length];

        for (int j = 0; j < tDatas.Count; j++)
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

        //Generate world on load if selected in menu and we are host
        if (GameNetworkManager.Instance != null)
        {
            if (GameNetworkManager.Instance.generateWorld)
            {
                if (!NetworkManager.Singleton.IsHost) return;
                StartCoroutine(initialTerrainGeneration());
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (NetworkManager.Singleton.IsHost && Input.GetKeyDown(KeyCode.O))
        {
            SGClientRPC();
        }

        if (!testing)
        {
            if (Input.GetKeyDown(KeyCode.U) && !GameNetworkManager.Instance.generateWorld)
            {
                // textureWorld();
                //StartCoroutine(textureTerrains());
                StartCoroutine(initialTerrainGeneration());


            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                // textureWorld();
                //StartCoroutine(textureTerrains());
                StartCoroutine(initialTerrainGeneration());


            }
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

    [ClientRpc]
    public void SGClientRPC()
    {
        StartCoroutine(initialTerrainGeneration());
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
        //Calculate the max weights from each biome

        //Iterate to set the max weights for each biome
        for (int i = 0; i < biomeStorage.biomes.Length; i++)
        {
            for (int sub = 0; sub < biomeStorage.biomes[i].subBiomes.Length; sub++)
            {
                for (int j = 0; j < biomeStorage.biomes[i].subBiomes[sub].biomeObjects.Length; j++)
                {
                    maxObjectWeights[sub, i] += biomeStorage.biomes[i].subBiomes[sub].biomeObjects[j].weight;
                }
            }
        }


    }

    //Method that generates the initial tiles
    bool moveNext = false;
    private IEnumerator initialTerrainGeneration()
    {
        int num = 0;
        beforeTerrainGeneration();
        //Get the list of building spots if we are the server
        if (NetworkManager.Singleton.IsServer)
        {
            GetBuildingSpotList();
        }
        while (num < tDatas.Count - 50)
        {

            StartCoroutine(generateTerrainTile(terrains[num], terrainParents[num], num, new int2(Mathf.RoundToInt(terrains[num].transform.position.x / 1000), Mathf.RoundToInt(terrains[num].transform.position.z / 1000))));
            yield return new WaitUntil(() => moveNext == true);

            num += 1;
            moveNext = false;
            Debug.Log("Finished tile " + (num - 1));
        }

        //Alert all clients that generation is done if we are the host. If not, wait until the host is done to spawn
        if (NetworkManager.Singleton.IsHost)
        {
            PlayerNetwork.instance.AlertGenerationDoneClientRPC();
            PlayerNetwork.instance.hostFinishedGenerating = true;
        }
        else
        {
            genScreen.SetWaitingForHost();
            yield return new WaitUntil(() => PlayerNetwork.instance.hostFinishedGenerating == true);
        }
        if (GameNetworkManager.Instance != null)
        {
            PlayerNetwork.instance.SpawnPlayer();
        }
        StartCoroutine(PlayerNetwork.instance.InventorySetupOnBeforeSpawn());
        genScreen.canvasObject.SetActive(false);
        yield break;
    }



    //Job that compares the temperature and humidity maps to a biomes parameters to determine what areas of the map are that biome
    [BurstCompile]
    private struct generateSplat : IJobParallelFor
    {
        //The final list for the biomes splatmap in 1D
        public NativeArray<float> positions;
        //The temperature map in 1D
        public NativeArray<float> tempPositions;
        //The humidity map in 1D
        public NativeArray<float> humPositions;
        //The subbiome native
        public NativeArray<float3> subBiomeNative;
        //Specific settings for that biome
        public float minHum;
        public float maxHum;
        public float minTemp;
        public float maxTemp;
        public float minSub;
        public float maxSub;


        public void Execute(int i)
        {


            float difAmount = 0.025f;
            float changeMult = 20;
            float weight = 0;
            //Determine BIOME value
            if (tempPositions[i] > (minTemp - difAmount) && tempPositions[i] < (maxTemp + difAmount) && humPositions[i] > (minHum - difAmount) && humPositions[i] < (maxHum + difAmount))
            {

                //Default to both at 1
                float tempAmount = 1;
                float humAmount = 1;
                //Check if on the verge of min temp
                if (tempPositions[i] < (minTemp + difAmount))
                {
                    tempAmount = 1 - (((minTemp + difAmount) - tempPositions[i]) * changeMult);
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
                    weight = 1;


                }
                else
                {
                    //If both are not equal (meaning one was on the verge), blend
                    if (tempAmount != 1 && humAmount != 1)
                    {
                        weight = tempAmount * humAmount;

                    }
                    else
                    {
                        if (tempAmount > humAmount)
                        {
                            weight = humAmount;

                        }
                        else
                        {

                            weight = tempAmount;

                        }
                    }


                }



                //Only calculate subbiomes if there is more than one 




                //  positions[i] *= 1 - (subBiomeNative[i].y / 2);

                //First don't blend if only one subbiome
                if ((minSub == 0 && maxSub == 1) || (minSub <= subBiomeNative[i].x && maxSub >= subBiomeNative[i].x && minSub <= subBiomeNative[i].z && maxSub >= subBiomeNative[i].z && subBiomeNative[i].y != 1))
                {

                }
                else if (minSub <= subBiomeNative[i].x && maxSub >= subBiomeNative[i].x)
                {
                    //Keep normal, unless there is blending here
                    if (subBiomeNative[i].y != 1)
                    {
                        weight *= 1 - (subBiomeNative[i].y / 2);

                    }
                }
                else if (minSub <= subBiomeNative[i].z && maxSub >= subBiomeNative[i].z && subBiomeNative[i].y != 1)
                {
                    weight *= subBiomeNative[i].y / 2;

                }
                else
                {
                    //Blend
                    // 
                    weight = 0;
                }


                positions[i] = weight;
            }



        }
    }

    //Job that generates the detail maps for a terrain tile
    [BurstCompile]
    private struct generateDetails : IJobParallelFor
    {
        //NativeArray storing the detail map in 1D. 1025x1025 even though detail is 1024x1024 to make room for splatmap
        public NativeArray<int> collapsedDetailMap;
        //NativeArray storing the biome texture map in 1D
        public NativeArray<float> tileBiomeMap;
        //Voronoi map for subbiomes
        public NativeArray<float3> subBiomeNative;
        //Density that this detail should generate as
        public int detailDensity;
        //Chance of the object spawning here
        public int spawnChance;
        //Random number for generator
        public Unity.Mathematics.Random randomInt;
        //Min and max for voronoi mao
        public float minSub;
        public float maxSub;
        /* * * * * * * Patches * * * * * * */
        //Does this generate in clumps?
        public bool generateInPatches;
        //Patch size
        public float patchCutoff;
        //Noisemap for patches
        public NativeArray<float> patchNoisemap;
        //Patches fade margin
        public float patchFadeMargin;
        public void Execute(int i)
        {
            float weight = 1;
            int densityNum = 0;
            //Calculate if the subbiome is blending
            if ((minSub == 0 && maxSub == 1) || (minSub <= subBiomeNative[i].x && maxSub >= subBiomeNative[i].x && minSub <= subBiomeNative[i].z && maxSub >= subBiomeNative[i].z && subBiomeNative[i].y != 1))
            {
                //We are in the correct spot, don't change the weight
            }
            else if (minSub <= subBiomeNative[i].x && maxSub >= subBiomeNative[i].x)
            {
                //Blend if we are in a blend spot
                if (subBiomeNative[i].y != 1)
                {
                    weight *= 1 - (subBiomeNative[i].y / 2);

                }
            }
            else if (minSub <= subBiomeNative[i].z && maxSub >= subBiomeNative[i].z && subBiomeNative[i].y != 1)
            {
                //Blend if we are in a blend spot
                weight *= subBiomeNative[i].y / 2;

            }
            else
            {
                //Not in the correct subbiome
                weight = 0;
            }
            if (weight > 1)
            {
                Debug.LogError("FUCK!");

            }
            //Run check if we are in the correct subbiome
            if (weight != 0)
            {
                //If not generating in patches
                if (!generateInPatches)
                {
                    /*Run a random check to see if grass spawns here
                    Tile biome map is so that grass fades out on the edges of tiles */
                    if (tileBiomeMap[i] != 0 && randomInt.NextUInt(0, 1001) < ((spawnChance * tileBiomeMap[i]) * weight))
                    {
                        densityNum = detailDensity;

                    }
                }
                //If we are generating in patches
                else
                {
                    //Make sure the noisemap is generating here   
                    if (patchNoisemap[i] > patchCutoff)
                    {
                        //Check if we are close enough to patchCutoff to fall within patchFadeMargin, and store a float to hold that value, which defaults to 1
                        float fadeAmnt = 1;
                        if (patchNoisemap[i] - patchCutoff < patchFadeMargin)
                        {
                            fadeAmnt = (patchNoisemap[i] - patchCutoff) / patchFadeMargin;
                        }
                        /*Run the check to see if we generate, but also factor in patchMarginFade.
                        Tile biome map is so that grass fades out on the edges of tiles */
                        if (tileBiomeMap[i] != 0 && randomInt.NextUInt(0, 1001) < ((spawnChance * tileBiomeMap[i] * fadeAmnt) * weight))
                        {
                            densityNum = detailDensity;
                        }
                    }
                }
            }
            collapsedDetailMap[i] = densityNum;

        }


    }


    //Method that generates a terrain tile
    private IEnumerator generateTerrainTile(Terrain terrain, Transform terrainParent, int tileNumber, int2 tileOffset1x1)
    {
        #region References and Setup

        // * * * * * Texture generation references * * * * * \\

        //Vector2 to hold the offset of the tile so it generates at the correct spot
        int2 tileOffset;
        //Array size is equal to the demensions of the splatmap
        int arraySize = 1025 * 1025;
        mapDemensions = new int2(1025, 1025);
        float offsetScale = 1.024f;
        //Create nativearrays and float[,]s to hold the values for our floats
        NativeArray<float> tempNative;
        NativeArray<float> humidNative;
        float[,] temperatureMap;
        float[,] humidityMap;
        //List of NativeArrays that hold the splatmaps of each biome
        NativeArray<float>[,] biomeNativeArrays = new NativeArray<float>[maxObjectWeights.GetLength(0), biomeStorage.biomes.Length];
        //Set aside memory for these arrays
        tempNative = new NativeArray<float>(arraySize, Allocator.Persistent);
        humidNative = new NativeArray<float>(arraySize, Allocator.Persistent);
        //Create a 3d array to hold the biome texture values for this map
        List<float[,,]> splatMap = new List<float[,,]>();
        //Setup for generating the terrain in stripts
        List<float[,,]> stripList = new List<float[,,]>();
        int remainder = (1025 % currentSpeedSettings.splatmapStrips);
        int pixelsPerStrip = (1025 - remainder) / currentSpeedSettings.splatmapStrips;
        int pixelsSetPerStrip = 0;
        //List to hold the layers the tile actually has
        List<TerrainLayer> tileLayers = new List<TerrainLayer>();
        NativeArray<float3> voronoiMap = new NativeArray<float3>(1025 * 1025, Allocator.Persistent);
        // * * * * * Detail generation references * * * * * \\

        //Reference to the GPUI detail prototype
        GPUInstancerDetailPrototype currentDetailPrototype = null;
        //List of detail prototypes for this terrain
        List<DetailPrototype> terrainDetailPrototypes = new List<DetailPrototype>();


        // * * * * * Object generation references * * * * * \\
        //Array storing the heightmap that we want to change
        float[,] terrainObjectHeightmap = tDatas[tileNumber].GetHeights(0, 0, terrainHeightmapLength, terrainHeightmapLength);
        //Array that gets filled in to block object spawns. 0 = object can spawn, 1 = object is blocked;
        float[,] objectBlockerHeightmap = new float[terrainHeightmapLength, terrainHeightmapLength];
        //Int to store how far the generation loop should progress before spawning again
        int density = 8;
        //Changing bool to determine if a picked generation spot is blended with another biome
        bool doBlend = false;
        //How much weight a biome has when being blended
        float blendAmount = 0;
        //Demensions of the biome density map
        int2 densityMapDimensions = new int2(257, 257);
        //Define an array to store the current list of spawnable objects
        ObjectScriptable[] tileObjects = { };
        //Int to keep track of the biome number selected. -1 So it throws an error if something doesn't work.
        int biomeIndex = -1;
        //2D float array of a tiles density
        float[,] tileObjectDensityMap = null;
        //Custom material for this terrains blending
        Material customTerrainBlendMaterial = new Material(terrainBlendMaterial);
        customTerrainBlendMaterial.CopyPropertiesFromMaterial(terrainBlendMaterial);
        //Storage of biome density
        int biomeDensityAmnt = 0;
        //Max weight for biome
        int maxWeightAmnt = 0;
        //Value to store the normals of the terrain
        Vector3 spotNormal = new Vector3();
        // * * * * * Before Generation Setup * * * * * \\

        //Log this tile in the terrain tile biome list
        biomesPerTerrainTile.Add(new int[maxObjectWeights.GetLength(0), biomeStorage.biomes.Length]);
        #endregion

        #region large Buildings
        //Go through and spawn large buildings if we are the host

        bool hasBuilding = false;
        int buildingIndex = 0;
        for (int i = 0; i < buildings.Count; i++)
        {
            if (buildings[i].terrainTileIndex == tileNumber)
            {
                hasBuilding = true;
                buildingIndex = i;
            }
        }
        //Loop through each building
        if (hasBuilding)
        {
            //Define a reference to the BuildingData
            BuildingData buildingData = buildings[buildingIndex];
            //Define radius and center values
            int radius = buildingData.GetBuildingRadius();
            Vector3 center = buildingData.centerPosition;
            //Define a Vector2 containing the X and Z components of the center for use in distance caluclations
            Vector2 centerVec2 = new Vector2(center.x, center.z);
            //What height the terrain should target its smoothing towards
            float targetHeight = center.y / terrainHeight;
            //Loop through each point in a square around this building to create the smooth distance
            float objectBlockRadius = buildingData.GetObjectBlockRadius();
            for (int y = (int)center.z - radius; y < center.z + radius; y++)
            {
                for (int x = (int)center.x - radius; x < center.x + radius; x++)
                {
                    //Check if we fall inside the circle we want, and if so set the height
                    float distanceFromCenter = Vector2.Distance(centerVec2, new Vector2(x, y));
                    float heightToSet = targetHeight;
                    if (distanceFromCenter <= radius)
                    {
                        //Check if we are within a spot where we should be fading
                        if (radius - distanceFromCenter < buildingData.GetFadeMargin())
                        {
                            float offsetDistance = (terrainObjectHeightmap[y, x] - targetHeight);
                            float distanceFromMiddleSLopeOffset = Mathf.Abs(distanceFromCenter - (buildingData.GetFadeMargin() / 2)) / buildingData.GetFadeMargin();
                            heightToSet = targetHeight + (offsetDistance * (1 - ((radius - distanceFromCenter) / buildingData.GetFadeMargin())));

                        }




                        terrainObjectHeightmap[y, x] = heightToSet + Random.Range(0f, 0.0002f); ;
                        if (distanceFromCenter < objectBlockRadius)
                        {
                            objectBlockerHeightmap[y, x] = 1;
                        }
                    }
                }
            }
            //Set the position to place this object, and modify it by the objects offset
            Vector3 placePosition = new Vector3((center.x / 1.024f) + terrains[tileNumber].transform.position.x, center.y, (center.z / 1.024f) + terrains[tileNumber].transform.position.z);
            placePosition += buildingData.spawnOffset;
            //Spawn the object, and set the scale
            GameObject c = Instantiate(buildingData.GetObjectModel(), placePosition, Quaternion.Euler(buildingData.spawnedRotationOffset));
            c.transform.localScale = buildingData.spawnedScale;
            //Create holes
            if (buildingData.hasHoles)
            {
                GenerationHoleCreator holeC = c.GetComponent<GenerationHoleCreator>();
                for (int n = 0; n < holeC.holes.Length; n++)
                {
                    SquareHole hole = holeC.holes[n];
                    Vector3 terrainPosition = terrains[tileNumber].transform.position;
                    Vector3 pos1TH = hole.PositionToTerrainHeightmap(terrainPosition, hole.pos1.position);
                    Vector3 pos2TH = hole.PositionToTerrainHeightmap(terrainPosition, hole.pos2.position);
                    bool[,] holeMap = hole.CreateHoleArray(pos1TH, pos2TH);
                    Debug.Log($"{holeMap.GetLength(0)}, {holeMap.GetLength(1)} is array dimensions, setting them at {pos1TH.x}, {pos1TH.z}");
                    tDatas[tileNumber].SetHoles((int)pos1TH.x, (int)pos1TH.z, holeMap);
                }
            }
            //Remove Details
            if (terrains[tileNumber].gameObject.activeInHierarchy)
            {
                c.transform.GetChild(2).gameObject.AddComponent<GPUInstancerDelayDetailRemoverUntilSpawn>();
            }
            else
            {
                GPUInstancerDelayDetailRemoverUntilTerrainActivation rem = terrains[tileNumber].gameObject.AddComponent<GPUInstancerDelayDetailRemoverUntilTerrainActivation>();
                rem.SetTargetObject(c.transform.GetChild(2));
            }
            //Set layout groups - layout parent should be the 4th object underneath the building (Index of 3)
            Transform layoutParent = c.transform.GetChild(3);
            for (int i = 0; i < buildingData.layouts.Length; i++)
            {
                //Set layout string and prefab array of the layout
                layoutParent.GetChild(i).GetComponent<RandomLayoutCreator>().SetLayoutValues(buildingData.GetLayoutScriptable().layoutGroups[i].layoutStrings[buildingData.layouts[i]], buildingData.GetLayoutScriptable().layoutGroups[i].prefabs);
                //Create the layout
                layoutParent.GetChild(i).GetComponent<RandomLayoutCreator>().CreateLayout();
            }

        }

        //Set heights
        tDatas[tileNumber].SetHeights(0, 0, terrainObjectHeightmap);

        #endregion

        #region Texturing
        //Get the offset of this tile in the world
        tileOffset = new int2();
        tileOffset.x = Mathf.RoundToInt(terrainParent.transform.position.x * offsetScale);
        tileOffset.y = Mathf.RoundToInt(terrainParent.transform.position.z * offsetScale);
        //Generate a temperature map for that tile and add it to the list
        temperatureMap = TerrainNoise.GenerateNoiseMap(mapDemensions, (uint)tempMapSettings.seed, tempMapSettings.scale, tempMapSettings.octaves, tempMapSettings.persistance, tempMapSettings.lacunarity, tileOffset);
        //Wait in order to not completely stall the main thread whilst all complete
        yield return new WaitForSeconds(0.03f);
        //Generate a humidity map for that tile and add it to the list
        humidityMap = TerrainNoise.GenerateNoiseMap(mapDemensions, (uint)humidMapSettings.seed, humidMapSettings.scale, humidMapSettings.octaves, humidMapSettings.persistance, humidMapSettings.lacunarity, tileOffset);
        yield return new WaitForSeconds(0.03f);
        //Generate the voronoi map for the tile
        StartCoroutine(GenerateTileVoronoiMap(1025, 2, (uint)seed, tileOffset1x1, voronoiMap, 50));
        //Wait until the map is finished generating
        yield return new WaitUntil(() => voronoiFinishCompletion == true);
        voronoiFinishCompletion = false;
        //Iterate through the noisemaps and feed that value into the nativeArrays so it can travel between threads
        for (int y = 0; y < 1025; y++)
        {
            for (int x = 0; x < 1025; x++)
            {
                tempNative[(y * 1025) + x] = temperatureMap[x, y];
                humidNative[(y * 1025) + x] = humidityMap[x, y];



            }
            if (y % currentSpeedSettings.maxIterationsPerFrame == 0)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        //Loop through each biome
        for (int m = 0; m < biomeStorage.biomes.Length; m++)
        {
            splatMap.Add(new float[1025, 1025, biomeStorage.biomes.Length]);
            for (int subNumber = 0; subNumber < biomeStorage.biomes[m].subBiomes.Length; subNumber++)
            {
                //Add a new nativeArray
                biomeNativeArrays[subNumber, m] = new NativeArray<float>(arraySize, Allocator.Persistent);
                //Setup the job
                generateSplatJob = new generateSplat()
                {
                    positions = biomeNativeArrays[subNumber, m],
                    // tempPositions = tempNative,
                    tempPositions = tempNative,
                    humPositions = humidNative,
                    subBiomeNative = voronoiMap,
                    minHum = biomeStorage.biomes[m].minHum,
                    maxHum = biomeStorage.biomes[m].maxHum,
                    minTemp = biomeStorage.biomes[m].minTemp,
                    maxTemp = biomeStorage.biomes[m].maxTemp,
                    minSub = biomeStorage.biomes[m].subBiomes[subNumber].minCutoff,
                    maxSub = biomeStorage.biomes[m].subBiomes[subNumber].maxCutoff
                };
                //Schedule the job
                generateSplatHandle = generateSplatJob.Schedule(biomeNativeArrays[subNumber, m].Length, 64);
                //Pause to not stutter frames
                //yield return new WaitUntil(() => generateSplatHandle.IsCompleted == true);
                generateSplatHandle.Complete();
                yield return new WaitForSecondsRealtime(0.05f);
                //Set which biomes have which tile array
                for (int y = 0; y < 1025; y++)
                {
                    for (int x = 0; x < 1025; x++)
                    {
                        if (generateSplatJob.positions[(y * 1025) + x] != 0)
                        {
                            biomesPerTerrainTile[tileNumber][subNumber, m] = 1;
                            tileLayers.Add(biomeStorage.biomes[m].subBiomes[subNumber].biomeLayer);
                            break;
                        }

                    }
                    if (biomesPerTerrainTile[tileNumber][subNumber, m] == 1)
                    {
                        break;

                    }
                    if (y % currentSpeedSettings.maxIterationsPerFrame == 0)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }

                biomeNativeArrays[subNumber, m] = generateSplatJob.positions;


                yield return new WaitForEndOfFrame();

            }

        }


        //PixelOffset is the index offset of the strip when compared to the main map
        int pixelOffset = 0;
        //Create a new float[,,] for each strip we will use
        for (int stripNumber = 0; stripNumber < currentSpeedSettings.splatmapStrips + 1; stripNumber++)
        {

            //Define the strip width dependant on iteration
            if (stripNumber == currentSpeedSettings.splatmapStrips)
            {
                pixelsSetPerStrip = remainder;
            }
            else
            {
                pixelsSetPerStrip = pixelsPerStrip;
            }
            //Add to list, with a Z width of how many layers actually exist
            float[,,] strip = new float[pixelsSetPerStrip, 1025, tileLayers.Count];
            stripList.Add(strip);

        }

        //Loop through biomes again to set strips this time
        //Copy float values to terrain array, and the list of array strips to set the splatmaps as
        int tempBIndex = 0;
        for (int stripNumber = 0; stripNumber < currentSpeedSettings.splatmapStrips + 1; stripNumber++)
        {



            //Define the strip width dependant on iteration
            if (stripNumber == currentSpeedSettings.splatmapStrips)
            {
                pixelsSetPerStrip = remainder;


            }
            else
            {
                pixelsSetPerStrip = pixelsPerStrip;

            }

            //Set the map
            float splatAmount = 0;
            for (int y = 0; y < pixelsSetPerStrip; y++)
            {

                for (int x = 0; x < 1025; x++)
                {

                    tempBIndex = 0;
                    for (int m = 0; m < biomeStorage.biomes.Length; m++)
                    {
                        for (int j = 0; j < biomeStorage.biomes[m].subBiomes.Length; j++)
                        {
                            if (biomesPerTerrainTile[tileNumber][j, m] == 1)
                            {
                                stripList[stripNumber][y, x, tempBIndex] = biomeNativeArrays[j, m][((y + pixelOffset) * 1025) + x];
                                splatMap[m][y + pixelOffset, x, j] = biomeNativeArrays[j, m][((y + pixelOffset) * 1025) + x];
                                tempBIndex += 1;
                            }


                        }

                    }

                }
                if (y % currentSpeedSettings.maxIterationsPerFrame == 0)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            pixelOffset += pixelsSetPerStrip;
            if (stripNumber % (currentSpeedSettings.maxLoopBeforeWait) == 0)
            {
                yield return new WaitForEndOfFrame();
            }

        }




        //Update terrain tile
        tDatas[tileNumber].terrainLayers = tileLayers.ToArray();
        int yOffset = 0;
        for (int stripNumber = 0; stripNumber < currentSpeedSettings.splatmapStrips + 1; stripNumber++)
        {
            tDatas[tileNumber].SetAlphamaps(0, yOffset, stripList[stripNumber]);
            yOffset += pixelsPerStrip;
            if (stripNumber % currentSpeedSettings.maxLoopBeforeWait == 0)
            {
                yield return new WaitForEndOfFrame();
            }
        }







        //Dispose of NativeArrays

        tempNative.Dispose();
        humidNative.Dispose();

        #endregion
        /*
        for (int y = 0; y < biomeStorage.biomes.Length; y++)
        {
            for (int x = 0; x < biomeStorage.biomes[y].subBiomes.Length; x++)
            {
                if (biomeNativeArrays[x, y] != null)
                {
                    biomeNativeArrays[x, y].Dispose();
                }
            }
        }
        voronoiMap.Dispose();
        /genScreen.genAmount += 1;
        genScreen.RefreshGenerationUI();
        moveNext = true;

        yield break;
        */
        #region Details
        //Store an array of what detail prototypes are used and their settings
        List<int> crossQuadCounts = new List<int>();
        //Get a list of all the detail prototypes we will use for this terrain
        for (int i = 0; i < biomeStorage.biomes.Length; i++)
        {
            for (int subBiomeNumber = 0; subBiomeNumber < biomeStorage.biomes[i].subBiomes.Length; subBiomeNumber++)
            {
                //Skip this biome if it doesn't exist on the tile
                if (biomesPerTerrainTile[tileNumber][subBiomeNumber, i] == 0)
                {
                    continue;
                }

                //If it does, add each detail in this biome to the list of details
                for (int j = 0; j < biomeStorage.biomes[i].subBiomes[subBiomeNumber].details.Length; j++)
                {
                    //Fill in settings

                    DetailPrototype proto = new DetailPrototype();

                    DetailObjectScriptable settings = biomeStorage.biomes[i].subBiomes[subBiomeNumber].details[j];
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
                    proto.useInstancing = false;

                    if (proto.usePrototypeMesh)
                    {
                        proto.prototype = settings.prototypeMesh;
                        crossQuadCounts.Add(0);
                        proto.renderMode = DetailRenderMode.VertexLit;


                    }
                    else
                    {
                        proto.renderMode = DetailRenderMode.Grass;
                        proto.prototypeTexture = settings.prototypeTexture;
                        crossQuadCounts.Add(settings.quadCount);

                    }

                    terrainDetailPrototypes.Add(proto);
                }
            }
        }
        //GOOD TILL HERE
        //Assign detail prototypes to this terrains as an array
        DetailPrototype[] prototypes = new DetailPrototype[terrainDetailPrototypes.Count];
        for (int k = 0; k < terrainDetailPrototypes.Count; k++)
        {
            prototypes[k] = terrainDetailPrototypes[k];
            if (!prototypes[k].Validate())
            {
                Debug.LogError("Prototype was not valid!");
            }
        }

        tDatas[tileNumber].detailPrototypes = prototypes;




        //Int to store the offset of this details map
        int detailIndexOffset = 0;

        //Generate the detail maps for each of the details
        for (int i = 0; i < biomeStorage.biomes.Length; i++)
        {
            for (int subBiomeNumber = 0; subBiomeNumber < biomeStorage.biomes[i].subBiomes.Length; subBiomeNumber++)
            {
                //Check to see if this terrain tile has this biome, if it doesn't, skip it
                if (biomesPerTerrainTile[tileNumber][subBiomeNumber, i] == 0)
                {
                    //Biome doesn't exist on this tile, skip and don't generate a map for it
                    continue;
                }


                //If the biome does exist, create a job to set its details

                //Create a new random struct for placement
                Unity.Mathematics.Random randomInt = new Unity.Mathematics.Random();
                randomInt.InitState((uint)System.DateTime.Now.Ticks);
                //Generate a detail map for each detail layer

                for (int detailIndex = 0; detailIndex < biomeStorage.biomes[i].subBiomes[subBiomeNumber].details.Length; detailIndex++)
                {
                    //Define a NativeArray that will hold the collapsed detail map
                    //1025x1025 even though detail is 1024x1024 to make room for splatmap
                    NativeArray<int> collapsedDetailMap = new NativeArray<int>((tDatas[tileNumber].detailHeight + 1) * (tDatas[tileNumber].detailWidth + 1), Allocator.Persistent);
                    //Get what settings are for this detail 
                    DetailObjectScriptable settings = biomeStorage.biomes[i].subBiomes[subBiomeNumber].details[detailIndex];
                    //Define an empty nativeArray to use for the patch noisemap
                    NativeArray<float> patchNoisemap = new NativeArray<float>(1, Allocator.Persistent);
                    //If the detail is set to generate in patches, generate a noisemap to use



                    if (settings.generationType == DetailObjectScriptable.GenerationType.Patches)
                    {
                        patchNoisemap.Dispose();
                        patchNoisemap = new NativeArray<float>(1025 * 1025, Allocator.Persistent);
                        int2 mapDems = new int2(settings.noisemapSettings.mapWidth, settings.noisemapSettings.mapHeight);
                        int2 detailTileOffset = new int2();
                        detailTileOffset.x = Mathf.RoundToInt(terrainParents[tileNumber].transform.position.x * offsetScale) / (1024 / settings.noisemapSettings.mapWidth);
                        detailTileOffset.y = Mathf.RoundToInt(terrainParents[tileNumber].transform.position.z * offsetScale) / (1024 / settings.noisemapSettings.mapHeight);
                        float[,] patchMap2D = TerrainNoise.GenerateNoiseMap(mapDems, (uint)settings.noisemapSettings.seed, settings.noisemapSettings.scale, settings.noisemapSettings.octaves, settings.noisemapSettings.persistance, settings.noisemapSettings.lacunarity, detailTileOffset);
                        //Convert to nativeArray
                        int pixelsPerTileX = 4;
                        int pixelsPerTileY = 4;
                        for (int y = 0; y < settings.noisemapSettings.mapHeight; y++)
                        {
                            for (int x = 0; x < settings.noisemapSettings.mapWidth; x++)
                            {
                                if (x == settings.noisemapSettings.mapWidth)
                                {
                                    pixelsPerTileX = 5;
                                }
                                if (y == settings.noisemapSettings.mapHeight)
                                {
                                    pixelsPerTileY = 5;
                                }
                                for (int k = 0; k < pixelsPerTileY; k++)
                                {
                                    for (int p = 0; p < pixelsPerTileX; p++)
                                    {
                                        patchNoisemap[(((y * 4) + k) * 1024) + ((x * 4) + p)] = patchMap2D[x, y];
                                    }
                                }


                            }
                        }
                    }
                    //Stop before job

                    generateDetailsJob = new generateDetails
                    {
                        collapsedDetailMap = collapsedDetailMap,
                        tileBiomeMap = biomeNativeArrays[subBiomeNumber, i],
                        subBiomeNative = voronoiMap,
                        detailDensity = settings.detailDensity,
                        spawnChance = settings.spawnChance,
                        randomInt = randomInt,
                        minSub = biomeStorage.biomes[i].subBiomes[subBiomeNumber].minCutoff,
                        maxSub = biomeStorage.biomes[i].subBiomes[subBiomeNumber].maxCutoff,
                        generateInPatches = settings.generationType == DetailObjectScriptable.GenerationType.Patches,
                        patchCutoff = settings.patchCutoff,
                        patchNoisemap = patchNoisemap,
                        patchFadeMargin = settings.patchFadeMargin

                    };
                    //Schedule the job
                    //1025x1025 even though detail is 1024x1024 to make room for splatmap
                    generateDetailsHandle = generateDetailsJob.Schedule((tDatas[tileNumber].detailHeight + 1) * (tDatas[tileNumber].detailWidth + 1), 128);
                    //Wait until the job is completed
                    // yield return new WaitUntil(() => generateDetailsHandle.IsCompleted == true);
                    //Call the job to force completion because Unity throws an error if an attempt is made to access te jobs values without a Complete() call
                    generateDetailsHandle.Complete();
                    //Wait till the end of the frame
                    yield return new WaitForEndOfFrame();

                    //Define a temporary detail map
                    int[,] DetailMap = new int[tDatas[tileNumber].detailHeight, tDatas[tileNumber].detailWidth];
                    //Construct the NativeArray into a 2D map
                    //1025 because res of splatmap
                    for (int y = 0; y < 1024; y++)
                    {

                        for (int x = 0; x < 1024; x++)
                        {


                            // DetailMap[y,x] = generateDetailsJob.collapsedDetailMap[(y * tDatas[tileNumber].detailHeight) + x];
                            DetailMap[y, x] = generateDetailsJob.collapsedDetailMap[(y * 1025) + x];

                        }
                        if (y % currentSpeedSettings.maxIterationsPerFrame == 0)
                        {
                            yield return new WaitForEndOfFrame();
                        }
                    }

                    //Set the detail layer

                    tDatas[tileNumber].SetDetailLayer(0, 0, detailIndex + detailIndexOffset, DetailMap);


                    collapsedDetailMap.Dispose();
                    patchNoisemap.Dispose();
                }
                //After doing all the details in a biome, increase the detailIndexOffset
                detailIndexOffset += biomeStorage.biomes[i].subBiomes[subBiomeNumber].details.Length;
            }

        }
        yield return new WaitForEndOfFrame();

        //Pause till the end of the frame
        yield return new WaitForEndOfFrame();
        //Dispose of the biomes maps
        for (int y = 0; y < biomeStorage.biomes.Length; y++)
        {
            for (int x = 0; x < biomeStorage.biomes[y].subBiomes.Length; x++)
            {
                if (biomeNativeArrays[x, y] != null)
                {
                    biomeNativeArrays[x, y].Dispose();
                }
            }
        }

        if (!terrains[tileNumber].gameObject.activeInHierarchy)
        {
            terrains[tileNumber].gameObject.AddComponent<GPUInstancerSetupOnTerrainActivation>();
            terrains[tileNumber].GetComponent<GPUInstancerSetupOnTerrainActivation>().crossQuadCounts = crossQuadCounts;
        }
        else
        {
            terrains[tileNumber].gameObject.AddComponent<GPUInstancerDelayCameraSet>();
            terrains[tileNumber].GetComponent<GPUInstancerDelayCameraSet>().crossQuadCounts = crossQuadCounts;
        }

        yield return new WaitForEndOfFrame();

        #endregion

        #region large objects and buildings

        #endregion

        #region Objects
        /* Only run this if we are the host/server */
        if (NetworkManager.Singleton.IsHost)
        {
            tileOffset.x = Mathf.RoundToInt(terrainParent.transform.position.x * offsetScale) / 4;
            tileOffset.y = Mathf.RoundToInt(terrainParent.transform.position.z * offsetScale) / 4;
            //Store the location of this tile
            Vector3 updatedTilePosition = terrain.transform.position;
            //Generate a noisemap for the density of this tile
            tileObjectDensityMap = TerrainNoise.GenerateNoiseMap(densityMapDimensions, (uint)biomeDensityMapSettings.seed, biomeDensityMapSettings.scale, biomeDensityMapSettings.octaves, biomeDensityMapSettings.persistance, biomeDensityMapSettings.lacunarity, tileOffset);


            //Iterate through the tile, but increment by the set density above
            for (int y = 0; y < tDatas[tileNumber].heightmapResolution; y += density)
            {
                for (int x = 0; x < tDatas[tileNumber].heightmapResolution; x += density)
                {
                    //Return if blocked at this spot
                    if (objectBlockerHeightmap[y, x] == 1) continue;

                    //Default doBlend to false
                    doBlend = false;
                    //Set biome density to default (0)
                    biomeDensityAmnt = 0;
                    //Default the blendAmount to 0
                    blendAmount = 0;
                    //Set the maxObjectWeight to 0
                    maxWeightAmnt = 0;
                    //Loop through biomes to check which ones exist

                    for (int i = 0; i < biomeStorage.biomes.Length; i++)
                    {
                        for (int subBiomeNumber = 0; subBiomeNumber < biomeStorage.biomes[i].subBiomes.Length; subBiomeNumber++)
                        {


                            //If the biome doesn't exist on this tile, or the biome has no objects, skip
                            if (biomesPerTerrainTile[tileNumber][subBiomeNumber, i] == 0 || biomeStorage.biomes[i].subBiomes[subBiomeNumber].biomeObjects.Length == 0)
                            {
                                //Do nothing

                            }
                            else
                            {

                                //Check biome splat map at this position
                                if (splatMap[i][y, x, subBiomeNumber] != 0)
                                {
                                    if (splatMap[i][y, x, subBiomeNumber] == 1)
                                    {

                                        tileObjects = biomeStorage.biomes[i].subBiomes[subBiomeNumber].biomeObjects;
                                        biomeIndex = i;
                                        biomeDensityAmnt = biomeStorage.biomes[i].subBiomes[subBiomeNumber].biomeDensity;
                                        //Set max weight amount
                                        maxWeightAmnt = maxObjectWeights[subBiomeNumber, biomeIndex];
                                    }
                                    else
                                    {
                                        //The biome is blended, add its weight to the blend array and blendAmount, and mark that we must blend
                                        doBlend = true;
                                        blendAmount += splatMap[i][y, x, subBiomeNumber];

                                    }

                                }
                            }


                        }
                    }

                    //If blending, decide a biome that will take prevelance at this spot (weighted by biome splatmap #)
                    if (doBlend)
                    {
                        float thisBlend = 0;
                        float blendNumber = Random.Range(0, blendAmount);
                        for (int i = 0; i < biomeStorage.biomes.Length; i++)
                        {
                            for (int subBiomeNumber = 0; subBiomeNumber < biomeStorage.biomes[i].subBiomes.Length; subBiomeNumber++)
                            {

                                if (splatMap[i][y, x, subBiomeNumber] + thisBlend > blendNumber)
                                {
                                    tileObjects = biomeStorage.biomes[i].subBiomes[subBiomeNumber].biomeObjects;
                                    biomeIndex = i;
                                    biomeDensityAmnt = biomeStorage.biomes[i].subBiomes[subBiomeNumber].biomeDensity;
                                    maxWeightAmnt = maxObjectWeights[subBiomeNumber, biomeIndex];
                                }
                                else
                                {
                                    thisBlend += splatMap[i][y, x, subBiomeNumber];
                                    break;
                                }
                            }

                        }
                    }
                    //As long as there is actually a biome, generate
                    if (biomeIndex != -1)
                    {

                        //Define random chance for this object to appear
                        int randomChance = Random.Range(0, 1000);
                        /* The odds of this object appearing depend on the chance for its category defined above.
                         * This number is multiplied by the value of the biome density map + 0.25 (normalized between 0 and 1, and +.25 is to still spawn at low density)
                           to decide if an object is actually spawned at this spot. The biome density X and Y are divided by 4 as it samples a smaller map. */
                        if (randomChance < biomeDensityAmnt * Mathf.Lerp(0, 1.25f, (tileObjectDensityMap[x / 4, y / 4] + 0.25f)))
                        {
                            //Pick a random object from the list depending on weights
                            int randomWeight = Random.Range(0, maxWeightAmnt);

                            //Store the object we want
                            ObjectScriptable selectedOb = null;
                            /* Object is found by: First picking a random number between 0, and the weight of all objects in this biome added up.
                             * Then, for each object it tests if that random number is smaller than the tested objects weight PLUS all the objects that failed
                             * selection before it          */
                            //Value of all the failed objects before the one being tested, added up
                            int priorWeights = 0;
                            //Iterate through list until we find that point.
                            for (int i = 0; i < tileObjects.Length; i++)
                            {
                                //Check if the random value is smaller than this weight + the failed weights
                                if (priorWeights + tileObjects[i].weight > randomWeight)
                                {
                                    //If true, this object is the index we want
                                    selectedOb = tileObjects[i];
                                    break;
                                }
                                else
                                {
                                    //If false, the tested object failed to reach the random number, add the failed objects weight to priorWeights
                                    priorWeights += tileObjects[i].weight;

                                }
                            }

                            //Log an error if somehow an object failed to be selected
                            if (selectedOb == null)
                            {
                            }
                            else
                            {

                                //Store a reference to the tiles normals for proper placement
                                //Set the position of where the object should spawn
                                Vector3 spawnedPosition = new Vector3(updatedTilePosition.x + (x / 1.025f), tDatas[tileNumber].GetHeight(x, y) + selectedOb.yOffset, updatedTilePosition.z + (y / 1.025f));
                                //If offsetHeightByNormals is checked, change spawn height depending on normals so roots and things don't stick out
                                if (selectedOb.offsetHeightByNormals)
                                {
                                    //100 is the multiple so it doesn't stick out of the terrain on large slopes
                                    // spawnedPosition.y -= (1 - Mathf.Abs(spotNormal.y)) * 100;
                                }

                                /* * * Set Rotation of the object and instantiate * * */
                                Quaternion rotQuat = new Quaternion();
                                if (selectedOb.alignNormals)
                                {

                                    RaycastHit previewHit;

                                    if (Physics.Raycast(new Ray(new Vector3(spawnedPosition.x, spawnedPosition.y + 300, spawnedPosition.z), Vector3.down), out previewHit, 1000f))
                                    {
                                        spotNormal = previewHit.normal;
                                    }

                                }
                                rotQuat = rotQuat * Quaternion.Euler(0, Random.Range(0, 360), 0);

                                //Spawn the object
                                GameObject spawnedObject = Instantiate(selectedOb.prefab, spawnedPosition, rotQuat);
                                //Spawn the object on the server
                                spawnedObject.GetComponent<Unity.Netcode.NetworkObject>().Spawn(true);
                                if (selectedOb.terrainBlending)
                                {

                                    spawnedObject.GetComponent<Renderer>().material = customTerrainBlendMaterial;
                                }
                                //Select a random point between 0 and 1 for the scale of the object
                                float randomAmnt = Random.Range(0f, 1f);
                                //Evaluate that point on the objects animation curve to get the scale of the object
                                float newObjectScale = selectedOb.scale.Evaluate(randomAmnt);
                                spawnedObject.transform.localScale = new Vector3(newObjectScale * selectedOb.scaleMultiplier.x, newObjectScale * selectedOb.scaleMultiplier.y, newObjectScale * selectedOb.scaleMultiplier.z);
                                //Name the object
                                spawnedObject.name = selectedOb.name;
                            }

                        }


                    }
                    else
                    {

                    }
                    //Timeout for a frame if we reach a certain amount of values, timeout is less often than others as density modifier means less values are computed

                    if (y % (currentSpeedSettings.maxIterationsPerFrame * 3) == 0)
                    {
                        //Later maybe have options in settings to have faster or slower gen depending on how you want your performance
                        yield return new WaitForEndOfFrame();
                    }
                }
            }

            //Update the InTerra_API with this new terrain data
            InTerra.InTerra_Data.UpdateTerrainData(true);
        }
        #endregion
        voronoiMap.Dispose();
        genScreen.genAmount += 1;
        genScreen.RefreshGenerationUI();
        moveNext = true;

        yield break;
    }

    //Method that gets a spot for every building we are going to create. 
    private void GetBuildingSpotList()
    {

        List<int> attemptedTerrains = new List<int>();
        //int randomTerrain = Random.Range(0, terrains.Count + 1);
        int randomTerrain = Random.Range(0, 4);
        buildings.Add(new BuildingData
        {
            type = BuildingData.BuildingType.BrokenColleseum,
            centerPosition = new Vector3(),
            terrainTileIndex = randomTerrain,
            spawnOffset = new Vector3(0, 3.1f, 0),
            spawnedScale = new Vector3(2, 2, 2),
            modelIndex = 0,
            hasHoles = true,
            spawnedRotationOffset = new Vector3(0, 18.8f, 0)
        });
        //Fill the layout array with random layouts
        RandomLayoutScriptable buildingScriptable = buildings[0].GetLayoutScriptable();
        int[] randomLayoutIndexs = new int[buildingScriptable.layoutGroups.Length];
        for (int i = 0; i < randomLayoutIndexs.Length; i++)
        {
            randomLayoutIndexs[i] = Random.Range(0, buildingScriptable.layoutGroups[i].layoutStrings.Length);
        }
        buildings[0].layouts = randomLayoutIndexs;
        //Choose a random terrain for the coleseum
        int buildingRadius = buildings[0].GetBuildingRadius();
        //Bool to store if we have found a spot to place this building yet. We need flat ground.
        bool foundSpot = false;
        int cnt = 0;
        while (foundSpot == false)
        {
            Vector3 middleSpot = new Vector3(Random.Range(0 + buildingRadius, terrainHeightmapLength - buildingRadius), 0, Random.Range(0 + buildingRadius, terrainHeightmapLength - buildingRadius));
            Vector3 terrainNormal = tDatas[randomTerrain].GetInterpolatedNormal(middleSpot.x / terrainHeightmapLength, middleSpot.z / terrainHeightmapLength);
            if (terrainNormal.x < 0.2f && terrainNormal.x > -0.2f && terrainNormal.z < 0.2f && terrainNormal.z > -0.2f)
            {
                float buildingMiddleHeight = tDatas[randomTerrain].GetInterpolatedHeight(middleSpot.x / 1024, middleSpot.z / 1024);
                Debug.Log($"Height at middle spot is {buildingMiddleHeight}");
                middleSpot.y = buildingMiddleHeight;
                buildings[0].centerPosition = middleSpot;
                buildings[0].terrainTileIndex = randomTerrain;
                foundSpot = true;
            }
            else
            {
                cnt += 1;

                if (cnt > 100)
                {
                    while (attemptedTerrains.Contains(randomTerrain))
                    {
                        randomTerrain = Random.Range(0, 4);
                    }
                    cnt = 0;
                }
            }
        }
        //Define an array to hold the BuildingDataNetworkStructs and pass it the data from the class list
        BuildingDataNetworkStruct[] buildingStructs = new BuildingDataNetworkStruct[buildings.Count];
        for (int i = 0; i < buildings.Count; i++)
        {
            buildingStructs[i] = buildings[i].ToStruct();
        }
        //After creating the struct array, call the clientRPC to sync the data and start the client generation
        // SetBuildingListClientRPC(buildingStructs);
        // SetBuildingListClientRpc();
        PlayerNetwork.instance.SetWorldgenBuildingListClientRpc(buildingStructs);
    }
    //Voronoi map generation method that uses a coroutine in order to generate over multiple frames.
    private IEnumerator GenerateTileVoronoiMap(int dimensions, int regionAmount, uint seed, int2 offset, NativeArray<float3> voronoiNative, int blendPixelAmount)
    {

        //Set the offset to a different value if zero
        if (offset.x == 0)
        {
            offset.x = 999999;
        }
        if (offset.y == 0)
        {
            offset.y = 999999;
        }
        //Define 2D point array - needs to be 2 larger than the tile width in order to accomidate tiling with surrounding tiles
        float2[,] points = new float2[(regionAmount + 2), (regionAmount + 2)];
        //Define 2D weights array - needs to be 2 larger than the tile width in order to accomidate tiling with surrounding tiles
        float[,] weights = new float[(regionAmount + 2), (regionAmount + 2)];
        //Define random for the X and Y values of the main tile
        Unity.Mathematics.Random randomX = new Unity.Mathematics.Random((uint)offset.x);
        Unity.Mathematics.Random randomY = new Unity.Mathematics.Random((uint)offset.y);
        Unity.Mathematics.Random tileRandom = new Unity.Mathematics.Random(randomX.NextUInt(0, 100000) + randomY.NextUInt(0, 100000) + seed);
        uint tileOverallSeed = tileRandom.NextUInt(0, 100000);
        //Create the same random but in a new form to get consistant values for the weights as well
        Unity.Mathematics.Random randomWeight = new Unity.Mathematics.Random(randomX.NextUInt(0, 100000) + randomY.NextUInt(0, 100000) + seed);
        //Define the size of the normal grid
        int gridSize = Mathf.RoundToInt(dimensions / regionAmount);
        //Use a for loop to iterate through all the surrounding tiles. The loop will iterate 8 times as each tile will have eight neighbors 
        for (int i = 0; i < 8; i++)
        {
            //Define values that will be used for each statement
            uint offsetX = 0;
            uint offsetY = 0;
            //Define the values of offsetX and offsetY
            switch (i)
            {
                //Top left tile
                case 0:
                    offsetX = (uint)offset.x - 1;
                    offsetY = (uint)offset.y + 1;
                    break;
                //Top middle tile
                case 1:
                    offsetX = (uint)offset.x;
                    offsetY = (uint)offset.y + 1;
                    break;
                //Top right tile
                case 2:
                    offsetX = (uint)offset.x + 1;
                    offsetY = (uint)offset.y + 1;
                    break;
                //Middle left tile
                case 3:
                    offsetX = (uint)offset.x - 1;
                    offsetY = (uint)offset.y;
                    break;
                //Middle right tile
                case 4:
                    offsetX = (uint)offset.x + 1;
                    offsetY = (uint)offset.y;
                    break;
                //Bottom left tile
                case 5:
                    offsetX = (uint)offset.x - 1;
                    offsetY = (uint)offset.y - 1;
                    break;
                //Bottom middle tile
                case 6:
                    offsetX = (uint)offset.x;
                    offsetY = (uint)offset.y - 1;
                    break;
                //Bottom right tile
                case 7:
                    offsetX = (uint)offset.x + 1;
                    offsetY = (uint)offset.y - 1;
                    break;
            }
            //Ensure the offset isn't zero
            if (offsetX == 0)
            {
                offsetX = 999999;
            }
            if (offsetY == 0)
            {
                offsetY = 999999;
            }
            //Create the random structs based upon that offset
            Unity.Mathematics.Random tileXRandom = new Unity.Mathematics.Random(offsetX);
            Unity.Mathematics.Random tileYRandom = new Unity.Mathematics.Random(offsetY);
            Unity.Mathematics.Random tileSeed = new Unity.Mathematics.Random(tileXRandom.NextUInt(0, 100000) + tileYRandom.NextUInt(0, 100000) + seed);
            Unity.Mathematics.Random tileWeight = new Unity.Mathematics.Random(tileXRandom.NextUInt(0, 100000) + tileYRandom.NextUInt(0, 100000) + seed);
            uint newTileSeed = tileSeed.NextUInt(0, 100000);

            //Storage of the throwaways for randoms
            float xPosition = 0;
            float yPosition = 0;


            Unity.Mathematics.Random xPointRandom;
            Unity.Mathematics.Random yPointRandom;
            uint originalNumber;
            uint combinedNumber;
            Unity.Mathematics.Random randomAmnts;
            //Set the edges of the tile map. Use a switch statement to determine what values to fill in, depending on the tile
            switch (i)
            {

                //Top left tile - only needs one value, the top left corner
                case 0:
                    //Subtract one from regionAmount because the offset of the main array starts at zero, not one
                    xPointRandom = new Unity.Mathematics.Random((uint)(((regionAmount - 1) * 694) + 1000));
                    yPointRandom = new Unity.Mathematics.Random((uint)(((regionAmount - 1) * 694) + 2000));
                    //Create the original number
                    originalNumber = (uint)xPointRandom.NextInt(0, 100000);
                    //Bitshift to free some room
                    combinedNumber = originalNumber << 16;
                    //Add the second number (Haha funny 'pipe equals' sign go brrrrr)
                    combinedNumber |= (uint)yPointRandom.NextInt(0, 100000);
                    //Get the seed
                    randomAmnts = new Unity.Mathematics.Random(combinedNumber + newTileSeed);

                    //RegionAmount * gridsize, because main tile is x+1 * gridsize and x+1 will evaluate to regionAmount at this point
                    //The actual placement of the point should be equal to dimensions - its X or Y, as that gets it offset up from this tile
                    xPosition = (((regionAmount - 1) * gridSize) + randomAmnts.NextInt(0, gridSize) - dimensions);
                    yPosition = (((regionAmount - 1) * gridSize) + randomAmnts.NextInt(0, gridSize) - dimensions);
                    points[0, 0].x = xPosition;
                    points[0, 0].y = yPosition;
                    weights[0, 0] = randomAmnts.NextFloat(0, 1);


                    break;
                //Top middle tile - needs the top row set except for edges
                case 1:

                    for (int tilePos = 0; tilePos < regionAmount; tilePos++)
                    {
                        //Random structs and bitshifting
                        xPointRandom = new Unity.Mathematics.Random((uint)((tilePos * 694) + 1000));
                        yPointRandom = new Unity.Mathematics.Random((uint)(((regionAmount - 1) * 694) + 2000));
                        originalNumber = (uint)xPointRandom.NextInt(0, 100000);
                        combinedNumber = originalNumber << 16;
                        combinedNumber |= (uint)yPointRandom.NextInt(0, 100000);
                        randomAmnts = new Unity.Mathematics.Random(combinedNumber + newTileSeed);
                        //Y has 1024 subtracted from it because we want the bottom, however, X stays the same because we want those points consistant
                        xPosition = (tilePos * gridSize) + randomAmnts.NextInt(0, gridSize);
                        yPosition = ((regionAmount - 1) * gridSize) + randomAmnts.NextInt(0, gridSize) - dimensions;
                        points[tilePos + 1, 0].x = xPosition;
                        points[tilePos + 1, 0].y = yPosition;
                        weights[tilePos + 1, 0] = randomAmnts.NextFloat(0, 1);



                    }


                    break;
                //Top right tile - only needs one value, the top right corner
                case 2:
                    //Subtract one from regionAmount because the offset of the main array starts at zero, not one
                    xPointRandom = new Unity.Mathematics.Random((uint)(0 + 1000));
                    yPointRandom = new Unity.Mathematics.Random((uint)(((regionAmount - 1) * 694) + 2000));
                    //Create the original number
                    originalNumber = (uint)xPointRandom.NextInt(0, 100000);
                    //Bitshift to free some room
                    combinedNumber = originalNumber << 16;
                    //Add the second number (Haha funny 'pipe equals' sign go brrrrr)
                    combinedNumber |= (uint)yPointRandom.NextInt(0, 100000);
                    //Get the seed
                    randomAmnts = new Unity.Mathematics.Random(combinedNumber + newTileSeed);
                    xPosition = 0 + randomAmnts.NextInt(0, gridSize) + dimensions;
                    yPosition = ((regionAmount - 1) * gridSize) + randomAmnts.NextInt(0, gridSize) - dimensions;
                    //RegionAmount * gridsize, because main tile is x+1 * gridsize and x+1 will evaluate to regionAmount at this point
                    points[regionAmount + 1, 0].x = xPosition;
                    points[regionAmount + 1, 0].y = yPosition;
                    weights[regionAmount + 1, 0] = randomAmnts.NextFloat(0, 1);


                    break;
                //Middle left tile - needs the left column set except for the edges
                case 3:
                    for (int tilePos = 0; tilePos < regionAmount; tilePos++)
                    {
                        //Random structs and bitshifting
                        xPointRandom = new Unity.Mathematics.Random((uint)(((regionAmount - 1) * 694) + 1000));
                        yPointRandom = new Unity.Mathematics.Random((uint)((tilePos * 694) + 2000));
                        originalNumber = (uint)xPointRandom.NextInt(0, 100000);
                        combinedNumber = originalNumber << 16;
                        combinedNumber |= (uint)yPointRandom.NextInt(0, 100000);
                        randomAmnts = new Unity.Mathematics.Random(combinedNumber + newTileSeed);
                        //Set points - tilePos + 1 is the same as x + 1 on main tile
                        xPosition = ((regionAmount - 1) * gridSize) + randomAmnts.NextInt(0, gridSize) - dimensions;
                        yPosition = (tilePos * gridSize) + randomAmnts.NextInt(0, gridSize);
                        points[0, tilePos + 1].x = xPosition;
                        points[0, tilePos + 1].y = yPosition;
                        weights[0, tilePos + 1] = randomAmnts.NextFloat(0, 1);



                    }
                    break;
                //Middle right tile - needs the right column set except for the edges
                case 4:
                    for (int tilePos = 0; tilePos < regionAmount; tilePos++)
                    {
                        //Random structs and bitshifting
                        xPointRandom = new Unity.Mathematics.Random((uint)(0 + 1000));
                        yPointRandom = new Unity.Mathematics.Random((uint)((tilePos * 694) + 2000));
                        originalNumber = (uint)xPointRandom.NextInt(0, 100000);
                        combinedNumber = originalNumber << 16;
                        combinedNumber |= (uint)yPointRandom.NextInt(0, 100000);
                        randomAmnts = new Unity.Mathematics.Random(combinedNumber + newTileSeed);
                        //Set points - tilePos + 1 is the same as x + 1 on main tile
                        xPosition = 0 + randomAmnts.NextInt(0, gridSize) + dimensions;
                        yPosition = (tilePos * gridSize) + randomAmnts.NextInt(0, gridSize);
                        points[regionAmount + 1, tilePos + 1].x = xPosition;
                        points[regionAmount + 1, tilePos + 1].y = yPosition;
                        weights[regionAmount + 1, tilePos + 1] = randomAmnts.NextFloat(0, 1);



                    }
                    break;
                //Bottom left tile - only needs one value, the bottom left corner
                case 5:
                    //Subtract one from regionAmount because the offset of the main array starts at zero, not one
                    xPointRandom = new Unity.Mathematics.Random((uint)(((regionAmount - 1) * 694) + 1000));
                    yPointRandom = new Unity.Mathematics.Random((uint)(0 + 2000));
                    //Create the original number
                    originalNumber = (uint)xPointRandom.NextInt(0, 100000);
                    //Bitshift to free some room
                    combinedNumber = originalNumber << 16;
                    //Add the second number (Haha funny 'pipe equals' sign go brrrrr)
                    combinedNumber |= (uint)yPointRandom.NextInt(0, 100000);
                    //Get the seed
                    randomAmnts = new Unity.Mathematics.Random(combinedNumber + newTileSeed);

                    //RegionAmount * gridsize, because main tile is x+1 * gridsize and x+1 will evaluate to regionAmount at this point
                    xPosition = ((regionAmount - 1) * gridSize) + randomAmnts.NextInt(0, gridSize) - dimensions;
                    yPosition = 0 + randomAmnts.NextInt(0, gridSize) + dimensions;
                    points[0, regionAmount + 1].x = xPosition;
                    points[0, regionAmount + 1].y = yPosition;
                    weights[0, regionAmount + 1] = randomAmnts.NextFloat(0, 1);

                    break;
                //Bottom middle tile - needs the bottom row set except for the edges
                case 6:
                    for (int tilePos = 0; tilePos < regionAmount; tilePos++)
                    {
                        //Random structs and bitshifting
                        xPointRandom = new Unity.Mathematics.Random((uint)((tilePos * 694) + 1000));
                        yPointRandom = new Unity.Mathematics.Random((uint)(0 + 2000));
                        originalNumber = (uint)xPointRandom.NextInt(0, 100000);
                        combinedNumber = originalNumber << 16;
                        combinedNumber |= (uint)yPointRandom.NextInt(0, 100000);
                        randomAmnts = new Unity.Mathematics.Random(combinedNumber + newTileSeed);
                        //Set points - tilePos + 1 is the same as x + 1 on main tile
                        xPosition = ((tilePos) * gridSize) + randomAmnts.NextInt(0, gridSize);
                        yPosition = 0 + randomAmnts.NextInt(0, gridSize) + dimensions;
                        points[tilePos + 1, regionAmount + 1].x = xPosition;
                        points[tilePos + 1, regionAmount + 1].y = yPosition;
                        weights[tilePos + 1, regionAmount + 1] = randomAmnts.NextFloat(0, 1);



                    }
                    break;
                //Bottom right tile - only needs one value, the bottom right corner
                case 7:
                    //Subtract one from regionAmount because the offset of the main array starts at zero, not one
                    xPointRandom = new Unity.Mathematics.Random((uint)(0 + 1000));
                    yPointRandom = new Unity.Mathematics.Random((uint)(0 + 2000));
                    //Create the original number
                    originalNumber = (uint)xPointRandom.NextInt(0, 100000);
                    //Bitshift to free some room
                    combinedNumber = originalNumber << 16;
                    //Add the second number (Haha funny 'pipe equals' sign go brrrrr)
                    combinedNumber |= (uint)yPointRandom.NextInt(0, 100000);
                    //Get the seed
                    randomAmnts = new Unity.Mathematics.Random(combinedNumber + newTileSeed);

                    //RegionAmount * gridsize, because main tile is x+1 * gridsize and x+1 will evaluate to regionAmount at this point
                    //This one has grid changed
                    xPosition = 0 + randomAmnts.NextInt(0, gridSize) + dimensions;
                    yPosition = 0 + randomAmnts.NextInt(0, gridSize) + dimensions;
                    points[regionAmount + 1, regionAmount + 1].x = xPosition;
                    points[regionAmount + 1, regionAmount + 1].y = yPosition;
                    weights[regionAmount + 1, regionAmount + 1] = randomAmnts.NextFloat(0, 1);

                    break;
            }
        }






        //Set the middle of the tile map
        for (int y = 0; y < regionAmount; y++)
        {
            for (int x = 0; x < regionAmount; x++)
            {
                //Generate random for each point - X and Y are offset by 1000 as to not attempt a zero seed
                Unity.Mathematics.Random xPointRandom = new Unity.Mathematics.Random((uint)((x * 694) + 1000));
                Unity.Mathematics.Random yPointRandom = new Unity.Mathematics.Random((uint)((y * 694) + 2000));

                uint originalNumber = (uint)xPointRandom.NextInt(0, 100000);

                uint combinedNumber = originalNumber << 16;
                combinedNumber |= (uint)yPointRandom.NextInt(0, 100000);

                Unity.Mathematics.Random randomAmnt = new Unity.Mathematics.Random(combinedNumber + tileOverallSeed);

                //X and Y are shifted by one to create a ring that will generate with the adjacent tiles points
                points[x + 1, y + 1].x = ((x) * gridSize) + randomAmnt.NextInt(0, gridSize);
                points[x + 1, y + 1].y = ((y) * gridSize) + randomAmnt.NextInt(0, gridSize);
                weights[x + 1, y + 1] = randomAmnt.NextFloat(0, 1);

            }

        }



        //Get points
        for (int y = 0; y < dimensions; y++)
        {
            for (int x = 0; x < dimensions; x++)
            {
                float distance = float.MaxValue;
                float secondDistance = float.MaxValue;
                int2 value = 0;
                int2 secondValue = 0;
                //Adding 2 to make room for tile borders
                float dist = 0;
                for (int pointY = 0; pointY < regionAmount + 2; pointY++)
                {
                    for (int pointX = 0; pointX < regionAmount + 2; pointX++)
                    {
                        dist = Vector2.Distance(new Vector2(x, y), points[pointX, pointY]);
                        if (dist < distance)
                        {
                            //If this is the closest so far, shift the old one into the second distance
                            secondDistance = distance;
                            secondValue = value;
                            //And set the new one
                            distance = dist;
                            value.x = pointX;
                            value.y = pointY;
                        }
                        //If its not  the closest, check if its the second closest
                        else if (dist < secondDistance)
                        {
                            secondDistance = dist;
                            secondValue.x = pointX;
                            secondValue.y = pointY;
                        }
                    }
                }
                float blendAmnt = 1;

                float maxPixelDistance = Mathf.Abs(dist - secondDistance);
                //Check if the distance between points is actually smaller than the blendPixelAmount
                if (Mathf.Abs(distance - secondDistance) <= blendPixelAmount)
                {


                    blendAmnt = 1 - ((Mathf.Abs(distance - secondDistance) / blendPixelAmount));


                }
                //Set value
                voronoiNative[(((dimensions - 1) - y) * dimensions) + x] = new float3(weights[value.x, value.y], blendAmnt, weights[secondValue.x, secondValue.y]);

                //If it is close enough, blend


            }
            if (y % (currentSpeedSettings.maxIterationsPerFrame) == 0)
            {
                yield return new WaitForEndOfFrame();
            }
        }




        voronoiFinishCompletion = true;
        yield break;
    }
    //Void for creating a GPUI detail manager for a terrain
    public static void createManagerForTerrain(Terrain terrain, List<int> crossQuadCounts)
    {
        //Add the GPUInstancerDetailManager class to the terrain
        GPUInstancerDetailManager detailManager = terrain.gameObject.AddComponent<GPUInstancerDetailManager>();
        detailManager.enabled = false;
        //Call the GPUInstancer API function to sync the terrain with the manager

        //Set the camera of the manager if generation is finished.
        detailManager.SetCamera(Camera.main);


        //Fill in settings to the detail manager
        detailManager.runInThreads = true;
        detailManager.isFrustumCulling = true;
        detailManager.isOcclusionCulling = true;
        //Set terrain settings
        detailManager.terrainSettings = ScriptableObject.CreateInstance<GPUInstancerTerrainSettings>();
        detailManager.terrainSettings.maxDetailDistance = 600;
        detailManager.terrainSettings.autoSPCellSize = true;
        detailManager.terrainSettings.healthyDryNoiseTexture = GPUInstancerHealthyDryNoiseTexture;
        detailManager.terrainSettings.windWaveNormalTexture = GPUInstancerWindWaveNormalTexture;
        detailManager.SetupManagerWithTerrain(terrain);
        GPUInstancerDetailPrototype currentDetailPrototype = null;
        for (int i = 0; i < detailManager.prototypeList.Count; i++)
        {
            currentDetailPrototype = detailManager.prototypeList[i] as GPUInstancerDetailPrototype;

            if (!currentDetailPrototype.usePrototypeMesh)
            {
                currentDetailPrototype.quadCount = crossQuadCounts[i];
            }
            currentDetailPrototype.healthyDryNoiseTexture = GPUInstancerHealthyDryNoiseTexture;
            currentDetailPrototype.isShadowCasting = true;
        }
        GPUInstancerAPI.InitializeGPUInstancer(detailManager);
        detailManager.enabled = true;




    }
    //Method that PlayerNetwork calls to start the terrain generation Coroutine
    public void StartTerrainGenCo()
    {
        StartCoroutine(initialTerrainGeneration());
    }
}
[System.Serializable]
public class BuildingData
{
    //Enum holding the different types of buildings
    public enum BuildingType
    {
        BrokenColleseum,
        SmallHouse
    }
    //Type of building
    public BuildingType type;
    //The position of the center of this building
    public Vector3 centerPosition { get; set; }
    //What terrain tile this building lies at
    public int terrainTileIndex;
    //The position offset to spawn the building at
    public Vector3 spawnOffset;
    //The scale to spawn the building at
    public Vector3 spawnedScale;
    //What offset to apply to the building when it is spawned
    public Vector3 spawnedRotationOffset;
    //Index to use when getting the model
    public int modelIndex;
    //If the building will generate any terrain holes
    public bool hasHoles;
    //An int array holding the chosen layout of all the random layout creators
    public int[] layouts;
    public int GetBuildingRadius()
    {
        switch (type)
        {
            case BuildingType.BrokenColleseum:
                return 150;
            case BuildingType.SmallHouse:
                return 10;
            default:
                return 10;
        }
    }
    public int GetFadeMargin()
    {
        switch (type)
        {
            case BuildingType.BrokenColleseum:
                return 110;
            case BuildingType.SmallHouse:
                return 5;
            default:
                return 5;
        }
    }
    public int GetObjectBlockRadius()
    {
        switch (type)
        {
            case BuildingType.BrokenColleseum:
                return 60;
            case BuildingType.SmallHouse:
                return 5;
            default:
                return 5;
        }
    }
    public GameObject GetObjectModel()
    {
        switch (modelIndex)
        {
            default:
                throw new System.NullReferenceException("Attempting to retrieve a model from an index that doesn't exist");
            case 0:
                return BuildingAssets.instance.brokenColleseumObject;
        }
    }
    public RandomLayoutScriptable GetLayoutScriptable()
    {
        switch (type)
        {
            case BuildingType.BrokenColleseum:
                return RandomLayoutAssets.instance.ColleseumLayout;
            default:
            case BuildingType.SmallHouse:
                throw new System.NotImplementedException("Layout doesn't exist lmao");
        }

    }

}
//Class handling conversions of BuildingData classes to structs and vice versa
public static class BuildingDataConversion
{
    ///<summary>
    ///Method that converts a BuildingData class to a struct
    /// </summary>
    /// <returns>A copy of the class in struct form</returns>
    public static BuildingDataNetworkStruct ToStruct(this BuildingData source)
    {
        if (source == null) throw new System.NullReferenceException("Attempting to convert a null object");
        return new BuildingDataNetworkStruct
        {
            type = source.type,
            centerPosition = source.centerPosition,
            terrainTileIndex = source.terrainTileIndex,
            spawnOffset = source.spawnOffset,
            spawnedScale = source.spawnedScale,
            spawnedRotationOffset = source.spawnedRotationOffset,
            modelIndex = source.modelIndex,
            hasHoles = source.hasHoles,
            layouts = source.layouts

        };
    }
    ///<summary>
    ///Method that converts a BuildingData struct to a class
    /// </summary>
    /// <returns>A copy of the struct in class form</returns>
    public static BuildingData ToClass(this BuildingDataNetworkStruct source)
    {
        return new BuildingData
        {
            type = source.type,
            centerPosition = source.centerPosition,
            terrainTileIndex = source.terrainTileIndex,
            spawnOffset = source.spawnOffset,
            spawnedScale = source.spawnedScale,
            spawnedRotationOffset = source.spawnedRotationOffset,
            modelIndex = source.modelIndex,
            hasHoles = source.hasHoles,
            layouts = source.layouts
        };
    }
}
//BuildingData class in network struct form
[System.Serializable]
public struct BuildingDataNetworkStruct : INetworkSerializable
{
    public BuildingData.BuildingType type;
    public Vector3 centerPosition;
    public int terrainTileIndex;
    public Vector3 spawnOffset;
    public Vector3 spawnedScale;
    public Vector3 spawnedRotationOffset;
    public int modelIndex;
    public bool hasHoles;
    public int[] layouts;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref type);
        serializer.SerializeValue(ref centerPosition);
        serializer.SerializeValue(ref terrainTileIndex);
        serializer.SerializeValue(ref spawnOffset);
        serializer.SerializeValue(ref spawnedScale);
        serializer.SerializeValue(ref spawnedRotationOffset);
        serializer.SerializeValue(ref modelIndex);
        serializer.SerializeValue(ref hasHoles);
        serializer.SerializeValue(ref layouts);
    }
}



