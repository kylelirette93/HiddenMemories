using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public List<AudioClip> musicClips = new List<AudioClip>();
    public List<AudioClip> audioClips = new List<AudioClip>();
    public Dictionary<string, AudioClip> audioClip = new Dictionary<string, AudioClip>();
    public Dictionary<string, AudioClip> musicClip = new Dictionary<string, AudioClip>();

    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioSource musicSource;

    private void Awake()
    {
        PopulateAudioLibrary();
    }

    public void PopulateAudioLibrary()
    {
        for (int i = 0; i < audioClips.Count; i++)
        {
            audioClip.Add(audioClips[i].name, audioClips[i]);
        }
        for (int i = 0; i < musicClips.Count; i++)
        {
            musicClip.Add(musicClips[i].name, musicClips[i]);
        }
    }
    public void PlaySound(string name)
    {
        if (audioClip.ContainsKey(name))
        {
            sfxSource.PlayOneShot(audioClip[name]);
        }
    }

    public void PlayMusic(string name)
    {
        if (musicClip.ContainsKey(name))
        {
            musicSource.clip = musicClip[name];
            musicSource.Play();
        }
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }
}
