using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
//Thank you to Edward-Dev on unity forms for the workaround for the "Network prefab list was not empty at initialization time" warning (https://forum.unity.com/threads/network-prefabs-not-initializing-correctly.1430224/)
public class AppStartup : MonoBehaviour
{
    [SerializeField] private NetworkPrefabsList _networkPrefabsList;
    // Start is called before the first frame update
    void Start()
    {
        RegisterNetworkPrefabs();
    }
    /// <summary>
    /// Adds all prefabs to the NetworkManager
    /// </summary>
    private void RegisterNetworkPrefabs()
    {
        var prefabs = _networkPrefabsList.PrefabList.Select(x => x.Prefab);
        foreach(var prefab in prefabs)
        {
            NetworkManager.Singleton.AddNetworkPrefab(prefab);
        }
    }
}
