using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public List<AudioClip> audioClips = new List<AudioClip>();
    public Dictionary<string, AudioClip> audioClip = new Dictionary<string, AudioClip>();
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
    public float sfxVolume = 1f;
    private const string SFXKey = "SFXVol";

    [Header("Master Volume")]
    public AudioSource musicSource;
    public float audioVolume = 1f;
    private const string MasterKey = "MasterVol";

    private void Awake()
    {
        PopulateAudioLibrary();
    }

    private void OnEnable()
    {
        audioVolume = PlayerPrefs.GetFloat(MasterKey, 0.5f);
        sfxVolume = PlayerPrefs.GetFloat(SFXKey, 0.5f);
        ChangeMasterVolume(audioVolume);
        ChangeSFXVolume(sfxVolume);
    }
    public void PopulateAudioLibrary()
    {
        for (int i = 0; i < audioClips.Count; i++)
        {
            audioClip.Add(audioClips[i].name, audioClips[i]);
        }
    }
    public void PlaySound(string name)
    {
        if (audioClip.ContainsKey(name))
        {
            sfxSource.PlayOneShot(audioClip[name]);
        }
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
