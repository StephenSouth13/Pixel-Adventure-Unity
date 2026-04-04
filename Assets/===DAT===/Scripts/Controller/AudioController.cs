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
    public bool isAudioControllerOnlySoundEffect = false; 
    // bật isAudioControllerOnlySoundEffect lên khi chỉ muốn dùng hàm PlaySFX mà không cần quản lý Toggle và Slider
    private bool isUpdating = false; // Biến để tránh vòng lặp khi cập nhật UI


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
        audioSource = D_AudioManager.Instance.GetAudio(isMusic);
        if(isAudioControllerOnlySoundEffect) return; // Nếu chỉ dùng để chơi SFX thì không cần thiết lập Toggle và Slider
        if(toggle == null || background == null || checkmark == null || slider == null)
        {
            Debug.LogError("Please assign all references in the inspector.");
            return;
        }
        // === lấy Instance ===
        // === Cài đặt trạng thái ban đầu của Toggle và Slider ===
        // Toggle
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
        // Slider
        slider.value = audioSource.volume * 17f;
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        
    }

    private void OnToggleValueChanged(bool value)
    {
        if(isUpdating) return;
        isUpdating = true;

        background.enabled = !value;
        var volume = isMusic ? D_AudioManager.Instance.currentMusicVolume 
                    : D_AudioManager.Instance.currentSFXVolume;
        if(value)
        {
            audioSource.volume = 0f;
            slider.value = 0;
        }
        else
        {
            if(slider.value == 0 && volume == 0)
            {
                slider.value = 1f; // Đặt về giá trị tối thiểu nếu trước đó là 0
                volume = 1f; // Đặt về giá trị tối thiểu nếu trước đó là 0
                audioSource.volume = Mathf.Clamp(volume / 17f, 0f, 1f) ;
                if(isMusic)
                {
                    D_AudioManager.Instance.SetCurrentAudioVolume(isMusic, slider.value);
                }
                else
                {
                    D_AudioManager.Instance.SetCurrentAudioVolume(isMusic, slider.value);
                }
                isUpdating = false;
                return;
            }
 
            
            slider.value = volume ;
            audioSource.volume = Mathf.Clamp(volume / 17f, 0f, 1f) ;
        }
        isUpdating = false;
    }

    private void OnSliderValueChanged(float value)
    {
        if(isUpdating) return;
        isUpdating = true;
        if(isMusic)
            {
                D_AudioManager.Instance.SetCurrentAudioVolume(isMusic, slider.value);
            }
            else
            {
                D_AudioManager.Instance.SetCurrentAudioVolume(isMusic, slider.value);
            }
        float volume = Mathf.Clamp(value / 17f, 0f, 1f); // Chia cho 17 để chuyển về giá trị từ 0 đến 1
        audioSource.volume = volume;
        // Khi slider = 0 → Toggle ON (mute), ngược lại OFF
        toggle.isOn = (value == 0);
        background.enabled = !(value == 0);
        isUpdating = false;
    }
    public void PlaySFX(int index)
    {
        D_AudioManager.Instance.PlaySFX(index);
    }
}
