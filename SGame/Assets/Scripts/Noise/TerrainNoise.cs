using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
public static class TerrainNoise
{
    //Used when generating many maps in a row

     [BurstCompile]
    public struct MapGenerationJob : IJobParallelFor
    {
        public int2 Dimensions;
        public float Scale;
        public int Octaves;
        public float Persistance;
        public float Lacunarity;
        public int2 Offset;
        [ReadOnly]
        public NativeArray<float2> OctaveOffsets;

        [WriteOnly]
        public NativeArray<float> Result;

        public void Execute(int index)
        {
            var halfWidth = (Dimensions.x) / 2;
            var halfHeight = (Dimensions.y) / 2;

            var amplitude = 1f;
            var frequency = 1f;
            var noiseHeight = 0f;

            
            var x = Mathf.RoundToInt((index % Dimensions.x) + Mathf.RoundToInt(Offset.x));
            var y = Mathf.RoundToInt((index / Dimensions.x) + Mathf.RoundToInt(Offset.y));
            
            for (var i = 0; i < Octaves; i++)
            {

                var sampleX = (x - halfWidth) / Scale * frequency + OctaveOffsets[i].x;
                var sampleY = (y - halfHeight) / Scale * frequency + OctaveOffsets[i].y;
                //Multiplied in order to not have just middle values
                var perlinValue = noise.cnoise(new float2(sampleX, sampleY));


                noiseHeight += perlinValue * amplitude;

                amplitude *= Persistance;
                frequency *= Lacunarity;
               
            }
           
           
            Result[index] = noiseHeight;
          
        }
    }

  
     public static float[,] GenerateNoiseMap(int2 dimensions, uint seed, float scale, int octaves, float persistance, float lacunarity, int2 offset)
    {
      //  Debug.Log(oOffsets[0] + ", " + oOffsets[1] + ", " + oOffsets[2] + ", " + oOffsets[3]);
        //Debug.Log("Dimensions: " + dimensions + ", octave offsets: " + oOffsets + ", scale: " + scale + ", persistance: " + persistance + ", lacunarity: " + lacunarity + ", offset" + offset);
        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        var random = new Unity.Mathematics.Random(seed);
        using var jobResult = new NativeArray<float>(dimensions.x * dimensions.y, Allocator.TempJob);
        using var octaveOffsets = new NativeArray<float2>(octaves, Allocator.TempJob);


        for (var i = 0; i < octaves; i++)
        {
            var offsetX = random.NextInt(-100000, 100000);
            var offsetY = random.NextInt(-100000, 100000);
            var nativeOctaveOffsets = octaveOffsets;
            nativeOctaveOffsets[i] = new float2(offsetX, offsetY);
          
        }

        var job = new MapGenerationJob()
        {
            Dimensions = dimensions,
            Lacunarity = lacunarity,
            Octaves = octaves,
            OctaveOffsets = octaveOffsets,
            Persistance = persistance,
            Result = jobResult,
            Scale = scale,
            Offset = offset
        };

        var handle = job.Schedule(jobResult.Length, 128);

        handle.Complete();

        return SmoothNoiseMap(dimensions, jobResult);
       
    }

    //Modified version of voronoi generator created by Berkay Tascl. https://www.youtube.com/@berkaytasc9642
    public static float[,] GenerateVoronoiMap(int dimensions, int regionAmount, uint seed)
    {
       //Define values
       Unity.Mathematics.Random randomAmnt = new Unity.Mathematics.Random(seed);
       float[,] noisemap = new float[dimensions,dimensions];
       float2[] points = new float2[regionAmount];
       float[] weights = new float[regionAmount];
      
       //Create random points and weights
       for (int i = 0; i < regionAmount; i++)
       {
                points[i] = randomAmnt.NextFloat2(0, dimensions);
                weights[i] = randomAmnt.NextFloat(0, 1);
       }


        
        for(int y=0; y<dimensions; y++)
        {
            for(int x=0; x<dimensions; x++)
            {
                float distance = float.MaxValue;
                int value = 0;
                for (int i = 0; i < regionAmount; i++)
                {
                    if (Vector2.Distance(new Vector2(x, y), points[i]) < distance)
                    {
                        distance = Vector2.Distance(new Vector2(x, y), points[i]);
                        value = i;
                    }
                }
                noisemap[x, y] = weights[value];

            }
        }



        return noisemap;
    }
    private static float[,] SmoothNoiseMap(int2 dimensions, NativeArray<float> jobResult)
    {
        var result = new float[dimensions.x, dimensions.y];

       


        for (var y = 0; y < dimensions.y; y++)
        {
            for (var x = 0; x < dimensions.x; x++)
            {
                
                //  result[x, y] = math.unlerp(minNoiseHeight, maxNoiseHeight, result[x, y]);
               
                result[x, y] = math.unlerp(-1.15f,1.15f, jobResult[y * dimensions.x + x]);
                if (result[x, y] > 1)
                {
                    result[x, y] = 1;
                }
                else if (result[x, y] < 0)
                {
                    result[x, y] = 0;
                }
                /*
                if (x == 1023 && y == 512)
                {
                    Debug.Log("Tile one supposed " + result[x,y]);
                }
                if (x == 0 && y == 512)
                {
                    Debug.Log("Tile two supposed " + result[x,y]);
                }
                */
            }
        }

        return result;
    }
}
