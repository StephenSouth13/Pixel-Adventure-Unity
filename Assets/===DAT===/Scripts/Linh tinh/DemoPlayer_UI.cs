using TMPro;
using UnityEngine;
using UnityEngine.UI;
using  Fusion;
public class DemoPlayer_UI : NetworkBehaviour
{
    public Image demo_img;
    public TextMeshProUGUI playerName_txt;
    string lastPlayerName = "Player"; 
    [Networked] public float spinValueZ { get; set; }
    NetworkObject N_object;
    PlayerNetworkData playerNetworkData;
    void Awake()
    {
        N_object = GetComponent<NetworkObject>();
        playerNetworkData = GetComponent<PlayerNetworkData>();
        
    }
    private void Update()
    {
        if(demo_img == null) return;
        // SpinToTarget();
    }
    void Start()
    {
        Button btn = gameObject.AddComponent<Button>();
        btn.onClick.AddListener(() => {
            Despawned(N_object.Runner, N_object.HasStateAuthority);
        });
    }
    public override void Spawned()
    {
        base.Spawned();
        Debug.Log("DemoPlayer_UI Spawned");
        SetUpSpawnPosition(N_object.InputAuthority);
        if (N_object.HasStateAuthority) // chỉ host có quyền khởi tạo
        {
            spinValueZ = 90f; // khởi tạo giá trị ban đầu
            GameController.Instance.Rpc_OpenMainPanel();
        }
        if(N_object.HasInputAuthority) // Chỉ người chơi local mới có quyền 
        {
            // mỗi client local sẽ set dữ liệu của chính họ
            Rpc_UpdatePlayerData(
                D_GameManager.Instance.playerData.character, 
                D_GameManager.Instance.playerData.playerName);
        }
        
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_UpdatePlayerData(string characterKey, string playerName)
    {
        // hàm này sẽ được gọi bởi client local để cập nhật dữ liệu của chính họ lên networked properties, 
        // và sẽ được gọi trên tất cả client, bao gồm cả host, nên sẽ đảm bảo được việc đồng bộ hóa dữ liệu player cho tất cả người chơi
        playerNetworkData.SetPlayerData(characterKey, playerName);
    }
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        Debug.Log("DemoPlayer_UI Despawned");
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
    }
    public override void Render()
    {

        // vì hàm Render sẽ được gọi trên tất cả client, 
        // bao gồm cả host,
        //  nên sẽ đảm bảo được việc đồng bộ hóa hiệu ứng quay của UI cho tất cả người chơi,
        base.Render();

        SpinToTarget();

        UpdatePlayerName();
    }
    public void UpdatePlayerName() // chỉ client local gọi để Tất cả làm việc với hàm này
    {
        string N_name = playerNetworkData.playerName.ToString();
        if(string.IsNullOrEmpty(N_name)) return;
        if(N_name == lastPlayerName) return; // nếu tên chưa thay đổi thì không cần cập nhật lại
        lastPlayerName = playerNetworkData.playerName.ToString();
        playerName_txt.text = lastPlayerName;
    }
    public void SetUpSpawnPosition(PlayerRef playerRef)
    {
        // gán parent theo PlayerId (consistent trên cả host và client)
        if (playerRef.PlayerId == 1)
            this.transform.SetParent(GameController.Instance.spawnUIPos1.transform , false);
        else
            this.transform.SetParent(GameController.Instance.spawnUIPos2.transform , false);
    }
    public void SetUp(Sprite spr, string name)
    {
        demo_img.sprite = spr;
        playerName_txt.text = name;
    }
    void SpinToTarget()
    {
        float currentZ = demo_img.rectTransform.rotation.eulerAngles.z;
        float newZ = Mathf.MoveTowardsAngle(currentZ, spinValueZ, 25f * Time.deltaTime);
        demo_img.rectTransform.rotation = Quaternion.Euler(0, 0, newZ);

        // host sẽ là người duy nhất có quyền thay đổi giá trị spinValueZ,
        if (Mathf.Approximately(newZ, spinValueZ) && N_object.HasStateAuthority)
        {
            spinValueZ = RandomSpinEulerZ();
        }
    }

    float RandomSpinEulerZ()
    {
        float[] eulerZ = {-360, -180 , -90 , 90 , 180 , 360};
        int index = Random.Range(0, eulerZ.Length);
        float value = eulerZ[index];
        return value;
    }
}
