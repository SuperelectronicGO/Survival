using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.Mathematics;
using Random = UnityEngine.Random;
public class MapGenerator : MonoBehaviour
{
	public bool testing = false;
	public bool randomSeed = true;

	public int mapWidth;
	public int mapHeight;
	public float noiseScale;

	public int octaves;
	[Range(0, 1)]
	public float persistance;
	public float lacunarity;

	public int seed;
	public Vector2 offset;

	

	public IslandGenerator[] gen;


	[Header("Biome Heat Map")]
	public int heatMapWidth;
	public int heatMapHeight;
	public float heatMapNoiseScale;
	public int heatMapOctaves;
	[Range(0, 1)]
	public float heatMapPersistance;
	public float heatMapLacunarity;
	public int heatMapSeed;
	public Vector2 heatMapOffset;

	public float[,] noiseMaps;
	public float[,] heatMaps;

	[Header("River Map")]
	public int riverMapWidth;
	public int riverMapHeight;
	[SerializeField] private RiverGeneration riverGen;
	[SerializeField] private FoliageGenerator foliageGen;
	//Editor
	public bool autoUpdate;
	public enum drawModes
    {
		noiseMap,
		heatMap,
		riverMap,
		Mesh

    }
	public drawModes drawmodes;
	void Start()
    {/*
		Debug.Log(SaveSystem.checkForFile());
        if (!SaveSystem.checkForFile())
        {
			noiseMaps = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
			heatMaps = Noise.GenerateNoiseMap(heatMapWidth, heatMapHeight, heatMapSeed, heatMapNoiseScale, heatMapOctaves, heatMapPersistance, heatMapLacunarity, heatMapOffset);
			SaveSystem.saveMaps(this);
        }
        else
        {
			NoiseMapData data = SaveSystem.loadMapData();
			noiseMaps = data.noiseMap;
			heatMaps = data.heatMap;
        }
		
		*/
        if (randomSeed)
        {
			seed = Random.Range(0, 100000);
			heatMapSeed = Random.Range(0, 100000);
        }
		int2 mapDemensions = new int2(mapWidth, mapHeight);
		/*
		if (testing)
		{

			float[,] noiseMap = Noise.GenerateNoiseMap(mapDemensions, (uint)seed, noiseScale, octaves, persistance, lacunarity, offset);
			float[,] heatMap = Noise.GenerateNoiseMap(mapDemensions, (uint)heatMapSeed, heatMapNoiseScale, heatMapOctaves, heatMapPersistance, heatMapLacunarity, heatMapOffset);


			foreach (IslandGenerator generator in gen)
			{
				generator.noiseMap = noiseMap;
				generator.heatMap = heatMap;
			}
        }
        else
        {
			float[,] noiseMap = Noise.GenerateNoiseMap(mapDemensions, (uint)seed, noiseScale, octaves, persistance, lacunarity, offset);
			float[,] heatMap = Noise.GenerateNoiseMap(mapDemensions, (uint)heatMapSeed, heatMapNoiseScale, heatMapOctaves, heatMapPersistance, heatMapLacunarity, heatMapOffset);
			foliageGen.noiseMap = noiseMap;
			foliageGen.heatMap = heatMap;
		}
	*/
	}
	public void GenerateMap()
	{

		if (randomSeed)
		{
			seed = Random.Range(0, 100000);
			heatMapSeed = Random.Range(0, 100000);
		}
		int2 mapDemensions = new int2(mapWidth, mapHeight);

		MapDisplay display = FindObjectOfType<MapDisplay>();
		if (drawmodes == drawModes.noiseMap)
		{
			float[,] noiseMap = BetterNoise.GenerateNoiseMap(mapDemensions, (uint)seed, noiseScale, octaves, persistance, lacunarity, offset);
		display.DrawNoiseMap(noiseMap);
        }
        else if(drawmodes==drawModes.heatMap)
        {
		//	float[,] heatMap = Noise.GenerateNoiseMap(mapDemensions, (uint)heatMapSeed, heatMapNoiseScale, heatMapOctaves, heatMapPersistance, heatMapLacunarity, heatMapOffset);
		//	display.DrawNoiseMap(heatMap);
        }
		
		else
        {
		//	float[,] riverMap = Noise.GenerateRiverMap(riverMapWidth, riverMapHeight,Vector3.zero,25);
		//	display.DrawNoiseMap(riverMap);
        }
		
	}
	
		
		
	
	void OnValidate()
	{
		if (mapWidth < 1)
		{
			mapWidth = 1;
		}
		if (mapHeight < 1)
		{
			mapHeight = 1;
		}
		if (lacunarity < 1)
		{
			lacunarity = 1;
		}
		if (octaves < 0)
		{
			octaves = 0;
		}
		if (heatMapWidth < 1)
		{
			heatMapWidth = 1;
		}
		if (heatMapHeight < 1)
		{
			heatMapHeight = 1;
		}
		if (heatMapLacunarity < 1)
		{
			heatMapLacunarity = 1;
		}
		if (heatMapOctaves < 0)
		{
			heatMapOctaves = 0;
		}
	}

}