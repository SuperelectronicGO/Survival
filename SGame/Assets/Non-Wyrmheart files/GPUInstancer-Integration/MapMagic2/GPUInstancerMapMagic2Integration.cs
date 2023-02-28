using System.Collections.Generic;
using UnityEngine;
using System;
#if MAPMAGIC2
using MapMagic.Terrains;
using MapMagic.Products;
using MapMagic.Nodes;
using MapMagic.Nodes.MatrixGenerators;
//using MapMagic.Nodes.ObjectsGenerators;
using MapMagic.Core;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GPUInstancer
{
    [ExecuteInEditMode]
    public class GPUInstancerMapMagic2Integration : MonoBehaviour
    {
        public List<GPUInstancerPrototype> detailPrototypes;
        public List<GPUInstancerPrototype> treePrototypes;
        public List<GPUInstancerPrototype> prefabPrototypes;
        public GPUInstancerTerrainSettings terrainSettings;
        public bool importDetails;
        public bool importTrees;
        public bool importObjects;
        private bool _selectAllPrefabs;

        public bool autoSelectCamera = true;
        public GPUInstancerCameraData cameraData = new GPUInstancerCameraData(null);
        public bool isFrustumCulling = true;
        public bool isOcclusionCulling = true;
        public float minCullingDistance = 0;
        public int detailLayer = 0;
        public bool detailRunInThreads = true;
        public bool disableMeshRenderers = false;
        public bool prefabRunInThreads = false;
        public bool simulateDetailsAtEditor = false;
        public bool useFloatingOriginHandler = false;
        public Transform floatingOriginTransform;

        public List<DetailPrototype> terrainDetailPrototypes;
        public List<TreePrototype> terrainTreePrototypes;
        public List<GameObject> prefabs;
        public List<GameObject> selectedPrefabs;

        public GPUInstancerPrefabManager prefabManagerInstance;
        public GPUInstancerTreeManager treeManagerInstance;

#if UNITY_EDITOR
        [HideInInspector]
        public GPUInstancerPrototype selectedDetailPrototype;
        public GPUInstancerPrototype selectedTreePrototype;
        public GPUInstancerPrototype selectedPrefabPrototype;
#endif


#if MAPMAGIC2
        public MapMagicObject mapMagicInstance;

        private void Start()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                CheckPrototypeChanges();
            }
            else
            {
#endif
                if (prefabPrototypes != null && prefabPrototypes.Count > 0)
                    MapMagicTerrainAddPrefabManagerSingleton();
                TerrainTile.OnTileApplied += MapMagic2TileApplied;

                // for pinned terrains
                Terrain[] activeTerrains = Terrain.activeTerrains;
                if (activeTerrains != null)
                {
                    foreach (Terrain terrain in activeTerrains)
                    {
                        MapMagicTerrainAddDetailManager(terrain);
                        MapMagicTerrainAddTreeManager(terrain);
                    }
                }

                if (useFloatingOriginHandler && floatingOriginTransform == null)
                {
                    if (activeTerrains != null && activeTerrains.Length > 0 && activeTerrains[0] != null)
                        floatingOriginTransform = activeTerrains[0].transform;
                }
#if UNITY_EDITOR
            }
#endif
            GPUInstancerConstants.gpuiSettings.SetDefultBindings();
        }

        private void MapMagic2TileApplied(TerrainTile terrainTile, TileData tileData, StopToken stopToken)
        {
            if (terrainTile.main != null)
            {
                if (terrainTile.main.terrain.terrainData.detailPrototypes.Length > 0)
                    MapMagicTerrainAddDetailManager(terrainTile.main.terrain);
                if (terrainTile.main.terrain.terrainData.treePrototypes.Length > 0)
                    MapMagicTerrainAddTreeManager(terrainTile.main.terrain);
            }
        }

