using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
public class NetworkManagerUI : MonoBehaviour
{
    public static NetworkManagerUI instance;
    [SerializeField]
    private Button hostButton;
    [SerializeField]
    private Button clientButton;
    public Camera spawnCamera;
    private void Awake()
    {
        instance = this;
        hostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            PlayerNetwork.instance.SpawnPlayerTest();
        });
        clientButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            
            PlayerNetwork.instance.SpawnPlayerTest();
        });
    }
}
