using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class SetTerrainMat : MonoBehaviour
{
    public float[,] tempMap;
    public float[,] humidMap;
    public bool gen = false;
    private TerrainData d;
    public TerrainData[] ds;
    public Terrain t;
    public Terrain[] ts;
    [NonReorderable]
    public Biomee[] biomes;
    public enum transType
    {
        Multi,
        Added,
        highest,
        lowest
    }
    public transType tranType;
    // Start is called before the first frame update
    void Start()
    {
       
          int  tempMapSeed = Random.Range(0, 100000);
        int    humidMapSeed = Random.Range(0, 100000);
        
       
        tempMap = Noise.GenerateNoiseMap(2048,2048, tempMapSeed, 1000, 4, .46f, 4, Vector2.zero);
        humidMap = Noise.GenerateNoiseMap(2048,2048, humidMapSeed,1500, 4, .46f, 4, Vector2.zero);
       
        d = t.terrainData;
        for (int i = 0; i < ts.Length; i++)
        {
            ds[i] = ts[i].terrainData;
        }
        TerrainLayer[] layers = new TerrainLayer[biomes.Length];
        for (int i = 0; i < biomes.Length; i++)
        {
            layers[i] = biomes[i].layer;
        }
        for (int i = 0; i < ds.Length; i++)
        {
            ds[i].terrainLayers = layers;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            textureTerrain();
            gen = false;
        }
    }

    public void textureTerrain()
    {

        for (int i = 0; i < 4; i++)
        {
            float[,,] splatmapData = new float[d.alphamapWidth, d.alphamapHeight, d.alphamapLayers];
            setBiomeSplats();
            for (int y = 0; y < d.alphamapHeight; y++)
            {
                for (int x = 0; x < d.alphamapWidth; x++)
                {







                    for (int j = 0; j < biomes.Length; j++)
                    {

                        if (i == 0)
                        {
                            splatmapData[y, x, j] = biomes[j].splatMap[x, y];
                        }
                        else if (i == 1)
                        {
                            splatmapData[y, x, j] = biomes[j].splatMap[x + d.alphamapWidth, y];
                        }
                        else if (i == 2)
                        {
                            splatmapData[y, x, j] = biomes[j].splatMap[x, y + d.alphamapWidth];

                        }
                        else if (i == 3)
                        {
                            splatmapData[y, x, j] = biomes[j].splatMap[x + d.alphamapWidth, y + d.alphamapWidth];
                        }

                    }
                }
            }
            ds[i].SetAlphamaps(0, 0, splatmapData);
        }
    }
    public void setBiomeSplats()
    {
        for (int i = 0; i < biomes.Length; i++)
        {
            float[,] biomeSpl = new float[(ds[0].alphamapWidth * 2) + 5, (ds[0].alphamapHeight * 2) + 5];
            float changeAmnt = 0.025f;
            float changeMult = 20;


            for (int y = 0; y < ds[0].alphamapHeight * 2; y++)
            {
                for (int x = 0; x < ds[0].alphamapWidth * 2; x++)
                {

                    if (tempMap[x, y] >= biomes[i].minTemp - changeAmnt && tempMap[x, y] <= biomes[i].maxTemp + changeAmnt && humidMap[x, y] >= biomes[i].minHum - changeAmnt && humidMap[x, y] <= biomes[i].maxHum + changeAmnt)
                    {
                        //Solid
                        float tempAmnt = 1;
                        float humAmnt = 1;
                        if (tempMap[x, y] < biomes[i].minTemp + changeAmnt)
                        {
                           tempAmnt = 1 - (((biomes[i].minTemp + changeAmnt) - tempMap[x, y]) * changeMult);
                          // tempAmnt = 0;
                        }
                        else if (tempMap[x, y] > biomes[i].maxTemp - changeAmnt)
                        {

                             tempAmnt = (biomes[i].maxTemp + changeAmnt - tempMap[x, y]) * changeMult;
                            //tempAmnt = 0;

                        }
                        else
                        {
                            //   tempAmnt = 0;
                        }

                        if (humidMap[x, y] < biomes[i].minHum + changeAmnt)
                        {
                             humAmnt = 1 - (((biomes[i].minHum + changeAmnt) - humidMap[x, y]) * changeMult);
                           // humAmnt = 0;
                        }
                        else if (humidMap[x, y] > biomes[i].maxHum - changeAmnt)
                        {
                              humAmnt = (biomes[i].maxHum + changeAmnt - humidMap[x, y]) * changeMult;
                           // humAmnt = 0;
                        }
                        else
                        {
                            // humAmnt = 0;
                        }








                        if (tempMap[x, y] > biomes[i].minTemp + changeAmnt && tempMap[x, y] < biomes[i].maxTemp - changeAmnt && humidMap[x, y] > biomes[i].minHum + changeAmnt && humidMap[x, y] < biomes[i].maxHum - changeAmnt)
                        {
                            biomeSpl[x, y] = 1;
                        }
                        else
                        {
                            //  biomeSpl[x, y] = tempAmnt*humAmnt;
                            switch (tranType)
                            {
                                case transType.Multi:
                                    biomeSpl[x, y] = tempAmnt * humAmnt;
                                    break;

                                case transType.Added:
                                    biomeSpl[x, y] = (tempAmnt / 2) + (humAmnt / 2);
                                    break;

                                case transType.highest:
                                    if (tempAmnt > humAmnt)
                                    {
                                        biomeSpl[x, y] = tempAmnt;
                                    }
                                    else
                                    {
                                        biomeSpl[x, y] = humAmnt;
                                    }
                                    break;
                                case transType.lowest:
                                    if (tempAmnt > humAmnt)
                                    {
                                        biomeSpl[x, y] = humAmnt;
                                    }
                                    else
                                    {
                                        biomeSpl[x, y] = tempAmnt;
                                    }
                                    break;
                            }

                        }

                    }








                }
            }


            biomes[i].splatMap = biomeSpl;



        }
    }

}

[System.Serializable]
public class Biomee
{
    public enum BiomeType
    {
        Swamp,
        Snow,
        Plains,
        Forest,
        Desert,
        Rainforest
    }
    public BiomeType biomeType;
    public TerrainLayer layer;
    public float minTemp;
    public float maxTemp;
    public float minHum;
    public float maxHum;
    public float[,] splatMap;

}
