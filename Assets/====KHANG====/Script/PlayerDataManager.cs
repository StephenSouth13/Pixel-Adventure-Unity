using Fusion;
using UnityEngine;

// struct quản lý thông tin
public struct PlayerMetaData : INetworkStruct
{
    public NetworkString<_16> Name;
    public CharacterClass Class;

    public int Hp;
    public int MaxHp;

    public int Mp;
    public int MaxMp;

    public int Level;
    public int Exp;
    public int Atk;
    public int Def;
}

public class PlayerDataManager : NetworkBehaviour
{
    // biến này của Fusion sẽ tự động đồng bộ giữa các client và host,
    // khi có thay đổi sẽ tự động cập nhật ở tất cả các bên
    [Networked, Capacity(10)] public NetworkDictionary<PlayerRef, PlayerMetaData> Players => default;

    // RPC: phương thức này sẽ được gọi từ client hoặc
    // host để cập nhật thông tin player, sau đó sẽ được gửi
    // đến state authority (host) để xử lý và đồng bộ lại cho tất cả các client
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_UpdatePlayerMetaData(PlayerRef playerRef, PlayerMetaData metaData)
    {
        Players.Set(playerRef, metaData);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_UpdateHp(PlayerRef playerRef, int amount)
    {
        // amount: số dương để tăng HP, số âm để giảm HP
        if (Players.TryGet(playerRef, out var metaData))
        {
            metaData.Hp += amount;
            metaData.Hp = Mathf.Clamp(metaData.Hp, 0, metaData.MaxHp); // đảm bảo HP không vượt quá max và không âm
            Players.Set(playerRef, metaData);
        }
    }

    public bool TryGetPlayerMetaData(PlayerRef playerRef, out PlayerMetaData metaData)
    {
        return Players.TryGet(playerRef, out metaData);
    }
}