using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class D_GameManager : MonoBehaviour
{
    public bool isOnline = false; // mặc định là offline, nếu muốn bật online thì set true và thêm các chức năng liên quan đến online sau này
    public bool isOffline = true; // mặc định là offline, nếu muốn bật online thì set false và thêm các chức năng liên quan đến online sau này
    public void SetMode_Onl_Off(bool value)
    {
        isOnline = value;
        isOffline = !value;
    }
    #region Singleton
    public static D_GameManager Instance;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            playerData = new PlayerData();
            playerData.LoadData(); // Load dữ liệu player khi game bắt đầu, nếu có dữ liệu đã lưu trước đó
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    // ----------------------------------------------------------------
    // === Quản Lý Scene ===
    #region Scene Management
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
    
    #endregion
    // ----------------------------------------------------------------
    // === Quản Lý Player Data ===
    #region Player Data Management
    [SerializeField] public PlayerData playerData; // đã khởi tạo trong Awake để đảm bảo không bị null
    public PlayerData GetLocalPlayerData()
    {
        return playerData;
    }
    #endregion
    // ----------------------------------------------------------------

}
