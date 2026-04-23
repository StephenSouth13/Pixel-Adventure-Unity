using UnityEngine;
using Fusion;

public class PlayerNetworkData : NetworkBehaviour
{
    //==== các biến cần SYNC lên network ở đây ====
    [Networked, Capacity(20)] public NetworkString<_16> characterKey { get; set; }
    [Networked] public int Hp { get; set; }
    [Networked, Capacity(16)] public NetworkString<_16> playerName { get; set; }
    //==== end ====
    public static PlayerNetworkData localInstance;
    PlayerData localPlayerData;
    NetworkObject N_object;
    //==== các hàm xử lý logic liên quan đến network data ở đây ====
    void Awake()
    {
        N_object = GetComponent<NetworkObject>();
        
    }
    public override void Spawned()
    {
        base.Spawned();
        Debug.Log("PlayerNetworkData Spawned");
        
        if (N_object.HasInputAuthority) 
        {
            // chỉ có local player mới được phép gán dữ liệu vào networked properties,
            // và cũng chỉ local player mới có thể truy cập vào instance này 
            // thông qua PlayerNetworkData.localInstance
            localInstance = this;
            localPlayerData = D_GameManager.Instance.GetLocalPlayerData(); // lấy dữ liệu player đã lưu từ GameManager
        }
    }
    public void SetPlayerData(string characterKey, string playerName)
    {
        this.characterKey = characterKey;
        this.playerName = playerName;
    }
    public void SetPlayerData(string characterKey, int hp, string playerName)
    {
        this.characterKey = characterKey;
        this.Hp = hp;
        this.playerName = playerName;
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestTakeHit(int damage)
    {
        ApplyDamage(damage);
    }

    public void ApplyDamage(int damage)
    {
        if (!Object.HasStateAuthority) return;

        Hp -= damage;

        if (Hp <= 0)
        {
            Hp = 0;
            // chết
        }
    }


    //==== end ====
}
