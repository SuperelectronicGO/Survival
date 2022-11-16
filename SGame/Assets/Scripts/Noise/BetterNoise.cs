using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
public static class BetterNoise
{
    //Used when generating many maps in a row

   // [BurstCompile(FloatPrecision.Standard, FloatMode.Default, CompileSynchronously =false, OptimizeFor =OptimizeFor.FastCompilation)]
    [BurstCompile]
    public struct MapGenerationJob : IJobParallelFor
    {
        public int2 Dimensions;
        public float Scale;
        public int Octaves;
        public float Persistance;
        public float Lacunarity;
        public float2 Offset;
        [ReadOnly]
        public NativeArray<float2> OctaveOffsets;

        [WriteOnly]
        public NativeArray<float> Result;

        public void Execute(int index)
        {
            var halfWidth = Dimensions.x / 2;
            var halfHeight = Dimensions.y / 2;

            var amplitude = 1f;
            var frequency = 1f;
            var noiseHeight = 0f;

            var x = (index % Dimensions.x)+Offset.x;
            var y = (index / Dimensions.x)+Offset.y;

            for (var i = 0; i < Octaves; i++)
            {
                //   var sampleX = ((x+Offset.x) - halfWidth) / Scale * frequency + OctaveOffsets[i].x;
                // var sampleY = ((y+Offset.y) - halfHeight) / Scale * frequency + OctaveOffsets[i].y;
                   var sampleX = (x - halfWidth) / Scale * frequency + OctaveOffsets[i].x;
                 var sampleY = (y - halfHeight) / Scale * frequency + OctaveOffsets[i].y;
                var perlinValue = noise.cnoise(new float2(sampleX, sampleY));
           //     var perlinValue = Mathf.PerlinNoise(sampleX, sampleY)/Persistance;
                if (perlinValue > 1)
                {
                    perlinValue = 1;
                }
                if (perlinValue < -1)
                {
                    perlinValue = -1;
                }

                noiseHeight += perlinValue * amplitude;

                amplitude *= Persistance;
                frequency *= Lacunarity;
            }

            Result[index] = noiseHeight;
        }
    }

    public static float[,] GenerateNoiseMap(int2 dimensions, uint seed, float scale, int octaves, float persistance, float lacunarity, float2 offset)
    {
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

    private static float[,] SmoothNoiseMap(int2 dimensions, NativeArray<float> jobResult)
    {
        var result = new float[dimensions.x, dimensions.y];

        var maxNoiseHeight = float.MinValue;
        var minNoiseHeight = float.MaxValue;

        for (var y = 0; y < dimensions.y; y++)
        {
            for (var x = 0; x < dimensions.x; x++)
            {
                var noiseHeight = jobResult[y * dimensions.x + x];

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                result[x, y] = noiseHeight;
            }
        }

        for (var y = 0; y < dimensions.y; y++)
        {
            for (var x = 0; x < dimensions.x; x++)
            {
                minNoiseHeight = -1.15f;
                maxNoiseHeight = 1.15f;
                result[x, y] = math.unlerp(minNoiseHeight, maxNoiseHeight, result[x, y]);
                if (result[x, y] > 1)
                {
                    result[x, y] = 1;
                }else if (result[x, y] <0)
                {
                    result[x, y] = 0;
                }
            }
        }

        return result;
    }
}
