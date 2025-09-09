using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    

    public void SetMusicVolume()
{
    float volume = musicSlider.value;
    audioMixer.SetFloat("Music", volume);
}

}