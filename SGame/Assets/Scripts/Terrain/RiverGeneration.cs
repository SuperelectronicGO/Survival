using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverGeneration : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int mapWidth=10240;
    [SerializeField] private int mapHeight=10240;
    [SerializeField] private float riverDiameter=30;
    [SerializeField] private bool drawHeightmap = false;
    [Header("Components")]
    [SerializeField] private Transform generatorParent;
    [SerializeField] private GameObject riverMesh;
    
    private Vector3 riverStartPoint;
    [HideInInspector]
    public List<Terrain> terrains = new List<Terrain>();
    private float maxRiverPoint = 0;
    private Vector2 minTerrainPos;
    

    
    private float[,] heightmap;
    private float[,] tHeightmap;









  
  


    //Start gathers a list of all terrains in an island
    void Start()
    {
       
       
    }
    public void GenerateRiverComponents()
    {
       
        riverStartPoint = Vector3.zero;

        GenerateCompleteTerrainHeightmap();

        //Get max height of terrain
        maxRiverPoint = getMaxTerrainHeight(terrains);

        //Loop until a valid point is found that the river can start upon
        while (riverStartPoint.y < maxRiverPoint * 0.9)
        {
            riverStartPoint = getRiverStartPoint(terrains, maxRiverPoint);
            if(riverStartPoint.y >= maxRiverPoint * 0.9)
            {
                break;
            }
          
        }

        //Generate the river heightmap
        heightmap = Noise.GenerateRiverMap(mapWidth+1, mapHeight+1, riverStartPoint, riverDiameter);
        riverMesh.GetComponent<RiverMeshGenerator>().riverHeightmap = heightmap;
        //Optional drawing of heightmap on a plane
        if (drawHeightmap)
        {
            MapDisplay display = FindObjectOfType<MapDisplay>();
            display.DrawNoiseMap(heightmap);
        }

        //Find minimum position of all terrains in the target group (used as baseline for which terrain gets [0,0] on the river heightmap (currently unused)
        minTerrainPos = new Vector2(terrains[0].transform.position.x, terrains[0].transform.position.z);
        for(int i=0; i<terrains.Count; i++)
        {
            if (terrains[i].transform.position.x < minTerrainPos.x)
            {
                minTerrainPos.x = terrains[i].transform.position.x;
            }
            if (terrains[i].transform.position.z < minTerrainPos.y)
            {
                minTerrainPos.y = terrains[i].transform.position.z;
            }
        }

        
    }
    // Update is called once per frame
    void Update()
    {
      
       if (Input.GetKeyDown(KeyCode.Space)) {
       //     GenerateRiverComponents();
            GenerateRiver();
          
         
       
         } 
       


    }

    public void GenerateRiver()
    {
        foreach(Terrain t in terrains)
        {
            bool terrainAffected = false;
            float[,] mapToSet = new float[1025, 1025];
            for(int y=0; y<1025; y++)
            {
                for (int x = 0; x <1025; x++)
                {
                    float heightmapValue = heightmap[x + Mathf.RoundToInt(t.transform.position.x * 1.024f), y + +Mathf.RoundToInt(t.transform.position.z * 1.024f)];
                    mapToSet[y,x] = tHeightmap[x + Mathf.RoundToInt(t.transform.position.x * 1.024f), y + Mathf.RoundToInt(t.transform.position.z * 1.024f)]-(heightmapValue * 0.03f);
                    if (heightmapValue != 0)
                    {
                        terrainAffected = true;
                    }
                }
            }

            if (terrainAffected)
            {
                t.terrainData.SetHeights(0,0,mapToSet);
            }

        }


        riverMesh.GetComponent<RiverMeshGenerator>().generateMesh(false);
















        /*


        foreach (Terrain t in terrains)
        {
            bool terrainAffected = false;
            float[,] updatedHeightmap = t.terrainData.GetHeights(0, 0, 1024, 1024);
            for (int y = 0; y < 1024; y++)
            {
                for (int x = 0; x < 1024; x++)
                {
                   // if (heightmap[x, y] != 0)
                   // {
                        terrainAffected = true;

                        updatedHeightmap[x, y] -= (heightmap[x + Mathf.RoundToInt((t.transform.position.x) * 1.024f), y + Mathf.RoundToInt((t.transform.position.z) * 1.024f)] * 0.02f);
                   // }
                }
            }
           

                t.terrainData.SetHeights(0, 0, updatedHeightmap);
            
        }
        */
    }

    public void GenerateCompleteTerrainHeightmap()
    {
        terrains.Clear();
        for (int i = 0; i < generatorParent.childCount; i++)
        {
            if (generatorParent.GetChild(i).name.Contains("Tile"))
            {
                terrains.Add(generatorParent.GetChild(i).Find("Main Terrain").gameObject.GetComponent<Terrain>());
            }
        }









        float[,] map = new float[mapWidth+1, mapHeight+1];
        foreach(Terrain t in terrains)
        {
            float[,] updatedHeightmap = t.terrainData.GetHeights(0, 0, 1025, 1025);
            for (int y = 0; y < 1025; y++)
            {
                for (int x = 0; x < 1025; x++)
                {
                    

                        
                    map[y + Mathf.RoundToInt((t.transform.position.x) * 1.024f), x + Mathf.RoundToInt((t.transform.position.z) * 1.024f)] = updatedHeightmap[x, y];
                    
                }
            }
        }
     
      
        riverMesh.GetComponent<RiverMeshGenerator>().terrainHeightmap = map;
        tHeightmap = map;

        
    }




















    //gets the highest point of all the terrains to determine what goes as a max river start point
    private float getMaxTerrainHeight(List<Terrain> ts)
    {
        float maxHeight = 0;
        for(int i=0; i<ts.Count; i++)
        {
            if (ts[i].terrainData.bounds.max.y > maxHeight)
            {
                maxHeight = ts[i].terrainData.bounds.max.y;
            }
        }
        
        return maxHeight;
    }





    //Gets a random point on the map and returns it
    private Vector3 getRiverStartPoint(List<Terrain> ts, float maxHeight)
    {
        float needY = 0;
        Vector3 terrainPos = Vector3.zero;
        
       
            Vector2 spot = new Vector3(Random.Range(0, 1000), Random.Range(0, 1000));
            Terrain t = ts[Random.Range(0, ts.Count)];
            terrainPos = new Vector3(spot.x + t.transform.position.x, 0, spot.y + t.transform.position.z);
            terrainPos.y = t.SampleHeight(terrainPos);
            needY = terrainPos.y;


       
      
            return (terrainPos);
        
        
    }

    



}
