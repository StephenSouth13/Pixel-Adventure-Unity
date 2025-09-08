using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class HomePageButton : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;      // Assign in Inspector
    [SerializeField] private float fadeDuration = 2f;       // Duration of fade
    [SerializeField] private GameObject musicOptionsPanel;  // Assign your music options UI panel here

    // Called when Tutorial button is clicked
    public void OnTutorialButtonClicked()
    {
        StartCoroutine(FadeOutAndLoad("Tutorial"));
    }

    // Called when Music Options button is clicked
    public void OnMusicOptionsButtonClicked()
    {
        if (musicOptionsPanel != null)
        {
            musicOptionsPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Music Options Panel not assigned in Inspector.");
        }
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1 - (timer / fadeDuration);
            }
            yield return null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
        }

        SceneManager.LoadSceneAsync(sceneName);
    }
}