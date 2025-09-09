using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // giữ lại khi chuyển scene
    }
}