using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : NetworkBehaviour
{
    public GameObject options_Panel;
    public GameObject level_panel;
    public GameObject loading_Panel;
    // -----------------------------------------------------------------------
    // === Main panel ===
    #region MAIN PANEL
    public GameObject main_panel;
    public TextMeshProUGUI playerName_txt;
    public TextMeshProUGUI playerLevel_txt;
    public TextMeshProUGUI experience_txt; // hiển thị dạng "currentEXP / maxEXP"
    public GameObject container_AllMainMenu_BTN;
    public Button newGame_btn;
    public Slider experienceSlider;
    [SerializeField] public GameObject spawnUIPos1;
    [SerializeField] public GameObject spawnUIPos2;
    #endregion
    public AudioController soundController;
    // -----------------------------------------------------------------------
    /// === viết các biến ở đây ===
    D_GameManager gameManager;
    int currentEXP;
    int maxEXP = 20; // tại 1 level -> max = (level + 1) * 10, sau khi đủ EXP để lên cấp thì sẽ reset lại currentEXP về 0 và tăng level lên 1, sau đó maxEXP sẽ được tính lại theo công thức mới

    /// === end ===
    public static GameController Instance;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public override void Spawned()
    {
        base.Spawned();
        Debug.Log("GameController Spawned");

    }
    private void Start()
    {
        StartUpPanel(); // default UI hiển thị.

        if(D_GameManager.Instance == null)
        {
            Debug.LogError("GameManager is not found in the scene.");
            return;
        }
        if(gameManager == null)
        {
            gameManager = D_GameManager.Instance;
        }

        // Sau khi load xong scene thì sẽ gọi SetUpUI để cập nhật lại UI theo dữ liệu đã lưu của player
        SetUpUI(); // sẽ tách onl và off trong cùng 1 hàm này.
    
        SetUpNewGamebtn();
        
    }
    public void StartUpPanel()
    {
        loading_Panel.SetActive(true);
        main_panel.SetActive(false);
        options_Panel.SetActive(false);
        level_panel.SetActive(false);
    }
    public void SetUpUI() // Offline mode
    {
        
        // Phần tên UI này chỉ mang tính chất demo, sau này sẽ được thay thế bằng UI thực tế
        // sẽ được lưu lại mỗi khi vào game, sau đó load lại khi vào game lần sau
        // Sẽ có nhiều loại UI khác nhau, mỗi loại sẽ có một key riêng để lưu vào PlayerPrefs, sau đó load lại khi vào game
        if(D_GameManager.Instance.isOffline)
        {
            var demoObject_UI = D_ObjectPoolManager.Instance.GetObjectPool(
                gameManager.playerData.character + "_UI", 
                spawnUIPos1.transform, 
                spawnUIPos1.transform.position);
            var demoScript_UI = demoObject_UI.GetComponent<DemoPlayer_UI>();
            demoScript_UI.playerName_txt.text = gameManager.playerData.playerName;

        }

        playerName_txt.text = gameManager.playerData.playerName;
        playerLevel_txt.text = gameManager.playerData.level.ToString();
        currentEXP = gameManager.playerData.experience;
        maxEXP = (gameManager.playerData.level + 1) * 10;
        experienceSlider.maxValue = maxEXP;
        experienceSlider.value = currentEXP;
        experienceSlider.onValueChanged.AddListener(OnSliderValueChanged);
        experience_txt.text = $"{currentEXP} / {maxEXP}";
        // vì có hàm OpenMainPanel trong RPC_SetUpUI rồi nên sẽ không cần gọi hàm này ở đây nữa, 
        // vì hàm này sẽ được gọi sau khi spawn xong object UI cho player, 
        // và sẽ được gọi trên tất cả client, bao gồm cả host, 
        // nên sẽ đảm bảo được việc hiển thị main panel đúng thời điểm cho cả host và client
        if(D_GameManager.Instance.isOnline) return;
        loading_Panel.SetActive(false);
        main_panel.SetActive(true);

    }
    void SetUpNewGamebtn()
    {
        if(D_GameManager.Instance.isOffline) return; 
        // gỡ bỏ listener cũ của offline setup thủ công 
        newGame_btn.onClick.RemoveAllListeners();
        // không cho client có quyền new game 
        if(soundController == null) soundController = FindFirstObjectByType<AudioController>();
        if (!D_NetworkManager.Instance.runner.IsServer)
        {
            newGame_btn.onClick.AddListener(() =>
            {
                soundController.PlaySFX(0);
                // chỉ cho play sfx
            });
            return;
        }
        // host sẽ có quyền chuyển scene mới
        newGame_btn.onClick.AddListener(() =>
        {
            soundController.PlaySFX(0);
            soundController.PlayMusic(1);
            container_AllMainMenu_BTN.SetActive(false);
            LoadSceneOnl(2); 
        });
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_OpenMainPanel()
    {
        loading_Panel.SetActive(false);
        main_panel.SetActive(true);
    }
    public void LoadScene(int sceneIndex)
    {
        gameManager.LoadScene(sceneIndex);
    }
    public void LoadSceneOnl(int sceneIndex)
    {
        if(D_NetworkManager.Instance.runner.IsServer)
        {
            D_NetworkManager.Instance.runner.LoadScene(SceneRef.FromIndex(sceneIndex));
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
    void OnSliderValueChanged(float value)
    {
        experience_txt.text = $"{currentEXP} / {maxEXP}";
    }
}