#if UNITY_EDITOR
        private void LateUpdate()
        {
            if (!Application.isPlaying)
            {
                CheckPrototypeChanges();
                if (simulateDetailsAtEditor)
                {
                    Terrain[] activeTerrains = Terrain.activeTerrains;
                    if (activeTerrains != null)
                    {
                        foreach (Terrain terrain in activeTerrains)
                        {
                            GPUInstancerDetailManager detailManager = MapMagicTerrainAddDetailManager(terrain);
                            if (detailManager.gpuiSimulator != null)
                            {
                                detailManager.keepSimulationLive = true;
                                if (!detailManager.gpuiSimulator.simulateAtEditor)
                                    detailManager.gpuiSimulator.StartSimulation();
                            }
                        }
                    }
                }
                else
                {
                    Terrain[] activeTerrains = Terrain.activeTerrains;
                    if (activeTerrains != null)
                    {
                        foreach (Terrain terrain in activeTerrains)
                        {
                            GPUInstancerDetailManager detailManager = terrain.GetComponent<GPUInstancerDetailManager>();
                            if (detailManager != null && detailManager.gpuiSimulator != null)
                            {
                                detailManager.keepSimulationLive = false;
                                if (detailManager.gpuiSimulator.simulateAtEditor)
                                    detailManager.gpuiSimulator.StopSimulation();
                            }
                        }
                    }
                }

                if (useFloatingOriginHandler)
                {
                    Terrain[] activeTerrains = Terrain.activeTerrains;
                    if (activeTerrains != null && activeTerrains.Length > 0 && activeTerrains[0] != null)
                        floatingOriginTransform = activeTerrains[0].transform;
                }
            }
        }
#endif

        private void OnDestroy()
        {
            TerrainTile.OnTileApplied -= MapMagic2TileApplied;
        }

        private void Reset()
        {
            if (mapMagicInstance == null)
                SetMapMagicInstance();
#if UNITY_EDITOR
            CheckPrototypeChanges();
#endif
        }

        public void SetMapMagicInstance()
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "GPUI Set Map Magic Instance");
#endif
            mapMagicInstance = FindObjectOfType<MapMagic.Core.MapMagicObject>();
            importDetails = true;
            importTrees = true;
            importObjects = true;
            _selectAllPrefabs = true;
        }

        public void SetUpWithGeneratorsAsset()
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "GPUI Map Magic Setup");
#endif
            GPUInstancerConstants.gpuiSettings.SetDefultBindings();

            terrainDetailPrototypes = new List<DetailPrototype>();
            terrainTreePrototypes = new List<TreePrototype>();
            prefabs = new List<GameObject>();
            if (selectedPrefabs == null)
                selectedPrefabs = new List<GameObject>();

            if (mapMagicInstance == null || mapMagicInstance.graph == null)
                return;

            FillListsWithGeneratorsAsset(mapMagicInstance.graph);

            if (selectedPrefabs.Count > 0)
                selectedPrefabs.RemoveAll(p => !prefabs.Contains(p));
            else if (_selectAllPrefabs)
            {
                selectedPrefabs.AddRange(prefabs);
                _selectAllPrefabs = false;
            }
        }

        public void FillListsWithGeneratorsAsset(Graph graph)
        {
            if (graph == null || graph.generators == null)
                return;

            foreach (Generator generator in graph.generators)
            {
                FillListWithGenerator(generator);
            }
        }

        public void FillListWithGenerator(Generator generator)
        {
            // biome
            if (generator is IBiome)
            {
                IBiome biome = (IBiome)generator;
                FillListsWithGeneratorsAsset(biome.SubGraph);
            }
            else if (generator is IMultiLayer)
            {
                IMultiLayer multiLayer = (IMultiLayer)generator;
                foreach (var item in multiLayer.Layers)
                {
                    if (item is IBiome)
                    {
                        IBiome biome = (IBiome)item;
                        FillListsWithGeneratorsAsset(biome.SubGraph);
                    }
                }
            }
            // detail instancing
            else if (generator is GrassOutput200)
            {
                GrassOutput200 gen = (GrassOutput200)generator;
                terrainDetailPrototypes.Add(gen.prototype);
            }
            // tree instancing - REMOVED BECAUSE I DON'T OWN MM OBJECTS
            /*
            else if (generator is TreesOutput)
            {
                TreesOutput gen = (TreesOutput)generator;
                for (int i = 0; i < gen.prefabs.Length; i++)
                {
                    if (gen.prefabs[i] != null)
                        terrainTreePrototypes.Add(new TreePrototype() { prefab = gen.prefabs[i] });
                    else
                        Debug.LogWarning("Map Magic generator contains unassigned Trees Output values. Please assign or remove these values.");
                }
            }
            // prefab instancing
            else if (generator is ObjectsOutput)
            {
                ObjectsOutput gen = (ObjectsOutput)generator;
                for (int i = 0; i < gen.prefabs.Length; i++)
                {
                    if (gen.prefabs[i] != null && !prefabs.Contains(gen.prefabs[i]))
                    {
#if UNITY_EDITOR
#if UNITY_2018_3_OR_NEWER
                        if (PrefabUtility.GetPrefabAssetType(gen.prefabs[i]) == PrefabAssetType.Model)
#else
                            if (PrefabUtility.GetPrefabType(gen.prefabs[i]) == PrefabType.ModelPrefab)
#endif
                            Debug.LogWarning(GPUInstancerConstants.TEXT_PREFAB_TYPE_WARNING_3D + " " + gen.prefabs[i].name, gen.prefabs[i]);
                        else
#endif
                            prefabs.Add(gen.prefabs[i]);
                    }
                }
                
            }
            */
        }

        public void GeneratePrototypes()
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "GPUI Set Map Magic Import");
            if (terrainSettings != null)
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(terrainSettings));
#endif

            // import terrain details
            detailPrototypes = new List<GPUInstancerPrototype>();

            if (importDetails)
            {
                GenerateMapMagicTerrainSettings();
                GPUInstancerUtility.SetDetailInstancePrototypes(gameObject, detailPrototypes, terrainDetailPrototypes.ToArray(), 2, terrainSettings, true);
            }
            
            // import terrain trees
            treePrototypes = new List<GPUInstancerPrototype>();
            if (importTrees)
            {
                GenerateMapMagicTerrainSettings();
                GPUInstancerUtility.SetTreeInstancePrototypes(gameObject, treePrototypes, terrainTreePrototypes.ToArray(), terrainSettings, true);
            }

            // import prefabs
            prefabPrototypes = new List<GPUInstancerPrototype>();
            if (importObjects)
            {
                GPUInstancerUtility.SetPrefabInstancePrototypes(gameObject, prefabPrototypes, selectedPrefabs, true);
                foreach (GPUInstancerPrefabPrototype prefabPrototype in prefabPrototypes)
                {
                    prefabPrototype.enableRuntimeModifications = true;
                    prefabPrototype.addRemoveInstancesAtRuntime = true;
                    prefabPrototype.addRuntimeHandlerScript = true;
                }
            }
            else
                selectedPrefabs.Clear();

            foreach(GameObject notSelectedPrefab in prefabs.FindAll(p => !selectedPrefabs.Contains(p)))
            {
                if(notSelectedPrefab.GetComponent<GPUInstancerPrefab>() != null)
                {
                    DestroyImmediate(notSelectedPrefab.GetComponent<GPUInstancerPrefab>(), true);
#if UNITY_EDITOR
                    EditorUtility.SetDirty(notSelectedPrefab);
#endif
                }
            }

