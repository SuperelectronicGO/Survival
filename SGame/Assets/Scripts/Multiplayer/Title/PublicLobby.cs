using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PublicLobby : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyName;
    public void SetLobbyText(string target)
    {
        lobbyName.text = target;
    }
}
