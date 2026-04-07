using UnityEngine;

public class GameSessionManager : MonoBehaviour
{
    public static GameSessionManager Instance;

    public bool IsOnline = false;
    public string RoomID = "DefaultRoom";
    public int SelectedLevelIndex = 1;

    private void Awake()
    {
        // Giữ Manager này không bị xóa khi chuyển cảnh
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
}