#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        private void GenerateMapMagicTerrainSettings()
        {
            if (terrainSettings)
                return;

            terrainSettings = ScriptableObject.CreateInstance<GPUInstancerTerrainSettings>();
            terrainSettings.name = "GPUI_MapMagic_" + mapMagicInstance.graph.name + "_" + mapMagicInstance.graph.GetInstanceID();
            //terrainSettings.maxDetailDistance = mapMagicInstance.detailDistance;
            //terrainSettings.maxTreeDistance = mapMagicInstance.treeDistance;
            //terrainSettings.detailDensity = mapMagicInstance.detailDensity;
            terrainSettings.healthyDryNoiseTexture = Resources.Load<Texture2D>(GPUInstancerConstants.NOISE_TEXTURES_PATH + GPUInstancerConstants.DEFAULT_HEALTHY_DRY_NOISE);
            terrainSettings.windWaveNormalTexture = Resources.Load<Texture2D>(GPUInstancerConstants.NOISE_TEXTURES_PATH + GPUInstancerConstants.DEFAULT_WIND_WAVE_NOISE);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                string assetPath = GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.PROTOTYPES_TERRAIN_PATH + terrainSettings.name + ".asset";

                if (!System.IO.Directory.Exists(GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.PROTOTYPES_TERRAIN_PATH))
                {
                    System.IO.Directory.CreateDirectory(GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.PROTOTYPES_TERRAIN_PATH);
                }

                AssetDatabase.CreateAsset(terrainSettings, assetPath);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
#endif
        }

        private GPUInstancerDetailManager MapMagicTerrainAddDetailManager(Terrain terrain)
        {
            GPUInstancerDetailManager newDetailManager = terrain.GetComponent<GPUInstancerDetailManager>();
            if (newDetailManager == null && detailPrototypes != null && detailPrototypes.Count > 0)
            {
                newDetailManager = terrain.gameObject.AddComponent<GPUInstancerDetailManager>();
                newDetailManager.isFrustumCulling = isFrustumCulling;
                newDetailManager.isOcclusionCulling = isOcclusionCulling;
                newDetailManager.minCullingDistance = minCullingDistance;
                newDetailManager.detailLayer = detailLayer;
                newDetailManager.runInThreads = detailRunInThreads;
                newDetailManager.autoSelectCamera = autoSelectCamera;
                newDetailManager.cameraData.SetCamera(cameraData.mainCamera);
                newDetailManager.cameraData.renderOnlySelectedCamera = cameraData.renderOnlySelectedCamera;
                newDetailManager.cameraData.hiZOcclusionGenerator = null;
                newDetailManager.InitializeCameraData();
                // for mapmagic detail optimization
                if (terrain.terrainData.detailPrototypes.Length != detailPrototypes.Count)
                {
                    int terrainDetailIndex = 0;
                    List<GPUInstancerPrototype> newPrototypeList = new List<GPUInstancerPrototype>();
                    for(int i = 0; i < detailPrototypes.Count; i++)
                    {
                        if (terrainDetailIndex >= terrain.terrainData.detailPrototypes.Length)
                            break;

                        GPUInstancerDetailPrototype dp = (GPUInstancerDetailPrototype)detailPrototypes[i];
                        if(!terrain.terrainData.detailPrototypes[terrainDetailIndex].usePrototypeMesh && dp.prototypeTexture == terrain.terrainData.detailPrototypes[terrainDetailIndex].prototypeTexture)
                        {
                            newPrototypeList.Add(dp);
                            terrainDetailIndex++;
                        }
                        else if (terrain.terrainData.detailPrototypes[terrainDetailIndex].usePrototypeMesh && dp.prefabObject == terrain.terrainData.detailPrototypes[terrainDetailIndex].prototype)
                        {
                            newPrototypeList.Add(dp);
                            terrainDetailIndex++;
                        }
                    }
                    newDetailManager.prototypeList = newPrototypeList;
                }
                else
                    newDetailManager.prototypeList = detailPrototypes;
                newDetailManager.SetupManagerWithTerrain(terrain);

                newDetailManager.terrainSettings.maxDetailDistance = terrainSettings.maxDetailDistance;
                newDetailManager.terrainSettings.detailDensity = terrainSettings.detailDensity;
                newDetailManager.terrainSettings.healthyDryNoiseTexture = terrainSettings.healthyDryNoiseTexture;
                newDetailManager.terrainSettings.windWaveNormalTexture = terrainSettings.windWaveNormalTexture;
                newDetailManager.terrainSettings.windVector = terrainSettings.windVector;
                newDetailManager.terrainSettings.autoSPCellSize = terrainSettings.autoSPCellSize;
                newDetailManager.terrainSettings.preferedSPCellSize = terrainSettings.preferedSPCellSize;

                if (terrain.gameObject.activeSelf)
                    newDetailManager.InitializeRuntimeDataAndBuffers();
            }
            newDetailManager.useFloatingOriginHandler = useFloatingOriginHandler;
            newDetailManager.floatingOriginTransform = floatingOriginTransform;
            newDetailManager.HandleFloatingOrigin();
            return newDetailManager;
        }

        private void MapMagicTerrainAddTreeManager(Terrain terrain)
        {
            if (treePrototypes != null && treePrototypes.Count > 0)
            {
                if (treeManagerInstance == null)
                {
                    GameObject treeManagerInstanceGO = new GameObject("GPUI Tree Manager");
                    treeManagerInstance = treeManagerInstanceGO.AddComponent<GPUInstancerTreeManager>();
                    treeManagerInstance.isFrustumCulling = isFrustumCulling;
                    treeManagerInstance.isOcclusionCulling = isOcclusionCulling;
                    treeManagerInstance.minCullingDistance = minCullingDistance;
                    treeManagerInstance.autoSelectCamera = autoSelectCamera;
                    treeManagerInstance.cameraData.SetCamera(cameraData.mainCamera);
                    treeManagerInstance.cameraData.renderOnlySelectedCamera = cameraData.renderOnlySelectedCamera;
                    treeManagerInstance.cameraData.hiZOcclusionGenerator = null;
                    treeManagerInstance.InitializeCameraData();
                    treeManagerInstance.initializeWithCoroutine = false;
                    treeManagerInstance.useFloatingOriginHandler = useFloatingOriginHandler;
                    treeManagerInstance.floatingOriginTransform = floatingOriginTransform;
                    treeManagerInstance.HandleFloatingOrigin();

                    treeManagerInstance.SetupManagerWithTerrain(terrain);
                    treeManagerInstance.terrainSettings = terrainSettings;
                    treeManagerInstance.prototypeList = treePrototypes;
                    treeManagerInstance.InitializeRuntimeDataAndBuffers();
                }

                if (terrain.GetComponent<GPUInstancerTerrainRuntimeHandler>() == null) { }
                    terrain.gameObject.AddComponent<GPUInstancerTerrainRuntimeHandler>();

                treeManagerInstance.useFloatingOriginHandler = useFloatingOriginHandler;
                treeManagerInstance.floatingOriginTransform = floatingOriginTransform;
                treeManagerInstance.HandleFloatingOrigin();
            }
        }

        private void MapMagicTerrainAddPrefabManagerSingleton()
        {
            if (prefabPrototypes != null && prefabPrototypes.Count > 0)
            {
                if (prefabManagerInstance == null)
                {
                    GameObject prefabManagerInstanceGO = new GameObject("GPUI Prefab Manager");
                    prefabManagerInstance = prefabManagerInstanceGO.AddComponent<GPUInstancerPrefabManager>();
                    prefabManagerInstance.isFrustumCulling = isFrustumCulling;
                    prefabManagerInstance.isOcclusionCulling = isOcclusionCulling;
                    prefabManagerInstance.minCullingDistance = minCullingDistance;
                    prefabManagerInstance.autoSelectCamera = autoSelectCamera;
                    prefabManagerInstance.cameraData.SetCamera(cameraData.mainCamera);
                    prefabManagerInstance.cameraData.renderOnlySelectedCamera = cameraData.renderOnlySelectedCamera;
                    prefabManagerInstance.cameraData.hiZOcclusionGenerator = null;
                    prefabManagerInstance.InitializeCameraData();
                    prefabManagerInstance.enableMROnRemoveInstance = false;
                    prefabManagerInstance.enableMROnManagerDisable = false;
                    prefabManagerInstance.useFloatingOriginHandler = useFloatingOriginHandler;
                    prefabManagerInstance.floatingOriginTransform = floatingOriginTransform;
                    prefabManagerInstance.HandleFloatingOrigin();

                    prefabManagerInstance.prototypeList = prefabPrototypes;
                    prefabManagerInstance.RegisterPrefabsInScene();
                    prefabManagerInstance.InitializeRuntimeDataAndBuffers();
                }

                prefabManagerInstance.useFloatingOriginHandler = useFloatingOriginHandler;
                prefabManagerInstance.floatingOriginTransform = floatingOriginTransform;
                prefabManagerInstance.HandleFloatingOrigin();
            }
        }
