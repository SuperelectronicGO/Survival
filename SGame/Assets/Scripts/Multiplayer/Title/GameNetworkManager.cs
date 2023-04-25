using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using Unity.Netcode;
using Steamworks.ServerList;
using Steamworks.Ugc;
using System.Threading.Tasks;
public class GameNetworkManager : MonoBehaviour
{
    #region Steam Server Values
    //Instance of this script that can be used from anywhere
    public static GameNetworkManager Instance { get; private set; } = null;
    //The facepunch transport we are using
    public FacepunchTransport transport = null;
    //The current lobby the player resides in
    public Lobby? currentLobby{ get; private set; } = null;
    public SteamServer currentSteamServer;
    //Internet request used to gather a list of available servers
    private Steamworks.ServerList.Internet Request = new Steamworks.ServerList.Internet();
    //The ID of our app (not used)
    public uint gameAppId;
    //Host ID
    public ulong hostId;
    //The SteamClient.Name
    public string PlayerName;
    //The SteamClient.SteamId
    public SteamId playerSteamId;
    //the SteamClient.SteamId as a string
    public string playerSteamIdString;
    #endregion

    //Boolean to keep track of if we want to generate the world upon entering the game
    public bool generateWorld = true;

    /*  Awake function sets the instance of this script, as well as marking this object
     *  as DontDestroyOnLoad, and sets the player playerName, playerSteamId, and its string 
     *  variant. */
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        //SteamClient.Init(gameAppId, asyncCallbacks: true);
        DontDestroyOnLoad(gameObject);
        
    }

    /*  Start function gets the transport, subscribes to the necessary callbacks, and starts
     *  running the coroutine to get the player list. */
    private void Start()
    {
        transport = GetComponent<FacepunchTransport>();
        SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite += OnLobbyInvite;
        SteamMatchmaking.OnLobbyGameCreated += OnLobbyGameCreated;
        SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequested;
        StartCoroutine(UpdateCurrentLobbys());
        PlayerName = SteamClient.Name;
        playerSteamId = SteamClient.SteamId;
        playerSteamIdString = SteamClient.SteamId.ToString();
    }

    //OnDestroy unsubscribes from all callbacks and shuts down the Steam client
    private void OnDestroy()
    {
        Debug.Log("Shutdown everything");
        SteamMatchmaking.OnLobbyCreated -= OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined -= OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave -= OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite -= OnLobbyInvite;
        SteamMatchmaking.OnLobbyGameCreated -= OnLobbyGameCreated;
        SteamFriends.OnGameLobbyJoinRequested -= OnGameLobbyJoinRequested;
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectedCallback;
        
        
        SteamClient.Shutdown();
        
    }
    private void Update()
    {
        SteamClient.RunCallbacks();
    }
    /*  Starts the host and creates a steam lobby for that game. The lobby has its name set as well, 
     *  and starts the coroutine to update the list of players in the lobby. */
    public async void StartHost(int maxMembers, string lobbyName)
    {
        
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;

        if (NetworkManager.Singleton.StartHost())
        {
          currentLobby = await SteamMatchmaking.CreateLobbyAsync(maxMembers);
            currentLobby.Value.SetData("name", lobbyName);
            currentLobby.Value.SetData("password", "amongus");
            //Random Key name and Value to only return lobbys that exist on this games network
            Debug.Log(currentLobby.Value.Id);

        }
        else
        {
            Debug.LogError("Error starting host");
        }
        TitleScreenSelector.instance.finishedLoading = true;
        StartCoroutine(UpdateLobbyPlayerInfo());
    }

    /* Starts the client and subscribes to the OnClientConnected and Disconnected callbacks. 
     * Sets the target steam ID. */
    public void StartClientNative(SteamId id)
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectedCallback;
        transport.targetSteamId = id;
        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("Started Client");
        }
    }

    /* Attempts to join the lobby specified by the lobbyId. */
    public async void JoinLobby(SteamId lobbyId, string password)
    {
        try
        {
            
            currentLobby = await SteamMatchmaking.JoinLobbyAsync(lobbyId);
            if (password != currentLobby.Value.GetData("password")){
                Debug.Log($"Password is wrong! You entered {password} and you needed {currentLobby.Value.GetData("password")}!");
            }
            Debug.Log($"Joined the lobby of {currentLobby.Value.GetData("name")}");
            TitleScreenSelector.instance.finishedLoading = true;
            StartCoroutine(UpdateLobbyPlayerInfo());
        }
        catch
        {
            Debug.LogError("No lobby found");
        }
        
    }
    
    //Disconnects when the app is closed
    private void OnApplicationQuit() => Disconnect();
   
    //Disconnect leaves the current lobby and shuts down the NetworkManager.
    public void Disconnect()
    {
        currentLobby?.Leave();
        if (NetworkManager.Singleton == null) return;

       // NetworkManager.Singleton.Shutdown();
    }
    #region Unity Callbacks
    private void OnServerStarted() => Debug.Log("Server started", this);
    private void OnClientConnectedCallback(ulong clientID) => Debug.Log($"Client connected (ID: {clientID})");

    //When a client disconnects
    private void OnClientDisconnectedCallback(ulong clientID)
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectedCallback;
        Debug.Log($"Client disconnected (ID: {clientID})");
    }
    #endregion

    #region Steam Callbacks
    //When we join lobby within steam client
    private void OnGameLobbyJoinRequested(Lobby lobby, SteamId id) 
    {
        Debug.LogError("This should never happen");
        StartClientNative(lobby.Id);
        Debug.Log("Started with lobby ID " + lobby.Id);
    
    }
    private void OnLobbyMemberJoined(Lobby lobby, Friend friend)
    {
        Debug.Log("Someone joined");
    }
    private void OnLobbyMemberLeave(Lobby lobby, Friend friend)
    {

    }
    private void OnLobbyGameCreated(Lobby lobby, uint ip, ushort port, SteamId id)
    {

    }
    private void OnLobbyInvite(Friend friend, Lobby lobby) => Debug.LogError($"You got an invite from {friend.Name}");
    private void OnLobbyEntered(Lobby lobby)
    {
        TitleScreenSelector.instance.SetLobbyText(lobby.GetData("name"), lobby.Id.ToString());
        if (NetworkManager.Singleton.IsHost) return;
        StartClientNative(currentLobby.Value.Owner.Id);

    }
    private void OnLobbyCreated(Result result, Lobby lobby)
    {
      if(result != Result.OK)
        {
            Debug.LogError($"Lobby couldn't be created {result}", this);
        }
        lobby.SetPublic();
        lobby.SetJoinable(true);
        lobby.SetGameServer(lobby.Owner.Id);
        Debug.Log("Lobby created");
        
    }
    #endregion

    #region Lobby Functions
    //Public void that stops refreshing the lobby list
    public void StopPlayerListRefresh()
    {
        StopCoroutine(UpdateLobbyPlayerInfo());
    }
    //Method that updates the player list in a lobby
    private IEnumerator UpdateLobbyPlayerInfo()
    {
        while (true)
        {
            Friend[] lobbyMembers = new Friend[currentLobby.Value.MemberCount];
            int memberIndex = 0;
            //Sprite[] playerIcons = new Sprite[currentLobby.Value.MemberCount];
            foreach(Friend friend in currentLobby.Value.Members)
            {
               // Image? i = GetFriendImage(lobbyMembers[memberIndex]).Result;
               // Texture2D tex = i?.ConvertImage();
              
                lobbyMembers[memberIndex] = friend;
                memberIndex += 1;
            }
           
            TitleScreenSelector.instance.SetPlayerList(lobbyMembers);

            yield return new WaitForSecondsRealtime(2.5f);
        }
    }

    #region Coroutine that gets the public lobbys
    //Get the public lobbys every minute
    private IEnumerator UpdateCurrentLobbys()
    {
        yield break;
        Request.OnChanges += OnServerUpdated;
        while (true)
        {
            UpdateServerList();
            yield return new WaitForSeconds(60);
        }
      
    }
    //Public void to update the server list
    public void UpdateServerList()
    {
        Request.Cancel();
        TitleScreenSelector.instance.ClearLobbyList();
        Request.RunQueryAsync(15);
    }
   
    //Method for above coroutine that checks if the lobby found is responsive
    private void OnServerUpdated()
    {
       
        //If no responsive servers, return
        if (Request.Responsive.Count == 0) { Debug.Log("No lobbys found"); return; }

        foreach(var s in Request.Responsive)
        {
            ServerResponded(s);
        }

        //Clear list so we don't reproccess (I did not copy this line from the facepunch wiki.)
        Request.Responsive.Clear();
    }
    //Method for above coroutine that acts as a server responds
    void ServerResponded(ServerInfo server)
    {
        if (server.AppId == 2397310)
        {
            TitleScreenSelector.instance.AddLobbyToAvailable(server.Name);
            Debug.Log($"Added {server.Name} to list of public servers");
        }
        
    }
    #endregion

    #endregion
    //Array that returns the players in the current lobby
    public Friend[] GetCurrentPlayers()
    {
        return currentLobby.Value.Members as Friend[];
    }
}
