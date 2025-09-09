using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
 [SerializeField] private GameObject musicPanel;

public void ShowMusicPanel()
{
    musicPanel.SetActive(true); // bật panel lên
}

    private void Start()
    {
        SetMusicVolume();
    }
    

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        audioMixer.SetFloat("Music", volume);
    }

}