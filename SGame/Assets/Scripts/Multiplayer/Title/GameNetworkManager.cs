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
    //The SteamClient.Name
    public string PlayerName;
    //The SteamClient.SteamId
    public SteamId playerSteamId;
    //the SteamClient.SteamId as a string
    public string playerSteamIdString;

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
        DontDestroyOnLoad(gameObject);
        PlayerName = SteamClient.Name;
        playerSteamId = SteamClient.SteamId;
        playerSteamIdString = SteamClient.SteamId.ToString();
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
    }

    //OnDestroy unsubscribes from all callbacks and shuts down the Steam client
    private void OnDestroy()
    {
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

    /*  Starts the host and creates a steam lobby for that game. The lobby has its name set as well, 
     *  and starts the coroutine to update the list of players in the lobby. */
    public async void StartHost(int maxMembers, string lobbyName)
    {
        
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;

        if (NetworkManager.Singleton.StartHost())
        {
          currentLobby = await SteamMatchmaking.CreateLobbyAsync();
            currentLobby.Value.SetData("name", lobbyName);
            currentLobby.Value.SetData("password", "amongus");
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
    public void StartClient(SteamId id)
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectedCallback;
        transport.targetSteamId = id;

        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("Client has joined", this);
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

        NetworkManager.Singleton.Shutdown();
    }
    #region Unity Callbacks
    private void OnServerStarted() => Debug.Log("Server started", this);
    private void OnClientConnectedCallback(ulong clientID) => Debug.Log($"Client connected (ID: {clientID})");
    private void OnClientDisconnectedCallback(ulong clientID)
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectedCallback;
        Debug.Log($"Client disconnected (ID: {clientID})");
    }
    #endregion

    #region Steam Callbacks
    private void OnGameLobbyJoinRequested(Lobby lobby, SteamId id) => StartClient(lobby.Id);
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
        StartClient(lobby.Id);
        
    }
    private void OnLobbyCreated(Result result, Lobby lobby)
    {
      if(result != Result.OK)
        {
            Debug.LogError($"Lobby couldn't be created {result}", this);
        }
        lobby.SetPublic();
        lobby.SetJoinable(true);
        Debug.Log("Lobby created");
        
    }
    #endregion

    #region Scene Load Callbacks
    public void SpawnPlayerOnSceneLoad()
    {

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
    private async Task<Image?> GetFriendImage(Friend friend)
    {
        var avatar = GetAvatarFromFriend(friend);
        await Task.WhenAll(avatar);
        Image? i = avatar.Result;
        return i;
        
    }
    #region Coroutine that gets the public lobbys
    private IEnumerator UpdateCurrentLobbys()
    {
        Request.OnChanges += OnServerUpdated;
        Request.AddFilter("", "sus");
        while (true)
        {
            
            TitleScreenSelector.instance.ClearLobbyList();
            Request.RunQueryAsync(10);
            yield return new WaitForSeconds(11);
        }
    }
    //Method for above coroutine
    private void OnServerUpdated()
    {
       
        return;
        //If no responsive servers, return
        if (Request.Responsive.Count == 0) { Debug.Log("No lobbys found"); return; }

        foreach(var s in Request.Responsive)
        {
            ServerResponded(s);
        }

        //Clear list so we don't reproccess (I did not copy this line from the facepunch wiki.)
        Request.Responsive.Clear();
    }
    //Method for above coroutine
    void ServerResponded(ServerInfo server)
    {
        TitleScreenSelector.instance.AddLobbyToAvailable(server.Name);
        Debug.Log($"Added {server.Name} to list of public servers");
        
    }
    #endregion

    private static async Task<Image?> GetAvatarFromFriend(Friend friend)
    {
        try
        {
            // Get Avatar using await
            return await friend.GetLargeAvatarAsync();
        }
        catch (Exception e)
        {
            // If something goes wrong, log it
            Debug.Log(e);
            return null;
        }
    }
    #endregion
}
[Serializable]
public static class Conversions
{
    public static Texture2D ConvertImage(this Image image)
    {
        // Create a new Texture2D
        var avatar = new Texture2D((int)image.Width, (int)image.Height, TextureFormat.ARGB32, false);

        // Set filter type, or else its really blury
        avatar.filterMode = FilterMode.Trilinear;

        // Flip image
        for (int x = 0; x < image.Width; x++)
        {
            for (int y = 0; y < image.Height; y++)
            {
                var p = image.GetPixel(x, y);
                avatar.SetPixel(x, (int)image.Height - y, new UnityEngine.Color(p.r / 255.0f, p.g / 255.0f, p.b / 255.0f, p.a / 255.0f));
            }
        }

        avatar.Apply();
        return avatar;
    }
}
