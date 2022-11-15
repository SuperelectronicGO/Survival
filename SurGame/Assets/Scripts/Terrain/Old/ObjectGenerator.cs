using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    public Vector2 chunkCoord;
    public Mesh mesh;
    public PrefabStorage pStorage;
    public AreaData data;
    bool spawned = false;
    public int treesPerChunk = 200;
    private int currentTrees;
    public MapGenerator mapGenerator;

    
    void Awake()
    {
       
       
           
        
        
    }
    void Update()
    {

        pStorage = GameObject.Find("GameManager").GetComponent<PrefabStorage>();
       
        mesh = this.GetComponent<MeshFilter>().sharedMesh;
        Vector3[] vertices = mesh.vertices;
        foreach (Zone zone in pStorage.data.zones)
        {
            float cx = chunkCoord.x;
            float cy = chunkCoord.y;
            if(cx<= zone.middleChunkCoord.x+zone.size&& cx >= -zone.middleChunkCoord.x+zone.size && cy <= zone.middleChunkCoord.y+zone.size && cy >= -zone.middleChunkCoord.y+zone.size)
            {
                Renderer r = GetComponent<Renderer>(); // assumes the terrain is in a mesh renderer on the same GameObject
                float randomX = Random.Range(r.bounds.min.x, r.bounds.max.x);
                float randomZ = Random.Range(r.bounds.min.z, r.bounds.max.z);

                RaycastHit hit;
                if (Physics.Raycast(new Vector3(randomX, r.bounds.max.y + 5f, randomZ), -Vector3.up, out hit))
                {
                    if (currentTrees <= treesPerChunk)
                    {
                        Instantiate(pStorage.bareTreeMold, hit.point, transform.rotation * Quaternion.Euler(-90f, 0f, 0f));
                        currentTrees += 1;
                    }
                    //transform.rotation * Quaternion.Euler (0f, 180f, 0f)
                }
                else
                {
                    // the raycast didn't hit, maybe there's a hole in the terrain?
                }
            }


        }
        }
}



/*
 *  Renderer r = GetComponent<Renderer>(); // assumes the terrain is in a mesh renderer on the same GameObject
                float randomX = Random.Range(r.bounds.min.x, r.bounds.max.x);
                float randomZ = Random.Range(r.bounds.min.z, r.bounds.max.z);

                RaycastHit hit;
                if (Physics.Raycast(new Vector3(randomX, r.bounds.max.y + 5f, randomZ), -Vector3.up, out hit))
                {
                    if (currentTrees <= treesPerChunk)
                    {
                        Instantiate(pStorage.bareTreeMold, hit.point, transform.rotation * Quaternion.Euler(-90f, 0f, 0f));
                        currentTrees += 1;
                    }
                    //transform.rotation * Quaternion.Euler (0f, 180f, 0f)
                }
                else
                {
                    // the raycast didn't hit, maybe there's a hole in the terrain?
                }
*/