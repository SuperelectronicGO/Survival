using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapGenerator
{

	
}

/*
 * 		float[,] gradientMapTL;
	 float[,] gradientMapTR;
	 float[,] gradientMapBL;
	 float[,] gradientMapBR;

	 float[,] gradientMapT;
	 float[,] gradientMapB;
	 float[,] gradientMapL;
	 float[,] gradientMapR;

	 float[,] gradientMapWhite;

		

	 float[,] falloffMap;
HeightMap GenerateMapData(Vector2 centre)
{
	float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, centre + noiseData.offset, noiseData.normalizeMode);

	if (terrainData.useFalloff)
	{

		if (falloffMap == null)
		{
			falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize + 2);
		}


		for (int y = 0; y < mapChunkSize + 2; y++)
		{
			for (int x = 0; x < mapChunkSize + 2; x++)
			{
				if (terrainData.useFalloff)
				{
					noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);

				}


			}
		}

	}

	if (terrainData.useGradient)
	{

		if (gradientMapBR == null)
		{
			gradientMapBR = GradientGenerator.GenerateTopLeftGradientMap(mapChunkSize + 2);
		}
		if (gradientMapBL == null)
		{
			gradientMapBL = GradientGenerator.GenerateTopRightGradientMap(mapChunkSize + 2);
		}
		if (gradientMapTR == null)
		{
			gradientMapTR = GradientGenerator.GenerateBottomLeftGradientMap(mapChunkSize + 2);
		}
		if (gradientMapTL == null)
		{
			gradientMapTL = GradientGenerator.GenerateBottomRightGradientMap(mapChunkSize + 2);
		}

		if (gradientMapT == null)
		{
			gradientMapT = GradientGenerator.GenerateLeftGradientMap(mapChunkSize + 2);
		}
		if (gradientMapR == null)
		{
			gradientMapR = GradientGenerator.GenerateTopGradientMap(mapChunkSize + 2);
		}
		if (gradientMapB == null)
		{
			gradientMapB = GradientGenerator.GenerateRightGradientMap(mapChunkSize + 2);
		}
		if (gradientMapL == null)
		{
			gradientMapL = GradientGenerator.GenerateBottomGradientMap(mapChunkSize + 2);
		}

		if (gradientMapWhite == null)
		{
			gradientMapWhite = GradientGenerator.GenerateOceanGradientMap(mapChunkSize + 2);
		}

		float cx = Mathf.Round((noiseData.offset.x + centre.x) / mapChunkSize);
		float cy = Mathf.Round((noiseData.offset.y + centre.y) / mapChunkSize);

		if (cx == terrainData.worldSize && cy == -terrainData.worldSize)
		{


			for (int y = 0; y < mapChunkSize + 2; y++)
			{
				for (int x = 0; x < mapChunkSize + 2; x++)
				{

					noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - gradientMapBR[x, y]);




				}
			}
		}
		if (cx == terrainData.worldSize && cy == terrainData.worldSize)
		{


			for (int y = 0; y < mapChunkSize + 2; y++)
			{
				for (int x = 0; x < mapChunkSize + 2; x++)
				{

					noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - gradientMapBL[x, y]);




				}
			}
		}
		if (cx == -terrainData.worldSize && cy == terrainData.worldSize)
		{


			for (int y = 0; y < mapChunkSize + 2; y++)
			{
				for (int x = 0; x < mapChunkSize + 2; x++)
				{

					noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - gradientMapTL[x, y]);




				}
			}
		}
		if (cx == -terrainData.worldSize && cy == -terrainData.worldSize)
		{


			for (int y = 0; y < mapChunkSize + 2; y++)
			{
				for (int x = 0; x < mapChunkSize + 2; x++)
				{

					noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - gradientMapTR[x, y]);




				}
			}
		}

		if (!(cx == -terrainData.worldSize) && !(cx == terrainData.worldSize) && cy == -terrainData.worldSize)
		{


			for (int y = 0; y < mapChunkSize + 2; y++)
			{
				for (int x = 0; x < mapChunkSize + 2; x++)
				{

					noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - gradientMapT[x, y]);




				}
			}
		}
		if (!(cx == -terrainData.worldSize) && !(cx == terrainData.worldSize) && cy == terrainData.worldSize)
		{


			for (int y = 0; y < mapChunkSize + 2; y++)
			{
				for (int x = 0; x < mapChunkSize + 2; x++)
				{

					noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - gradientMapB[x, y]);




				}
			}
		}

		if (!(cy == -terrainData.worldSize) && !(cy == terrainData.worldSize) && cx == terrainData.worldSize)
		{


			for (int y = 0; y < mapChunkSize + 2; y++)
			{
				for (int x = 0; x < mapChunkSize + 2; x++)
				{

					noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - gradientMapR[x, y]);




				}
			}
		}
		if (!(cy == -terrainData.worldSize) && !(cy == terrainData.worldSize) && cx == -terrainData.worldSize)
		{


			for (int y = 0; y < mapChunkSize + 2; y++)
			{
				for (int x = 0; x < mapChunkSize + 2; x++)
				{

					noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - gradientMapL[x, y]);




				}
			}
		}

		if ((cy > terrainData.worldSize) || cy < -terrainData.worldSize)
		{


			for (int y = 0; y < mapChunkSize + 2; y++)
			{
				for (int x = 0; x < mapChunkSize + 2; x++)
				{

					noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - gradientMapWhite[x, y]);




				}
			}
		}
		if ((cx > terrainData.worldSize) || cx < -terrainData.worldSize)
		{


			for (int y = 0; y < mapChunkSize + 2; y++)
			{
				for (int x = 0; x < mapChunkSize + 2; x++)
				{

					noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - gradientMapWhite[x, y]);




				}
			}
		}
	}


	return new HeightMap(noiseMap);
}


	



else if (drawMode == DrawMode.TopLeftGradientMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(GradientGenerator.GenerateTopLeftGradientMap(mapChunkSize)));
		}
		else if (drawMode == DrawMode.TopRightGradientMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(GradientGenerator.GenerateTopRightGradientMap(mapChunkSize)));
		}
		else if (drawMode == DrawMode.BottomLeftGradientMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(GradientGenerator.GenerateBottomLeftGradientMap(mapChunkSize)));
		}
		else if (drawMode == DrawMode.BottomRightGradientMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(GradientGenerator.GenerateBottomRightGradientMap(mapChunkSize)));
		}
		else if (drawMode == DrawMode.TopGradientMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(GradientGenerator.GenerateTopGradientMap(mapChunkSize)));
		}
		else if (drawMode == DrawMode.BottomGradientMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(GradientGenerator.GenerateBottomGradientMap(mapChunkSize)));
		}
		else if (drawMode == DrawMode.LeftGradientMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(GradientGenerator.GenerateLeftGradientMap(mapChunkSize)));
		}
		else if (drawMode == DrawMode.RightGradientMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(GradientGenerator.GenerateRightGradientMap(mapChunkSize)));
		}
*/