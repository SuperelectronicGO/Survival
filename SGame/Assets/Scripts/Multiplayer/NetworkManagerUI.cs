using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField]
    private Button hostButton;
    [SerializeField]
    private Button clientButton;
    [SerializeField]
    private Camera spawnCamera;
    private void Awake()
    {
        hostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            spawnCamera.gameObject.SetActive(false);
        });
        clientButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            spawnCamera.gameObject.SetActive(false);
        });
    }
}
