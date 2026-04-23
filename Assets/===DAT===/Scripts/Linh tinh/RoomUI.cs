using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class RoomUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button join_btn;
    [SerializeField] private TextMeshProUGUI roomPos_txt;
    [SerializeField] private TextMeshProUGUI playerName_txt; // tên player == tên phòng
    [SerializeField] private TextMeshProUGUI playerCount_txt;
    [SerializeField] public TextMeshProUGUI id_txt;
    private void Start()
    {
        join_btn.onClick.AddListener(OnJoinButtonClicked);
    }
    private void OnJoinButtonClicked()
    {
        Debug.Log("Join button clicked");
        if(playerName_txt.text == "")
        {
            Debug.LogWarning("Player name is empty, cannot join room");
            return;
        }
        D_NetworkManager.Instance.JoinRoom(playerName_txt.text);
        // D_NetworkManager.Instance.JoinRoom();
    }
    public void UpdateRoomInfo(string playerName, int roomPos, int roomId, int playerCount, int maxPlayers)
    {
        playerName_txt.text = playerName;
        roomPos_txt.text = $"{roomPos}";
        id_txt.text = $"{roomId}";
        playerCount_txt.text = $"{playerCount}/{maxPlayers}";
    }
}
