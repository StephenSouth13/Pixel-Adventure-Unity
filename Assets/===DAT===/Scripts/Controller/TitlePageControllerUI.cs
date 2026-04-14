using UnityEngine;
using UnityEngine.UI;

public class TitlePageControllerUI : MonoBehaviour
{
    public GameObject titlePanel;
    public GameObject loadingPanel;
    public GameObject lobbyPanel;
    public Button oNL_btn;
    public Button oFF_btn;
    public Button close_btn;
    public Button createRoom_btn;
    // ===
    private bool isEventSubscribed = false;
    private void Start()
    {
        oNL_btn.onClick.AddListener(() =>
        {
            D_NetworkManager.Instance.ConnectToServer();
        });
        oFF_btn.onClick.AddListener(() =>
        {
            // show offline mode UI
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
