using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using Controllers; // Đảm bảo namespace này khớp với PlayerController của ông

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkObject _playerPrefab;
    private NetworkRunner _runner;

    // --- HÀM KHỞI TẠO CHÍNH (GỌI TỪ NÚT BẤM MENU) ---
    // mode: Single (Offline) hoặc AutoHostOrClient (Online)
    // levelIndex: Màn muốn vào chơi
    // roomID: Tên phòng nhập từ UI
    // Trong file BasicSpawner.cs
// Đổi tên từ StartGameFromMenu thành StartLevel
public async void StartLevel(GameMode mode, int levelIndex, string roomID = "PixelRoom")
{
    if (_runner != null) return; 

    _runner = gameObject.AddComponent<NetworkRunner>();
    _runner.ProvideInput = true;

    var sceneRef = SceneRef.FromIndex(levelIndex);

    var result = await _runner.StartGame(new StartGameArgs()
    {
        GameMode = mode,
        SessionName = roomID,
        Scene = sceneRef,
        SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
    });

    if (result.Ok)
    {
        Debug.Log($"Đã khởi động thành công: {mode} tại phòng: {roomID}");
    }
    else
    {
        Debug.LogError($"Lỗi: {result.ShutdownReason}");
        Destroy(_runner);
        _runner = null;
    }
}

    // --- TRIỆU HỒI PLAYER ---
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            // Spawn nhân vật tại vị trí an toàn
            runner.Spawn(_playerPrefab, new Vector3(0, 2, 0), Quaternion.identity, player);
        }
    }

    // --- CẦU NỐI INPUT ---
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        // Phải có đoạn này thì PlayerController mới nhận được phím bấm!
        var myPlayer = runner.GetPlayerObject(runner.LocalPlayer);
        if (myPlayer != null)
        {
            var controller = myPlayer.GetComponent<PlayerController>();
            if (controller != null) controller.OnInput(runner, input);
        }
    }

    // --- CÁC CALLBACK BẮT BUỘC (GIỮ NGUYÊN ĐỂ KHÔNG LỖI) ---
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    // XÓA OnGUI() ĐI VÌ MÌNH SẼ DÙNG NÚT BẤM UI TRÊN CANVAS
}