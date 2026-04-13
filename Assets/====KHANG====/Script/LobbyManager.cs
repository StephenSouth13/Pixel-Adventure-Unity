using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameSceneManager : NetworkBehaviour
{
    public BasicSpawner spawner;
    public NetworkPrefabRef berryPrefab;

    public GameObject panelPlayerList;
    public GameObject playerListItemPrefab;

    public Button buttonLeaveRoom;

    void Start()
    {
        spawner = FindFirstObjectByType<BasicSpawner>();

        if (panelPlayerList != null)
            panelPlayerList.SetActive(false);

        if (buttonLeaveRoom != null)
        {
            buttonLeaveRoom.onClick.RemoveAllListeners();
            buttonLeaveRoom.onClick.AddListener(OnClickLeaveRoom);
        }
    }

    private void OnDestroy()
    {
        if (buttonLeaveRoom != null)
            buttonLeaveRoom.onClick.RemoveListener(OnClickLeaveRoom);
    }

    private void OnClickLeaveRoom()
    {
        if (spawner == null)
            spawner = FindFirstObjectByType<BasicSpawner>();

        if (spawner != null)
            spawner.LeaveRoomAndReturnToLobby();
    }

    private void Update()
    {
        if (panelPlayerList == null) return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            panelPlayerList.SetActive(true);
            UpdatePlayerList();
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            panelPlayerList.SetActive(false);
        }
    }

    void UpdatePlayerList()
    {
        if (spawner == null) return;

        var playerDataManager = FindFirstObjectByType<PlayerDataManager>();
        if (playerDataManager == null || panelPlayerList == null || playerListItemPrefab == null) return;

        foreach (Transform child in panelPlayerList.transform)
            Destroy(child.gameObject);

        foreach (var kvp in playerDataManager.Players)
        {
            var metaData = kvp.Value;
            var item = Instantiate(playerListItemPrefab, panelPlayerList.transform);
            var text = item.GetComponent<TextMeshProUGUI>();
            if (text != null)
                text.text = metaData.Name.ToString() + " - " + metaData.Class.ToString() + " - Lv" + metaData.Level;
        }
    }
}