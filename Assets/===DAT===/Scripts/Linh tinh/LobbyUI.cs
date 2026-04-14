using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using JetBrains.Annotations;
using Fusion;
using Fusion.Sockets;
public class LobbyUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_InputField searchInput;
    [SerializeField] private Button search_btn;
    [SerializeField] private Button refresh_btn;
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private Transform roomListContainer;
    private int maxRooms = 10; // Giới hạn số lượng phòng hiển thị
    private Queue<GameObject> roomObjects = new Queue<GameObject>(); // Hàng đợi để quản lý các phòng đã tạo
    // bool isEventSubscribed = false;
    void Awake()
    {
        InitRoom(maxRooms); // Tạo sẵn 5 phòng để test, có thể xóa sau khi tích hợp với mạng
    }
    void OnEnable()
    {
        if(D_NetworkManager.Instance == null)
        {
            Debug.LogWarning("D_NetworkManager instance not found, cannot subscribe to events");
            return;
        }
        D_NetworkManager.Instance.OnRoomListUpdated += UpdateRoomList;
    }
    void OnDisable()
    {
        D_NetworkManager.Instance.OnRoomListUpdated -= UpdateRoomList;
    }
    void Start()
    {
        refresh_btn.onClick.AddListener(() =>
        {
            RefreshRoomList();
        });
        search_btn.onClick.AddListener(() =>
        {
            SearchRoomByID(searchInput.text);
        });
    }
    void InitRoom(int room)
    {
        if(roomListContainer.childCount >= maxRooms)
        {
            return;
        }
        for(int i = 0; i < room; i++)
        {
            GameObject roomObj = Instantiate(roomPrefab, roomListContainer);
            roomObj.SetActive(false); // Tạm thời ẩn phòng mới tạo, có thể chỉnh sửa sau để hiển thị khi có người chơi
            RoomUI roomUI = roomObj.GetComponent<RoomUI>();
            roomUI.UpdateRoomInfo($"Player{i+1}", i+1, 1000 + i, Random.Range(1, 4), 4);
            roomObjects.Enqueue(roomObj); // Thêm phòng mới vào cuối hàng đợi
        }
    }
    void UpdateRoomList(List<SessionInfo> sessionList)
    {
        Debug.Log($"Updating room list with {sessionList.Count} rooms");
        // Ẩn tất cả phòng hiện tại
        foreach (GameObject roomObj in roomObjects)
        {
            roomObj.SetActive(false);
        }
        // Hiển thị lại các phòng dựa trên dữ liệu mới
        for (int i = 0; i < sessionList.Count && i < roomObjects.Count; i++)
        {
            SessionInfo session = sessionList[i];
            GameObject roomObj = roomObjects.ToArray()[i]; // lấy theo index thay vì dequeue
            RoomUI roomUI = roomObj.GetComponent<RoomUI>();
            /// lấy properties của session ;
            int id = session.Properties.ContainsKey("ID") ? (int)session.Properties["ID"] : 0;
            /// cập nhật thông tin phòng dựa trên session
            roomUI.UpdateRoomInfo(session.Name, i + 1, id, session.PlayerCount, session.MaxPlayers);

            roomObj.SetActive(true);
            roomObj.transform.SetParent(roomListContainer, false); // đảm bảo parent đúng
        }
    }
    private void RefreshRoomList()
    {
        UpdateRoomList(D_NetworkManager.Instance.lastSessionList); // Cập nhật lại danh sách phòng với dữ liệu mới nhất
        searchInput.text = ""; // Xóa nội dung tìm kiếm sau khi thực hiện

    }
    private void SearchRoomByID(string keyword)
    {
        int id = int.Parse(keyword); // chuyển string sang int để so sánh
        // Tìm kiếm phòng theo từ khóa và cập nhật UI
        List<SessionInfo> filteredList = D_NetworkManager.Instance.lastSessionList.FindAll(session => session.Properties.ContainsKey("ID") && session.Properties["ID"] == id);

        UpdateRoomList(filteredList);
    }
}
