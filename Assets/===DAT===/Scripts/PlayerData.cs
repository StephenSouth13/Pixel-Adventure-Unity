using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class PlayerData
{
    public string playerId;
    public string playerName;
    public string character; // tên key của objectpool để biết được player đang chọn nhân vật nào, sau này sẽ dùng để load đúng prefab nhân vật đó vào game
    public int level;
    public int experience;
    public int gold;
    public List<string> unlockedPlanets; // Danh các màn chơi đã mở khóa

    public void SaveData()
    {
        // Lưu dữ liệu vào PlayerPrefs hoặc hệ thống lưu trữ khác
        PlayerPrefs.SetString("PlayerId", playerId);
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.SetString("Character", character);
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetInt("Experience", experience);
        PlayerPrefs.SetInt("Gold", gold);
        PlayerPrefs.Save();
    }
    public void LoadData()
    {
        // Tải dữ liệu từ PlayerPrefs hoặc hệ thống lưu trữ khác
        playerId = PlayerPrefs.GetString("PlayerId", null);
        playerName = PlayerPrefs.GetString("PlayerName", null);
        character = PlayerPrefs.GetString("Character", "Virtual_Guy");
        level = PlayerPrefs.GetInt("Level", 1);
        experience = PlayerPrefs.GetInt("Experience", 0);
        gold = PlayerPrefs.GetInt("Gold", 0);
    }
    public void UnlockPlanet(string planetName)
    {
        // tạm thời chưa dùng đến, sau này sẽ dùng để lưu lại
    }
    public void SetPlayerName(string name)
    {
        playerName = name;
        SaveData(); // Lưu lại dữ liệu sau khi cập nhật tên
    }
    public void SetPlayerId(string id)
    {
        playerId = id;
        SaveData(); // Lưu lại dữ liệu sau khi cập nhật ID
    }
    public void SetCharacter(string characterKey)
    {
        character = characterKey;
        SaveData(); // Lưu lại dữ liệu sau khi cập nhật nhân vật
    }
    public void AddExperience(int exp)
    {
        experience += exp;
        // Kiểm tra nếu đủ EXP để lên cấp, nếu có thể thì tăng level và reset EXP
        int expToLevelUp = (level + 1) * 10; // Ví dụ: mỗi cấp cần 10 EXP // + 1 cho level 0
        
        if(experience >= expToLevelUp)
        {
            if(level <= 10){ // Giới hạn level tối đa là 10
                level++;
                experience -= expToLevelUp; // Reset EXP sau khi lên cấp
                SaveData(); // Lưu lại dữ liệu sau khi cập nhật level và EXP
            }
            else
            {
                experience = expToLevelUp; // Giữ EXP ở mức tối đa nếu đã đạt level 10
                SaveData(); // Lưu lại dữ liệu sau khi cập nhật EXP
            }
        }
    }
    public void AddGold(int amount)
    {
        gold += amount;
        SaveData(); // Lưu lại dữ liệu sau khi cập nhật vàng
    }
    public void SpendGold(int amount)
    {
        if(gold >= amount)
        {
            gold -= amount;
            SaveData(); // Lưu lại dữ liệu sau khi cập nhật vàng
        }
        else
        {
            Debug.LogWarning("Not enough gold!");
        }
    }
    public void ResetData()
    {
        playerId = "DefaultId";
        playerName = "Player";
        character = "Virtual_Guy";
        level = 1;
        experience = 0;
        gold = 0;
        unlockedPlanets.Clear();
        SaveData(); // Lưu lại dữ liệu sau khi reset
    }
}