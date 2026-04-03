using UnityEngine;
using UnityEngine.Audio;
public class D_AudioManager : MonoBehaviour
{
    public static D_AudioManager Instance;
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public float currentMusicVolume = 1f;
    public float currentSFXVolume = 1f;
    public AudioClip[] musicClips;
    public AudioClip[] sfxClips;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public AudioSource GetAudio(bool isMusic)
    {
        Debug.Log("Getting audio source for " + (isMusic ? "music" : "SFX"));
        return isMusic ? musicSource : sfxSource;
    }
    public void PlayMusic(int index)
    {
        if (index >= 0 && index < musicClips.Length)
        {
            musicSource.clip = musicClips[index];
            musicSource.Play();
        }
    }
    public void PlaySFX(int index)
    {
        if (index >= 0 && index < sfxClips.Length)
        {
            sfxSource.PlayOneShot(sfxClips[index]);
        }
    }
    public void SetCurrentAudioVolume(bool isMusic, float volume)
    {
        if(isMusic)
        {
            currentMusicVolume = volume;
        }
        else
        {
            currentSFXVolume = volume;
        }
    }

}
