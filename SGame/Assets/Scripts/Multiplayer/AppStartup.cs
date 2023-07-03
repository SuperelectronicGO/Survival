using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
//Thank you to Edward-Dev on unity forms for the workaround for the "Network prefab list was not empty at initialization time" warning (https://forum.unity.com/threads/network-prefabs-not-initializing-correctly.1430224/)
public class AppStartup : MonoBehaviour
{
    [SerializeField] private MaterialSystemLink[] materialsToLoad;
    [SerializeField] private NetworkPrefabsList _networkPrefabsList;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(OnAppStartup());
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
        finishedRegisterNetworkPrefabs = true;
    }
    bool finishedRegisterNetworkPrefabs = false;
    /// <summary>
    /// Adds all materials and their particle systems to the material dictionary
    /// </summary>
    private void AssignMaterialDictionary()
    {
        for(int i = 0; i < materialsToLoad.Length; i++)
        {
            MaterialDictionary.instance.HitParticles.Add(materialsToLoad[i].material, materialsToLoad[i].system);
        }
        finishedAssignMaterialDictionary = true;
    }
    bool finishedAssignMaterialDictionary = false;
    /// <summary>
    /// Coroutine that runs all the app startup functions
    /// </summary>
    /// <returns>After running all app startup funcitons</returns>
    private IEnumerator OnAppStartup()
    {
        RegisterNetworkPrefabs();
        yield return new WaitUntil(() => finishedRegisterNetworkPrefabs == true);
        AssignMaterialDictionary();
        yield return new WaitUntil(() => finishedAssignMaterialDictionary == true);
        yield break;
    }
}
