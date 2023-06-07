using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.Mathematics;
public class CustomObjectFiltering : NetworkBehaviour
{
    //Active player
    private Transform activePlayer;
    //Active tile list of objects
    private List<HashSet<GameObject>> activeTiles = new List<HashSet<GameObject>>();
    //Max distance objects can be before they are disabled
    [SerializeField] private int maxObjectDistance = 500;
    //Dictionary holding the terrains and their positions
    public Dictionary<float2, TileObjectList> terrainPositionList = new Dictionary<float2, TileObjectList>();
    //Static instance of the pooler to be referenced
    public static CustomObjectFiltering instance { get; private set; }
    private void Awake()
    {
        //Assign instance
        instance = this;
    }
    /// <summary>
    /// Coroutine that runs every couple seconds to active objects close to the player
    /// </summary>
    /// <returns>Never returns</returns>
    private IEnumerator CheckTileObjects()
    {
        while (true)
        {
            for (int i = 0; i < activeTiles.Count; i++)
            {
                FilterObjects(activeTiles[i]);
                yield return new WaitForSecondsRealtime(0.5f);
            }
            yield return new WaitForSecondsRealtime(2);
        }
    }
    /// <summary>
    /// Enables/Disables objects for a tile depending on their distance from the player
    /// </summary>
    /// <param name="objectList">The hash set containing the objects</param>
    public void FilterObjects(HashSet<GameObject> objectList)
    {
        foreach (GameObject g in objectList)
        {
            if (Vector2.Distance(new Vector2(activePlayer.position.x, activePlayer.position.z), new Vector2(g.transform.position.x, g.transform.position.z)) < maxObjectDistance)
            {
                g.SetActive(true);
            }
            else
            {
                g.SetActive(false);
            }
        }
    }
    /// <summary>
    /// Method that sets all the objects in a hash set inactive
    /// </summary>
    /// <param name="objectList">The hash set of objects to reference</param>
    public void SetAllInactive(HashSet<GameObject> objectList)
    {
        foreach (GameObject g in objectList)
        {
            g.SetActive(false);
        }
    }
    /// <summary>
    /// Sets the target player for the object pooling
    /// </summary>
    /// <param name="playerObject">The transform of the player to set</param>
    public void SetActivePlayer(Transform playerObject)
    {
        activePlayer = playerObject;
    }
    /// <summary>
    /// Adds a terrain tile to the list of filtered objects
    /// </summary>
    /// <param name="tileList">The list of gameObjects to add</param>
    public void AddTileToList(HashSet<GameObject> tileList)
    {
        activeTiles.Add(tileList);
    }
    /// <summary>
    /// Removes a terrain tile from the list of filtered objects
    /// </summary>
    /// <param name="tileList">The list of gameObjects to remove</param>
    public void RemoveTileFromList(HashSet<GameObject> tileList)
    {
        try
        {
            activeTiles.Remove(tileList);
        }
        catch
        {
            throw new NullReferenceException("Trying to remove a list that doesn't exist!");
        }
    }
    /// <summary>
    /// Starts the object pooling coroutine
    /// </summary>
    public void StartObjectPooling()
    {
        StartCoroutine(CheckTileObjects());
    }

}

