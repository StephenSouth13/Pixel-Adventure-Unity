using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour
{
    // Danh sách tên scene theo thứ tự bạn muốn
    private string[] levelNames = { "Home", "Tutorial", "Level1", "Level2" };

    public void OpenLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelNames.Length)
        {
            string levelName = levelNames[levelIndex];
            SceneManager.LoadScene(levelName);
            Debug.Log($"Opening scene: {levelName}");
        }
        else
        {
            Debug.LogWarning($"Invalid level index: {levelIndex}");
        }
    }
}