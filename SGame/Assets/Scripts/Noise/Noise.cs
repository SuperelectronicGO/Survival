using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
public static class Noise
{
    /*
	 * Generates a noise map
	 * 
	 * mapWidth/mapHeight : how long and tall the generated heightmap is
	 * seed: a random seed to get a random map
	 * scale: how scaled up or down the end heightmap is
	 * octaves: used to make the noisemap more smooth
	 * persistance: only the gods know
	 * lacunarity: only the gods know
	 * offset: how far shifted up/down and left/right the graph is
	 */
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;


        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }



    /*
	 *Generates a river heightmap 
	 * 
	 * mapWidth/mapHeight: how long and tall the end heightmap is. Should match the size of the target terrain group
	 * startPos: where on the map the river starts. Is generated through script
	 * riverDiameter: how far across the river is
	 */
    public static float[,] GenerateRiverMap(int mapWidth, int mapHeight, Vector3 startPos, float riverDiameter)
    {
		float[,] Map = new float[mapWidth, mapHeight];
		float directionalChange = 0;
		
		
		for (int y = 0; y < mapHeight; y++)
		{
			
			for (int x = 0; x < mapWidth; x++)
			{
                if (x > (startPos.x - (riverDiameter / 2)) && x < startPos.x + (riverDiameter / 2)&&y>=startPos.z)
                {
					Map[x, y] = 1-(Mathf.Abs(startPos.x-x)/(riverDiameter /2));
                }
                else
                {
					Map[x, y] = 0;
                }
					
				
			}
			
			if (y > startPos.z)
			{
				directionalChange += UnityEngine.Random.Range(-.03f, .03f);
				directionalChange = Mathf.Clamp(directionalChange, -0.3f, 0.3f);
				//startPos.x += Mathf.Sin(y / 40);
				startPos.x += directionalChange;
			}
			
		}
		return Map;
	}

}