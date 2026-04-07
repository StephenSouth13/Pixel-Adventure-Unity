using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] GameObject _modeSelectionPanel; // Cái bảng chung chứa các lựa chọn
    [SerializeField] GameObject _roomInputArea;      // Vùng chứa InputField và nút Xác nhận Online
    [SerializeField] GameObject _initialButtons;     // Vùng chứa 2 nút Offline/Online ban đầu
    
    // Tui đã comment dòng này lại vì nó đang gây lỗi CS0246
    // [SerializeField] Main_Panel mainPanel; 

    [Header("References")]
    [SerializeField] BasicSpawner _spawner;         
    [SerializeField] TMP_InputField roomInputField; 

    public void OpenModeSelection(int levelIndex)
    {
        if (GameSessionManager.Instance != null)
            GameSessionManager.Instance.SelectedLevelIndex = levelIndex;
            
        _modeSelectionPanel.SetActive(true);
        
        if (_roomInputArea != null) _roomInputArea.SetActive(false);
        if (_initialButtons != null) _initialButtons.SetActive(true);
    }

    public void ShowRoomInput()
    {
        if (_roomInputArea != null) _roomInputArea.SetActive(true);
        // Ẩn 2 nút cũ đi khi hiện ô nhập mã cho đẹp
        if (_initialButtons != null) _initialButtons.SetActive(false);
    }

    public void SelectOffline()
    {
        if (GameSessionManager.Instance != null)
            GameSessionManager.Instance.IsOnline = false;
        
        _spawner.StartLevel(GameMode.Single, GameSessionManager.Instance.SelectedLevelIndex);
    }

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

    // Nút BACK sẽ gọi hàm này
    public void CloseModeSelection()
    {
        _modeSelectionPanel.SetActive(false);
        
        // Reset lại trạng thái để lần sau mở New Game nó hiện lại từ đầu
        if (_roomInputArea != null) _roomInputArea.SetActive(false);
        if (_initialButtons != null) _initialButtons.SetActive(true);
        if (roomInputField != null) roomInputField.text = "";
    }
}