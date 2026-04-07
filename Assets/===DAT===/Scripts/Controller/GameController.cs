using UnityEngine;
using Fusion; // Thêm cái này để dùng được GameMode

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    
    [Header("References")]
    [SerializeField] BasicSpawner _spawner; // Kéo BasicSpawner vào ô này trong Inspector
    
    // Lưu trạng thái để các script khác (như Cổng chuyển màn) biết đường mà chạy
    public bool IsOnline { get; private set; }
    public string CurrentRoomID { get; set; } = "PixelRoom";

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

    // --- HÀM KHỞI CHẠY GAME ---

    // Gọi cái này khi bấm nút OFFLINE
    public void StartOffline(int sceneIndex)
    {
        IsOnline = false;
        if (_spawner != null)
        {
            _spawner.StartLevel(GameMode.Single, sceneIndex);
        }
        else
        {
            Debug.LogError("Chưa kéo BasicSpawner vào GameController!");
        }
    }

    // Gọi cái này khi bấm nút ONLINE
    public void StartOnline(int sceneIndex)
    {
        IsOnline = true;
        if (_spawner != null)
        {
            _spawner.StartLevel(GameMode.AutoHostOrClient, sceneIndex, CurrentRoomID);
        }
        else
        {
            Debug.LogError("Chưa kéo BasicSpawner vào GameController!");
        }
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif  
    }
}