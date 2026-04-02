using UnityEngine;
using UnityEngine.UI;
public class AudioController : MonoBehaviour
{
    // Gắn trực tiếp Toggle, Background và Checkmark vào Inspector
    // Mục đích thay đổi hình ảnh của Toggle khi nó được bật hoặc tắt
    [Header("Audio Toggle Components")]
    public Toggle toggle;
    public Image background;
    public Image checkmark;
    public Slider slider;
    [Header("Audio Manager Reference")]
    AudioSource audioSource;
    public bool isMusic; // Xác định đây là Toggle cho âm nhạc hay âm thanh

    void Start()
    {
        Init();
    }
    private void Init()
    {
        if(D_AudioManager.Instance == null)
        {
            Debug.LogError("D_AudioManager instance not found in the scene.");
            return;
        }
        if(toggle == null || background == null || checkmark == null || slider == null)
        {
            Debug.LogError("Please assign all references in the inspector.");
            return;
        }
        // === lấy Instance ===
        audioSource = D_AudioManager.Instance.GetAudio(isMusic);
        // === Cài đặt trạng thái ban đầu của Toggle và Slider ===
        // Toggle
        toggle.isOn = audioSource.isPlaying; // isplaying sẽ trả về true nếu âm thanh đang phát, ngược lại trả về false
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
        // Slider
        slider.value = audioSource.volume;
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        
    }

    private void OnToggleValueChanged(bool value)
    {
        background.enabled = !value;
        checkmark.enabled = value;
        if(value)
        {
            if(isMusic)
            {
                D_AudioManager.Instance.currentMusicVolume = slider.value; // Lưu âm lượng hiện tại của nhạc
            }
            else
            {
                D_AudioManager.Instance.currentSFXVolume = slider.value; // Lưu âm lượng hiện tại của hiệu ứng âm thanh
            }
            slider.value = 0; // Khi Toggle bật
        }
        else
        {
            if(isMusic)
            {
                slider.value = D_AudioManager.Instance.currentMusicVolume; // Khôi phục âm lượng hiện tại của nhạc
            }
            else
            {
                slider.value = D_AudioManager.Instance.currentSFXVolume; // Khôi phục âm lượng hiện tại của hiệu ứng âm thanh
            }
        }
    }
    private void OnSliderValueChanged(float value)
    {
        audioSource.volume = value; // Điều chỉnh âm lượng của AudioSource
        if(value > 0)
        {
            toggle.isOn = true; // Nếu âm lượng lớn hơn 0, bật Toggle

        }else
        {
            toggle.isOn = false; // Nếu âm lượng bằng 0, tắt Toggle
        }
    }
}
