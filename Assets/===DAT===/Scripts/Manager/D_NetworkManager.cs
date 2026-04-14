using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion.Sockets;
using System;
using System.Collections.Generic;

public class D_NetworkManager : MonoBehaviour , INetworkRunnerCallbacks
{
    public static D_NetworkManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private NetworkRunner runner;
    [Header("Network RunTime")]
    [SerializeField] private bool isConnecting;
    [SerializeField] private bool isCanceled;
    public List<SessionInfo> lastSessionList; // Lưu trữ danh sách phòng mới nhất để replay cho UI khi cần

    // === event ===
    public event Action onConnectLobby;
    public event Action onFailed;
    public event Action onDisconnected;
    public event Action onLoading;
    public event Action<List<SessionInfo>> OnRoomListUpdated;
    public event Action<PlayerRef> OnPlayerJoinedEvent;
    public event Action<PlayerRef> OnPlayerLeftEvent;
    // === end event ===
    void Start()
    {
        InitRunner();
    }
    public void InitRunner() // cần phải gọi hàm này trước khi gọi ConnectToServer() để khởi tạo runner
    {
        if(runner!= null)
        {
            Debug.LogWarning("Runner already initialized");
            return;
        }
        GameObject runnerObj = new GameObject("NetworkRunner");
        runner = runnerObj.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;
        if(D_NetworkManager.Instance == null)
        {
            Debug.LogWarning("D_NetworkManager instance not found, cannot add callbacks");
            return;
        }
        runner.AddCallbacks(D_NetworkManager.Instance);
    }
    public async void ConnectToServer()
    {
        if(isConnecting)
        {
            Debug.LogWarning("Already connecting to server");
            return;
        }
        isConnecting = true;
        isCanceled = false;
        onLoading?.Invoke();
        InitRunner();
        var result = await runner.JoinSessionLobby(SessionLobby.ClientServer);

        if (isCanceled)
        {
            Debug.Log("Connection canceled → ignore result");
            return;
        }

        isConnecting = false;

        if (result.Ok)
        {
            Debug.Log("Connected to server successfully");
            // vào lobby
            onConnectLobby?.Invoke();
        }
        else
        {
            Debug.LogError("Failed to connect to server");
            onFailed?.Invoke();
            // báo lỗi
        }
    }
    public async void OnClickBack() 
    {
        if(isCanceled)
        {
            Debug.LogWarning("Already canceled connection");
            return;
        }
        isCanceled = true;
        isConnecting = false;
        onLoading?.Invoke();
        if(runner != null)
        {
            await runner.Shutdown();
            runner = null;
            onDisconnected?.Invoke();
        }
        // hide Loading UI
        Debug.Log("Closed connection to server");
    }
    public async void CreateRoom()
    {
        onLoading?.Invoke();
        await runner.Shutdown(); // bắt buộc tắt runner cũ(Lobby) nếu có để tránh lỗi khi tạo room mới
        runner = null;

        InitRunner();
        var props = new Dictionary<string, SessionProperty>() // biến lưu trữ các thuộc tính của phòng, có thể thêm sau này nếu cần thiết (map, mode,...) để khi join có thể lấy ra và hiển thị cho người chơi dễ chọn phòng hơn
        {
            {"ID", UnityEngine.Random.Range(1000, 9999)}, // tạo ID phòng ngẫu nhiên
             // có thể thêm các thuộc tính khác như map, mode,... sau này
        };
        var result = await runner.StartGame(new StartGameArgs()
        {
           GameMode = GameMode.Host,
           SessionName = "Room_" + UnityEngine.Random.Range(1000, 9999), // tạo tên phòng ngẫu nhiên
           // SessionName - để tạm - sau kết nối Firebase sẽ lấy tên của player làm tên phòng
           SceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
           SessionProperties = props
        });
        if (result.Ok)
        {
            Debug.Log("Room created successfully");
            D_GameManager.Instance.LoadScene(1);
            // load game scene
        }
        else
        {
            Debug.LogError("Failed to create room");
            onFailed?.Invoke();
            // báo lỗi
        }
    }
    public async void JoinRoom(string sessionName)
    {
        onLoading?.Invoke();
        await runner.Shutdown(); // bắt buộc tắt runner cũ(Lobby) nếu có để tránh lỗi khi join room mới
        runner = null;

        InitRunner();

        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = sessionName, // tên phòng cần join
            SceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
        if (result.Ok)
        {
            Debug.Log("Joined room successfully");
            // load game scene
            D_GameManager.Instance.LoadScene(1);

        }
        else
        {
            Debug.LogError("Failed to join room");
            onFailed?.Invoke();
            // báo lỗi
        }
    }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        lastSessionList = sessionList;

        OnRoomListUpdated?.Invoke(sessionList);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player joined: {player}");
        OnPlayerJoinedEvent?.Invoke(player);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player left: {player}");
        OnPlayerLeftEvent?.Invoke(player);
    }

    // Các callback khác có thể để trống nếu không dùng
    // public void OnConnectedToServer(NetworkRunner runner) {}
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("[Fusion][OnConnectedToServer]Connected to server");
    }
     public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.LogError($"[Fusion][OnConnectFailed]Failed to connect to server: {reason}");
    }
     public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.LogWarning($"[Fusion][OnDisconnectedFromServer]Disconnected from server: {reason}");
    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {}
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {}
    public void OnInput(NetworkRunner runner, NetworkInput input) {}
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {}
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) {}
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) {}
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) {}
    public void OnSceneLoadStart(NetworkRunner runner) {}
    public void OnSceneLoadDone(NetworkRunner runner) {}
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {}
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {}
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {}
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {}
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {}
    
}
