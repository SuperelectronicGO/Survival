using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoliagePlacer : MonoBehaviour
{
    [SerializeField] private int terrainWidth;
    public List<Terrain> terrains = new List<Terrain>();
    [SerializeField] private Transform generatorParent;

    [SerializeField] private Transform parentObject;
    public GameObject[] foliageObjects;
    public bool startedGeneration=false;
    public bool batched = false;


    public int maxFoliageCount;
    private int amountPlaced = 0;
    // Start is called before the first frame update
    void Start()
    {
       for(int i=0; i< generatorParent.childCount; i++)
        {
            if (generatorParent.GetChild(i).name.Contains("Tile"))
            {
                terrains.Add(generatorParent.GetChild(i).Find("Main Terrain").gameObject.GetComponent<Terrain>());
            }
        }
    }
    Terrain placeTerrain(Vector2 tposition)
    {

        for (int j = 0; j < terrains.Count; j++)
        {
            if (terrains[j].gameObject.transform.position.x < tposition.x && terrains[j].transform.position.x > tposition.x - 1000)
            {
                if (terrains[j].gameObject.transform.position.z < tposition.y && terrains[j].transform.position.z > tposition.y - 1000)
                {
                    return terrains[j];

                }
            }
        }
        return terrains[0];
    }
    // Update is called once per frame
    void Update()
    {
        if (startedGeneration&&amountPlaced<maxFoliageCount)
        {
            for (int i = 0; i < 10; i++)
            {
                Terrain t;
                int r = Random.Range(0, terrainWidth);
                int r2 = Random.Range(0, terrainWidth);
                t = terrains[Random.Range(0, terrains.Count + 1)];
                Vector3 spawnPos = new Vector3(r+t.transform.position.x, 0, r2+t.transform.position.z);
                spawnPos.y = t.SampleHeight(spawnPos);
                int foliagePrefab = Random.Range(0, foliageObjects.Length);
                GameObject objToPlace = foliageObjects[foliagePrefab];
                GameObject g = Instantiate(objToPlace, spawnPos, Quaternion.identity, parentObject);
                g.transform.localScale = foliageObjects[foliagePrefab].transform.localScale * Random.Range(0.8f, 1.2f);
                
            }
            amountPlaced += 10;
            
        }
        if (!(amountPlaced < maxFoliageCount))
        {
            batched = true;
        }
    }
}