#endif // MAPMAGIC

#if UNITY_EDITOR
        public void CheckPrototypeChanges()
        {
            GPUInstancerConstants.gpuiSettings.SetDefultBindings();

            if (GPUInstancerConstants.gpuiSettings.shaderBindings != null)
            {
                GPUInstancerConstants.gpuiSettings.shaderBindings.ClearEmptyShaderInstances();

                CheckForShaderBindings(detailPrototypes);
                CheckForShaderBindings(treePrototypes);
                CheckForShaderBindings(prefabPrototypes);
            }
            if (GPUInstancerConstants.gpuiSettings.billboardAtlasBindings != null)
            {
                GPUInstancerConstants.gpuiSettings.billboardAtlasBindings.ClearEmptyBillboardAtlases();

                CheckForBillboardBindinds(detailPrototypes);
                CheckForBillboardBindinds(treePrototypes);
                CheckForBillboardBindinds(prefabPrototypes);
            }
        }

        public void CheckForShaderBindings(List<GPUInstancerPrototype> prototypeList)
        {
            if (prototypeList != null)
            {
                foreach (GPUInstancerPrototype prototype in prototypeList)
                {
                    if (prototype != null && prototype.prefabObject != null)
                    {
                        GPUInstancerUtility.GenerateInstancedShadersForGameObject(prototype);
                        if (string.IsNullOrEmpty(prototype.warningText))
                        {
                            if (prototype.prefabObject.GetComponentInChildren<MeshRenderer>() == null)
                            {
                                prototype.warningText = "Prefab object does not contain any Mesh Renderers.";
                            }
                        }
                    }
                }
            }
        }

        public void CheckForBillboardBindinds(List<GPUInstancerPrototype> prototypeList)
        {
            //if (prototypeList != null)
            //{
            //    foreach (GPUInstancerPrototype prototype in prototypeList)
            //    {
            //        if (prototype.prefabObject != null && prototype.useGeneratedBillboard &&
            //                (prototype.billboard == null || prototype.billboard.albedoAtlasTexture == null || prototype.billboard.normalAtlasTexture == null))
            //            GPUInstancerUtility.GeneratePrototypeBillboard(prototype, billboardAtlasBindings);
            //    }
            //}
        }
#endif

        public void SetCamera(Camera camera)
        {
            cameraData.mainCamera = camera;
            GPUInstancerAPI.SetCamera(camera);
        }
    }
}
