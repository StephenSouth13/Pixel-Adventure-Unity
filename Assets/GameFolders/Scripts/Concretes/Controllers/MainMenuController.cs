using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] GameObject _modeSelectionPanel; // Cái bảng chung chứa các lựa chọn
    [SerializeField] GameObject _roomInputArea;      // Vùng chứa InputField và nút Xác nhận Online
    [SerializeField] GameObject _initialButtons;     // (Tùy chọn) Vùng chứa 2 nút Offline/Online ban đầu

    [Header("References")]
    [SerializeField] BasicSpawner _spawner;         
    [SerializeField] TMP_InputField roomInputField; 

    /// <summary>
    /// Gọi khi bấm New Game hoặc Choose Level
    /// </summary>
    public void OpenModeSelection(int levelIndex)
    {
        if (GameSessionManager.Instance != null)
            GameSessionManager.Instance.SelectedLevelIndex = levelIndex;
            
        _modeSelectionPanel.SetActive(true);
        
        // Mặc định ẩn phần nhập ID đi để người dùng chọn mode trước
        if (_roomInputArea != null) _roomInputArea.SetActive(false);
        if (_initialButtons != null) _initialButtons.SetActive(true);
    }

    /// <summary>
    /// Gọi khi người dùng nhấn nút "ONLINE" lần đầu
    /// </summary>
    public void ShowRoomInput()
    {
        if (_roomInputArea != null) _roomInputArea.SetActive(true);
        // Nếu ông muốn ẩn 2 nút Offline/Online cũ đi cho gọn thì dùng dòng dưới:
        // if (_initialButtons != null) _initialButtons.SetActive(false);
    }

    /// <summary>
    /// Gắn vào nút "OFFLINE"
    /// </summary>
    public void SelectOffline()
    {
        if (GameSessionManager.Instance != null)
            GameSessionManager.Instance.IsOnline = false;
            
        _spawner.StartLevel(GameMode.Single, GameSessionManager.Instance.SelectedLevelIndex);
    }

    /// <summary>
    /// Gắn vào nút "XÁC NHẬN VÀO PHÒNG" (Nằm trong RoomInputArea)
    /// </summary>
    public void SelectOnline() 
    {
        string id = (roomInputField != null && !string.IsNullOrEmpty(roomInputField.text)) 
                    ? roomInputField.text : "PixelRoom";
        
        if (GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.IsOnline = true;
            GameSessionManager.Instance.RoomID = id;
        }
        
        _spawner.StartLevel(GameMode.AutoHostOrClient, GameSessionManager.Instance.SelectedLevelIndex, id);
    }

    /// <summary>
    /// Nút để đóng cái bảng chọn mode nếu người dùng đổi ý
    /// </summary>
    public void CloseModeSelection()
    {
        _modeSelectionPanel.SetActive(false);
    }
}