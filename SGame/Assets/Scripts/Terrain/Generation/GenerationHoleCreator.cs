using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationHoleCreator : MonoBehaviour
{
    [NonReorderable]
    public SquareHole[] holes;
    public Transform holeParent;
    public Transform terrainPosition;
    
}


[System.Serializable]
public class SquareHole{
    public Transform pos1;
    public Transform pos2;
    public bool[,] CreateHoleArray(Vector3 pos1, Vector3 pos2)
    {
        
        bool[,] holes = new bool[(int)Mathf.Abs(pos2.z - pos1.z), (int)Mathf.Abs(pos2.x - pos1.x)];
        return holes;

    }
    public Vector3 PositionToTerrainHeightmap(Vector3 terrainPosition, Vector3 anchorPosition)
    {
        Vector3 position = anchorPosition - terrainPosition;
        position = new Vector3((int)position.x * 1.024f, 0, (int)position.z * 1.024f);

        return position;
    }
}

