using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientGenerator : MonoBehaviour
{
    public static float[,] GenerateTopLeftGradientMap(int size)
    {
        float[,] map = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = i / (float)size  *2-1;
                float y = j / (float)size *2 -1;
                
                float value = Mathf.Max(x,y);
              
                map[i, j] = Evaluate(value);
            }
        }
        return map;
    }
    public static float[,] GenerateTopRightGradientMap(int size)
    {
        float[,] map = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                float value = Mathf.Max(x, -y);
                map[i, j] = Evaluate(value);
            }
        }
        return map;
    }
    public static float[,] GenerateBottomRightGradientMap(int size)
    {
        float[,] map = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                float value = Mathf.Max(-x, -y);
                map[i, j] = Evaluate(value);
            }
        }
        return map;
    }
    public static float[,] GenerateBottomLeftGradientMap(int size)
    {
        float[,] map = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                float value = Mathf.Max(-x, y);
                map[i, j] = Evaluate(value);
            }
        }
        return map;
    }
    public static float[,] GenerateTopGradientMap(int size)
    {
        float[,] map = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;
                y = 0;
                float value = Mathf.Max(x, y);
                map[i, j] = Evaluate(value);
            }
        }
        return map;
    }
    public static float[,] GenerateBottomGradientMap(int size)
    {
        float[,] map = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;
                y = 0;
                float value = Mathf.Max(-x, y);
                map[i, j] = Evaluate(value);
            }
        }
        return map;
    }
    public static float[,] GenerateLeftGradientMap(int size)
    {
        float[,] map = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;
                x=0;
                float value = Mathf.Max(x, y);
                map[i, j] = Evaluate(value);
            }
        }
        return map;
    }
    public static float[,] GenerateRightGradientMap(int size)
    {
        float[,] map = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;
                x = 0;
                float value = Mathf.Max(x, -y);
                map[i, j] = Evaluate(value);
            }
        }
        return map;
    }

    public static float[,] GenerateOceanGradientMap(int size)
    {
        float[,] map = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;
                x = 1;
                y = 1;
                float value = Mathf.Max(x, y);
                map[i, j] = Evaluate(value);
            }
        }
        return map;
    }
    static float Evaluate(float value)
    {
        float a = 1f;
        float b = 5f;
        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
}
