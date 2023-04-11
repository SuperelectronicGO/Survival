using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PlayerListTemplate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;
    public void SetPlayerName(string nameToSet)
    {
        playerName.text = nameToSet;
    }
    
}
