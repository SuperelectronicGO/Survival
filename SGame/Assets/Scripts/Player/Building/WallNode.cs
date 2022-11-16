using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallNode : MonoBehaviour
{
    public enum NodeType
    {
        Left,
        Right
    }
    public NodeType nodeType;
    [SerializeField] private GameObject player;

    private BuildingManager buildManager;
    // Start is called before the first frame update
    void Start()
    {
        buildManager = FindObjectOfType<BuildingManager>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = player.transform.position;
        playerPos.y = 0;
        Vector3 thisPos = transform.position;
        thisPos.y = 0;


        if (Vector3.Distance(playerPos, thisPos) < 20)
        {
            if (!buildManager.currentNodes.Contains(this.gameObject))
            {
                buildManager.currentNodes.Add(this.gameObject);
            }
        }
        else
        {
            if (buildManager.currentNodes.Contains(this.gameObject))
            {
                buildManager.currentNodes.Remove(this.gameObject);
            }
        }
    }
    
}
