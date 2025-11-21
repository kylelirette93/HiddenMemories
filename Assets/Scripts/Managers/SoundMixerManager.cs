using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider soundFXSlider;
    [SerializeField] private Slider musicSlider;

    private void Start()
    {
        LoadVolumeLevels();
    }

    private void LoadVolumeLevels()
    {
        if (!PlayerPrefs.HasKey("masterVolume") ||
            !PlayerPrefs.HasKey("soundFXVolume") ||
            !PlayerPrefs.HasKey("musicVolume"))
        {
            ResetAudioLevelsToDefault();
            return;
        }
        float masterVolume = PlayerPrefs.GetFloat("masterVolume");
        float soundFXVolume = PlayerPrefs.GetFloat("soundFXVolume");
        float musicVolume = PlayerPrefs.GetFloat("musicVolume");
        audioMixer.SetFloat("masterVolume", masterVolume);
        audioMixer.SetFloat("soundFXVolume", soundFXVolume);
        audioMixer.SetFloat("musicVolume", musicVolume);
        masterSlider.value = Mathf.Pow(10f, masterVolume / 20f);
        soundFXSlider.value = Mathf.Pow(10f, soundFXVolume / 20f);
        musicSlider.value = Mathf.Pow(10f, musicVolume / 20f);
    }

    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20f);
        SaveVolumeLevel();
    }
    public void SetSoundFXVolume(float level)
    {
        audioMixer.SetFloat("soundFXVolume", Mathf.Log10(level) * 20f);
        SaveVolumeLevel();
    }
    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("musicVolume", Mathf.Log10(level) * 20f);
        SaveVolumeLevel();
    }

    private void SaveVolumeLevel()
    {
        PlayerPrefs.SetFloat("masterVolume", audioMixer.GetFloat("masterVolume", out float masterVol) ? masterVol : 0.5f);
        PlayerPrefs.SetFloat("soundFXVolume", audioMixer.GetFloat("soundFXVolume", out float sfxVol) ? sfxVol : 0.5f);
        PlayerPrefs.SetFloat("musicVolume", audioMixer.GetFloat("musicVolume", out float musicVol) ? musicVol : 0.5f);
    }

    private float ConvertToDecibels(float level)
    {
        return Mathf.Log10(level) * 20f;
    }

    public void ResetAudioLevelsToDefault()
    {
        float defaultMusicDB = ConvertToDecibels(0.3f);
        float defaultSFXDB = ConvertToDecibels(0.5f);
        float defaultDB = ConvertToDecibels(0.5f);
        audioMixer.SetFloat("masterVolume", defaultDB);
        audioMixer.SetFloat("soundFXVolume", defaultSFXDB);
        audioMixer.SetFloat("musicVolume", defaultMusicDB);
        masterSlider.value = 0.5f;
        soundFXSlider.value = 0.7f;
        musicSlider.value = 0.3f;
        SaveVolumeLevel();
    }
}
