using UnityEngine;

public class GameController : MonoBehaviour
{
    D_GameManager gameManager;
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
    private void Start()
    {
        if(D_GameManager.Instance == null)
        {
            Debug.LogError("GameManager is not found in the scene.");
            return;
        }
        if(gameManager == null)
        {
            gameManager = D_GameManager.Instance;
        }
    }
    public void LoadScene(int sceneIndex)
    {
        gameManager.LoadScene(sceneIndex);
    }
    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif  
    }
}
