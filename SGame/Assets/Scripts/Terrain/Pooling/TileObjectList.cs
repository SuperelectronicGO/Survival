using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObjectList : MonoBehaviour
{
    //List of objects on the tile
    public HashSet<GameObject> objectList = new HashSet<GameObject>();
    //Adds objectList to filtered list if terrain tile is active upon script initialization
    private void Start()
    {
        if (gameObject.activeInHierarchy)
        {
            CustomObjectFiltering.instance.AddTileToList(objectList);
        }
    }
    //Adds objectList to filtered list when enabled
    private void OnEnable()
    {
        CustomObjectFiltering.instance.AddTileToList(objectList);
    }
    //Removes objectList from filtered list when disabled
    private void OnDisable()
    {
        CustomObjectFiltering.instance.RemoveTileFromList(objectList); 
    }

}
