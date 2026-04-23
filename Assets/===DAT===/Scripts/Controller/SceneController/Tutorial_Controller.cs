using UnityEngine;
using Fusion;
using System.Collections.Generic;
using System.Linq;

public class Tutorial_Controller : NetworkBehaviour
{
    // === REFERENCES ===
    public Transform[] spawnPos;

    // === 
    NetworkRunner runner;
    D_NetworkManager d_NetworkManager;
    // === End ===
    public static Tutorial_Controller Instance;
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
        runner = D_NetworkManager.Instance.runner;
        d_NetworkManager = D_NetworkManager.Instance;
    }

    public override void Spawned()
    {
        base.Spawned();
        Debug.Log("Tutorial_Controller Spawned");

        // Lấy danh sách player hiện tại
        var players = runner.ActivePlayers.ToList();

        for (int i = 0; i < players.Count; i++)
        {
            var player = players[i];

            // Lấy token của player
            var token = D_NetworkManager.Instance.runner.GetPlayerConnectionToken(player);
            string data = System.Text.Encoding.UTF8.GetString(token);

            string[] split = data.Split('|');
            string characterKey = split[0];
            string playerName = split[1];

            // Spawn prefab cho player này
            var obj = D_NetworkManager.Instance.RunnerSpawn(characterKey + "_Onl", spawnPos[i].position , Quaternion.identity ,player);
            // ❗ Chỉ host (StateAuthority) mới spawn player
            if (Object.HasStateAuthority)
            {
                var NetData = obj.GetComponent<PlayerNetworkData>();
                NetData.SetPlayerData(characterKey + "_Onl",3 , playerName);
            }

        }
    }

}
