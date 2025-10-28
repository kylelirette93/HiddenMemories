using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public List<AudioClip> audioClips = new List<AudioClip>();
    [ContextMenu("Clear Saved Audio Settings")]
    public void ClearAudioSettings()
    {
        PlayerPrefs.DeleteKey(MasterKey);
        PlayerPrefs.DeleteKey(SFXKey);
        PlayerPrefs.Save();
        Debug.Log("Audio settings cleared!");
    }
    [Header("SFX Volume")]
    public AudioSource sfxSource;
    public float sfxVolume = 0.5f;
    private const string SFXKey = "SFXVol";

    [Header("Master Volume")]
    public AudioSource musicSource;
    public float audioVolume = 0.5f;
    private const string MasterKey = "MasterVol";

    private void OnEnable()
    {
        audioVolume = PlayerPrefs.GetFloat(MasterKey, 0.5f);
        sfxVolume = PlayerPrefs.GetFloat(SFXKey, 0.5f);
        ChangeMasterVolume(audioVolume);
        ChangeSFXVolume(sfxVolume);
    }

    public void PlaySFX(AudioClip clip)
    {    
        // Play clip from list.
        if (audioClips.Contains(clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("AudioManager: Clip not found in audioClips list.");
        }
    }

    public void AddSFX(AudioClip clip)
    {
        audioClips.Add(clip);
    }

    public void SetMasterVolume(float value)
    {
        audioVolume = Mathf.Clamp01(value);
        ChangeMasterVolume(audioVolume);
        PlayerPrefs.SetFloat(MasterKey, audioVolume);
        PlayerPrefs.Save();
    }

    public float GetMasterVolume()
    {
        return audioVolume;
    }

    public void ChangeMasterVolume(float value)
    {
        musicSource.volume = value;
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        ChangeSFXVolume(sfxVolume);
        PlayerPrefs.SetFloat(SFXKey, sfxVolume);
        PlayerPrefs.Save();
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    public void ChangeSFXVolume(float value)
    {
        sfxSource.volume = value;
    }
}
