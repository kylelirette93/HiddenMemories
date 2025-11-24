using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] List<AudioClip> musicClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> audioClips = new List<AudioClip>();
    Dictionary<string, AudioClip> audioClipDict = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> musicClipDict = new Dictionary<string, AudioClip>();

    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioSource musicSource;

    private void Awake()
    {
        // Clear before populating to avoid duplicates.
        audioClipDict.Clear();
        musicClipDict.Clear();

        PopulateAudioLibrary();
    }

    public void PopulateAudioLibrary()
    {
        for (int i = 0; i < audioClips.Count; i++)
        {
            if (!audioClipDict.ContainsKey(audioClips[i].name))
            {
                // Only add if the key doesn't already exist.
                audioClipDict.Add(audioClips[i].name, audioClips[i]);
            }
        }
        for (int i = 0; i < musicClips.Count; i++)
        {
            if (!musicClipDict.ContainsKey(musicClips[i].name))
            {
                // Only add if the key doesn't already exist.
                musicClipDict.Add(musicClips[i].name, musicClips[i]);
            }
        }
    }
    public void PlaySound(string name)
    {
        if (sfxSource == null)
        {
            Debug.LogWarning("SFX source not assigned in inspector.");
            return;
        }
        if (audioClipDict.ContainsKey(name))
        {
            sfxSource.PlayOneShot(audioClipDict[name]);
        }
    }

    public void PlayMusic(string name)
    {
        if (musicSource == null)
        {
            Debug.LogWarning("Music source not assigned in inspector.");
            return;
        }
        // Return if music is already playing.
        if (musicSource.clip == musicClipDict[name] && musicSource.isPlaying) return;

        if (musicClipDict.ContainsKey(name))
        {
            musicSource.clip = musicClipDict[name];
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
