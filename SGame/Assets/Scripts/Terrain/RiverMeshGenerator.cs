using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class RiverMeshGenerator : MonoBehaviour
{
   [Header("Settings")]
   
    public int width = 10240;
    public int height = 10240;
    [Tooltip("Height output number in mapmagic generator")]
    public int terrainHeight = 800;







    //Private components
    Mesh mesh;
    List<Vector3> v;
    List<int> t;
    Vector3[] vertices;
    int[] triangles;
    public float[,] riverHeightmap;
    public float[,] terrainHeightmap;
    float[,] updatedMap;
    float[,] verticeIndex;
    List<Vector3> gizom = new List<Vector3>();
    // Start is called before the first frame update
    void Start()
    {
      
        

       
    }
    public void generateMesh(bool inEditor)
    {
        if (inEditor)
        {
            FindObjectOfType<RiverGeneration>().GenerateRiverComponents();
        }
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        
        createRiverMesh();
        updateRiverMesh();
    }



    

   
    void createRiverMesh()
    {
       
        //Set scale of river mesh
        this.transform.localScale = new Vector3(1 / 1.024f, 1, 1 / 1.024f);
        this.transform.position = new Vector3(0, -1, 0);
        this.transform.eulerAngles = new Vector3(0, 0, 0);

      
        vertices = new Vector3[(width + 1) * (height + 1)];
         v = new List<Vector3>();
        t = new List<int>();
        updatedMap = new float[width+1, height+1];
        verticeIndex = new float[width + 1, height + 1];
        int vertPos = 0;
        for (int i=0, z = 0; z<=height; z++)
        {
            //Find smallest Y value on row in order to not have weird water heights
            float smallestY = 100000;
            
            for (int x = 0; x <= width; x++)
            {

              

              
                  float sampledHeight = terrainHeightmap[x,z]*terrainHeight;
                
                if (sampledHeight < smallestY&&riverHeightmap[x,z]!=0)
                {
                    smallestY = sampledHeight;
                }
               
            }
           
           
            //Actually set heights
            for (int x = 0; x <= width; x++)
            {
                vertices[i] = new Vector3(x, 0, z);
                updatedMap[x, z] = 0;
                if (riverHeightmap[x, z] > 0)
                {

                    //  vertices[i].y = terrainHeightmap[x,z]*terrainHeight;
                    vertices[i].y = smallestY;
               
                    v.Add(vertices[i]);
                    updatedMap[x, z] = 1;
                    verticeIndex[x, z] = vertPos;
                    vertPos += 1;
                }
                    
                    
                
                
                i++;

            }
        }

       
        vertices = v.ToArray();
       
        t.Clear();
        for(int y=0; y<height; y++)
        {
            for(int x=0; x<width; x++)
            {
                //Generate triangles - First pass: lower left
                if (updatedMap[x, y] == 1 && updatedMap[x, y + 1] == 1 && updatedMap[x + 1, y] == 1)
                {
                    //Debug.Log("triangle uses vertices" + verticeIndex[x, y] + ", " + verticeIndex[x, y + 1] + ", and " + verticeIndex[x + 1, y] + ".");
                    t.Add(Mathf.RoundToInt(verticeIndex[x, y]));
                    t.Add(Mathf.RoundToInt(verticeIndex[x, y+1]));
                    t.Add(Mathf.RoundToInt(verticeIndex[x+1, y]));
                }

                //Generate triangles - second pass: top right
                if (updatedMap[x + 1, y] == 1 && updatedMap[x, y + 1] == 1 && updatedMap[x + 1, y + 1] == 1)
                {
                    t.Add(Mathf.RoundToInt(verticeIndex[x+1, y]));
                    t.Add(Mathf.RoundToInt(verticeIndex[x, y + 1]));
                    t.Add(Mathf.RoundToInt(verticeIndex[x + 1, y+1]));
                }
            }
        }

       
       
    }
   
    void updateRiverMesh()
    {
        mesh.Clear();
        mesh.vertices = v.ToArray();
        mesh.triangles = t.ToArray();
        mesh.RecalculateNormals();
    }

   
}
