using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TitlePageControllerUI : MonoBehaviour
{
    [Header("enter NAME panel")]
        public GameObject enterNamePanel;
        public TMP_InputField nameInputField;
        public Button enterName_btn;
        public Button closeEnterName_btn;
    [Header("TITLE panel")]
        public GameObject titlePanel;
        public Button oNL_btn;
        public Button oFF_btn;
    [Header("LOBBY panel")]
        public GameObject loadingPanel;
        public GameObject lobbyPanel;
        public Button close_btn;
        public Button createRoom_btn;
    // === viết các biến ở đây ===
    private bool isEventSubscribed = false;
    // === end ===
    private void Start()
    {
       
        if(string.IsNullOrEmpty(D_GameManager.Instance.playerData.playerName))
        {
            enterNamePanel.gameObject.SetActive(true);
            titlePanel.gameObject.SetActive(false);
            lobbyPanel.gameObject.SetActive(false);
            loadingPanel.gameObject.SetActive(false);
        }
        else
        {
            enterNamePanel.gameObject.SetActive(false);
            titlePanel.gameObject.SetActive(true);
            lobbyPanel.gameObject.SetActive(false);
            loadingPanel.gameObject.SetActive(false);
        }

        oNL_btn.onClick.AddListener(() =>
        {
            D_NetworkManager.Instance.ConnectToServer();
            D_GameManager.Instance.SetMode_Onl_Off(true); // bật chế độ online khi nhấn nút Online
        });
        oFF_btn.onClick.AddListener(() =>
        {
            D_GameManager.Instance.SetMode_Onl_Off(false); // tắt chế độ online khi nhấn nút Offline
            D_AudioManager.Instance.PlayMusic(1); 
            D_GameManager.Instance.LoadScene(1); // load scene main (title scene -> main scene  )
        });
        close_btn.onClick.AddListener(() =>
        {
            // exit lobby panel
            D_NetworkManager.Instance.OnClickBack();
        });
        createRoom_btn.onClick.AddListener(() =>
        {
            // show create room UI
            D_NetworkManager.Instance.CreateRoom();
        });
        enterName_btn.onClick.AddListener(() =>
        {
            string playerName = nameInputField.text;
            if(!string.IsNullOrEmpty(playerName))
            {
                D_GameManager.Instance.playerData.SetPlayerName(playerName);
                enterNamePanel.gameObject.SetActive(false);
                titlePanel.gameObject.SetActive(true);
            }
        });
        closeEnterName_btn.onClick.AddListener(() =>
        {
            string playerName = D_GameManager.Instance.playerData.playerName;
            if(!string.IsNullOrEmpty(playerName)) 
            {
                // Nếu đã có tên rồi thì vẫn cho phép đóng panel nhập tên để vào title panel
                enterNamePanel.gameObject.SetActive(false);
                titlePanel.gameObject.SetActive(true);
            }
            else
            {
                // Nếu chưa có tên nào được nhập thì random một cái tên mặc định để cho phép người chơi tiếp tục vào title panel
                string defaultName = $"Player{Random.Range(1000, 9999)}";
                D_GameManager.Instance.playerData.SetPlayerName(defaultName);
                enterNamePanel.gameObject.SetActive(false);
                titlePanel.gameObject.SetActive(true);
            }
        });
        if(!isEventSubscribed)
        {
            OnEnable();
        }
    }
    public void OnEnable() 
    {
        if(D_NetworkManager.Instance == null)
        {
            return;
        }
        isEventSubscribed = true;
        D_NetworkManager.Instance.onConnectLobby += HandleConnected;
        D_NetworkManager.Instance.onFailed += HandleFailed;
        D_NetworkManager.Instance.onDisconnected += HandleDisconnected;
        D_NetworkManager.Instance.onLoading += ShowLoadingPanel;
        Debug.Log("[OnEnable] Subscribed to network events");
    }
    public void OnDisable()
    {
        D_NetworkManager.Instance.onConnectLobby -= HandleConnected;
        D_NetworkManager.Instance.onFailed -= HandleFailed;
        D_NetworkManager.Instance.onDisconnected -= HandleDisconnected;
        D_NetworkManager.Instance.onLoading -= ShowLoadingPanel;
        Debug.Log("[OnDisable] Unsubscribed from network events");
    }
    private void HandleConnected()
    {
        HideLoadingPanel();
        lobbyPanel.gameObject.SetActive(true);
        titlePanel.gameObject.SetActive(false);
        Debug.Log("Connected to server, showing lobby panel");
        // load lobby scene
    }
    private void HandleFailed()
    {
        Debug.LogError("Failed to connect to server");

        // show error message
    }
    private void HandleDisconnected()
    {
        Debug.Log("Disconnected from server, returning to title page");
        HideLoadingPanel();
        lobbyPanel.gameObject.SetActive(false);
        titlePanel.gameObject.SetActive(true);
        // return to title page
    }
    private void ShowLoadingPanel()
    {
        loadingPanel.gameObject.SetActive(true);
    }
    private void HideLoadingPanel()
    {
        loadingPanel.gameObject.SetActive(false);
    }


}
