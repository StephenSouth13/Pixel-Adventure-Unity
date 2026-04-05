using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class D_GameManager : MonoBehaviour
{
    public static D_GameManager Instance;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // === Quản Lý Scene ===
    #region Scene Management
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
    
    #endregion
}
