using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public struct PlayerInputData : INetworkInput
{
    public Vector2 Direction;
    public bool Attack;
}

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _runner { get; set; }

    public LobbyManager LobbyManager;

    [SerializeField] private string lobbySceneName = "Lobby";
    [SerializeField] private NetworkPrefabRef _playerPrefab;

    private readonly Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new();

    private bool _isReturningToLobby;
    private bool _isProcessingShutdown;
    private bool _callbacksRegistered;

    public bool IsInLobby { get; private set; }
    public bool IsStartingLobby { get; private set; }
    public bool IsInRoom { get; private set; }

    public PlayerProfile LocalPlayerProfile { get; private set; }

    private void Awake()
    {
        var spawners = FindObjectsByType<BasicSpawner>(FindObjectsSortMode.None);
        if (spawners.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        EnsureRunner();
        DontDestroyOnLoad(gameObject);
    }

    private void EnsureRunner()
    {
        if (_runner == null)
            _runner = GetComponent<NetworkRunner>() ?? gameObject.AddComponent<NetworkRunner>();
    }

    private void RegisterCallbacks()
    {
        if (_runner == null || _callbacksRegistered) return;
        _runner.AddCallbacks(this);
        _callbacksRegistered = true;
    }

    public void SetLocalPlayerProfile(PlayerProfile profile)
    {
        LocalPlayerProfile = profile;
    }

    public async void LeaveRoomAndReturnToLobby()
    {
        if (_isReturningToLobby || _isProcessingShutdown) return;

        _isReturningToLobby = true;

        if (_runner == null)
        {
            await ReturnToLobbyAndRestart();
            _isReturningToLobby = false;
            return;
        }

        try
        {
            await _runner.Shutdown();
        }
        catch (Exception e)
        {
            Debug.LogError($"Shutdown failed: {e.Message}");
            await ReturnToLobbyAndRestart();
            _isReturningToLobby = false;
        }
    }

    public async Task StartLobby()
    {
        if (IsStartingLobby || IsInLobby) return;

        EnsureRunner();

        IsStartingLobby = true;
        _runner.ProvideInput = true;
        RegisterCallbacks();

        var result = await _runner.JoinSessionLobby(SessionLobby.ClientServer);
        IsStartingLobby = false;

        if (result.Ok)
        {
            IsInLobby = true;
            IsInRoom = false;
            Debug.Log("Joined lobby successfully!");
        }
        else
        {
            Debug.LogError($"Failed to join lobby: {result.ShutdownReason}");
        }
    }

    public async Task StartHost(string sessionName, SceneRef scene)
    {
        EnsureRunner();
        RegisterCallbacks();

        IsInLobby = false;
        IsInRoom = true;

        var result = await _runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Host,
            SessionName = sessionName,
            Scene = scene,
            SceneManager = GetComponent<NetworkSceneManagerDefault>() ??
                           gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        if (result.Ok)
        {
            Debug.Log("Started host successfully!");
        }
        else
        {
            IsInRoom = false;
            Debug.LogError($"Failed to start host: {result.ShutdownReason}");
        }
    }

    public async Task StartClient(string sessionName)
    {
        EnsureRunner();
        RegisterCallbacks();

        IsInLobby = false;
        IsInRoom = true;

        var result = await _runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Client,
            SessionName = sessionName,
            SceneManager = GetComponent<NetworkSceneManagerDefault>() ??
                           gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        if (result.Ok)
        {
            Debug.Log("Started client successfully!");
        }
        else
        {
            IsInRoom = false;
            Debug.LogError($"Failed to start client: {result.ShutdownReason}");
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer) return;

        var spawnPosition = new Vector2(Random.Range(0, 5), Random.Range(0, 5));
        var networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);

        if (!_spawnedCharacters.ContainsKey(player))
            _spawnedCharacters.Add(player, networkPlayerObject);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out var networkObject))
        {
            if (networkObject != null && networkObject.Runner != null)
                runner.Despawn(networkObject);

            _spawnedCharacters.Remove(player);
        }
    }

    public async void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        if (_isProcessingShutdown) return;
        _isProcessingShutdown = true;

        Debug.Log($"OnShutdown: {shutdownReason}");

        IsInLobby = false;
        IsInRoom = false;
        IsStartingLobby = false;

        _spawnedCharacters.Clear();

        // Hủy đăng ký callback trực tiếp từ runner được trả về
        if (runner != null)
        {
            runner.RemoveCallbacks(this);
        }
        _callbacksRegistered = false;

        // Xóa cả Runner và SceneManager cũ để dọn dẹp sạch sẽ
        if (_runner != null)
        {
            var oldRunner = _runner;
            _runner = null;

            var oldSceneManager = GetComponent<NetworkSceneManagerDefault>();
            if (oldSceneManager != null)
            {
                Destroy(oldSceneManager);
            }

            if (oldRunner != null)
            {
                Destroy(oldRunner);
            }
        }

        await ReturnToLobbyAndRestart();

        _isProcessingShutdown = false;
        _isReturningToLobby = false;
    }

    private async Task ReturnToLobbyAndRestart()
    {
        await ReturnToLobbySceneOnly();

        if (this == null) return;

        LobbyManager = FindFirstObjectByType<LobbyManager>();

        // Nhờ dùng LoadSceneAsync ở hàm dưới, lúc này chắc chắn active scene đã là Lobby
        if (SceneManager.GetActiveScene().name == lobbySceneName)
        {
            await StartLobby();
        }
    }

    private async Task ReturnToLobbySceneOnly()
    {
        if (this == null) return;

        var currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != lobbySceneName)
        {
            // Sử dụng LoadSceneAsync và bắt hàm đợi cho đến khi isDone = true
            var asyncOperation = SceneManager.LoadSceneAsync(lobbySceneName);
            while (!asyncOperation.isDone)
            {
                await Task.Yield();
            }
        }
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log($"Disconnected from server: {reason}");
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new PlayerInputData
        {
            Direction = new Vector2(
                Input.GetAxis("Horizontal"),
                Input.GetAxis("Vertical")),
            Attack = Input.GetKeyDown(KeyCode.Y)
        };

        input.Set(data);
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("Session list updated, total sessions: " + sessionList.Count);

        if (LobbyManager == null)
            LobbyManager = FindFirstObjectByType<LobbyManager>();

        if (LobbyManager != null)
            LobbyManager.DisplayRoomList(sessionList);
    }

    public void DespawnGO(NetworkObject networkObject)
    {
        if (_runner != null && networkObject != null)
            _runner.Despawn(networkObject);
    }

    public NetworkObject SpawnGO(NetworkPrefabRef prefab, Vector2 position, Quaternion rotation, PlayerRef owner = default)
    {
        if (_runner != null)
            return _runner.Spawn(prefab, position, rotation, owner);

        return null;
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
}