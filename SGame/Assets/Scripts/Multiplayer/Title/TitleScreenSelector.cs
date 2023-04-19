using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class TitleScreenSelector : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject[] Screens;
    [Header("Join Lobby Screen")]
    [SerializeField] private TMP_InputField lobbyIDEnter;
    [SerializeField] private TMP_InputField lobbyPasswordEnter;
    [Header("Create Lobby Screen")]
    [SerializeField] private TMP_InputField lobbyNameEnter;
    [SerializeField] private RectTransform maxPlayerSelector;
    [SerializeField] private RectTransform[] maxPlayerButtonTransforms;
    [SerializeField] private int maxLobbyPlayers = 1;
    [Header("Lobby Screen")]
    [SerializeField] private TextMeshProUGUI lobbyName;
    [SerializeField] private TextMeshProUGUI lobbyIdText;
    [SerializeField] private TextMeshProUGUI lobbyChatText;
    [SerializeField] private Button loadMainSceneButton;
    [SerializeField] private TMP_InputField lobbyChatEnter;
    private string lobbyId;
    [SerializeField] private GameObject[] playerList;
    [SerializeField] private Transform playerListAnchor;
    [SerializeField] private GameObject playerListDisplayTemplate;
    [SerializeField] private Button copyLobbyIdButton;
    [Header("Browse Lobby Screen")]
    [SerializeField] private List<GameObject> availableLobbys = new List<GameObject>();
    [SerializeField] private GameObject publicLobbyTemplate;
    [SerializeField] private Transform publicLobbyAnchor;
    [Header("Loading Screen")]
    public bool finishedLoading;
    public static TitleScreenSelector instance { get; private set; }
    private void Start()
    {
        instance = this;
        copyLobbyIdButton.onClick.AddListener(() =>
        {
            CopyLobbyID();
        });
       
    }
    // Start is called before the first frame update
    //Sets screen to title screen
    public void SetScreenToMain()
    {
        FilterScreens(0);
    }
    //Set screen to lobby select (Host, Join, ect)
    public void SetScreenToLobbySelect()
    {
        FilterScreens(1);
    }
    //Set screen to join lobby
    public void SetScreenToJoinLobby()
    {
        FilterScreens(3);
    }
    //Set screen to browse lobbys
    public void SetScreenToBrowseLobbys()
    {
        FilterScreens(6);
    }
    //Calls the StartHost function on Steams side and sets the screen to the lobby
    public void HostLobby()
    {
        GameNetworkManager.Instance.StartHost(maxLobbyPlayers, lobbyNameEnter.text);
        StartCoroutine(QueueScreen(4));
    }
    //Attempts to join a lobby with a specified ID
    public void JoinLobby()
    {
        string IdStringEntered = lobbyIDEnter.text;
        Steamworks.SteamId Id =  new Steamworks.SteamId();
        Id.Value = ulong.Parse(IdStringEntered);
        GameNetworkManager.Instance.JoinLobby(Id, lobbyPasswordEnter.text);
        StartCoroutine(QueueScreen(4));

        //Don't allow starting game if we are the host
        if (!NetworkManager.Singleton.IsHost)
        {
            loadMainSceneButton.interactable = false;
        }
    }
    public void AddLobbyToAvailable(string lobbyName)
    {
        GameObject g = Instantiate(publicLobbyTemplate, new Vector3(publicLobbyAnchor.transform.position.x, (availableLobbys.Count * 50) + 50, 0), Quaternion.identity, publicLobbyAnchor.transform);
        g.GetComponent<PublicLobby>().SetLobbyText(lobbyName);
        availableLobbys.Add(g);
    }
    //Function to update the list of lobbys
    public void ClearLobbyList()
    {
        foreach(GameObject i in availableLobbys)
        {
            Destroy(i);
        }
        availableLobbys.Clear();
    }
    //Set screen to create lobby settings
    public void SetScreenToCreateLobby()
    {
        FilterScreens(2);
    }
    //Queues a screen to show after loading is done
    public IEnumerator QueueScreen(int screen)
    {
        //Set the screen to the loading screen
        FilterScreens(5);
        while (!finishedLoading)
        {
            yield return new WaitForEndOfFrame();
        }
        FilterScreens(screen);
        finishedLoading = false;
        yield break;
    }
    //Method that takes a screen as an index and sets the proper screen active
    public void FilterScreens(int selectedIndex)
    {
        for(int i=0; i<Screens.Length; i++)
        {
            if (i == selectedIndex)
            {
                Screens[i].SetActive(true);
            }
            else
            {
                Screens[i].SetActive(false);
            }
        }
    }
    //Public void to set lobby name
    public void SetLobbyText(string name, string lobbyIdString)
    {
        lobbyName.text = name;
        lobbyIdText.text = "Lobby ID: "+lobbyIdString;
        lobbyId = (string)lobbyIdString;
    }
    //Public void to update the player list
    public void SetPlayerList(Steamworks.Friend[] members)
    {
        for(int i=0; i<playerListAnchor.transform.childCount; i ++)
        {
            Destroy(playerListAnchor.transform.GetChild(i).gameObject);
        }
        for(int i=0; i<members.Length; i++)
        {
            GameObject g = Instantiate(playerListDisplayTemplate, playerListAnchor.position, Quaternion.identity, playerListAnchor);
            g.transform.position -= new Vector3(0, 28 * canvas.scaleFactor * (i+1), 0);
            g.GetComponent<PlayerListTemplate>().SetPlayerName(members[i].Name);
           
        }
    }
    //Public void to update the max players
    public void SetMaxPlayers(int max)
    {
        maxLobbyPlayers = max;
        switch (max)
        {
            case 1:
                maxPlayerSelector.position = new Vector3(-60, -32, 0);
                break;
            case 2:
                maxPlayerSelector.position = new Vector3(-20, -32, 0);
                break;
            case 3:
                maxPlayerSelector.position = new Vector3(20, -32, 0);
                break;
            case 4:
                maxPlayerSelector.position = new Vector3(60, -32, 0);
                break;


        }
        maxPlayerSelector.position = maxPlayerButtonTransforms[max-1].position;
    }
    //Public void to copy the lobby ID
    public void CopyLobbyID()
    {
        GUIUtility.systemCopyBuffer = lobbyId;
        
    }
    //Public void to load the main world scene
    public void LoadMainScene()
    {
        GameNetworkManager.Instance.StopPlayerListRefresh();
        Debug.Log("Loading Scene [Main]...");
        NetworkManager.Singleton.SceneManager.LoadScene("Main", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}


