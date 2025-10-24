using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    AudioManager audioManager;
    public Slider volumeSlider;
    public Slider sfxSlider;
    public TextMeshProUGUI volumeText;

    private void OnEnable()
    {
        audioManager = GameManager.Instance.audioManager;
        if  (audioManager != null)
        {
            volumeSlider.onValueChanged.AddListener(audioManager.SetMasterVolume);
            sfxSlider.onValueChanged.AddListener(audioManager.SetSFXVolume);
        }
    }
}
