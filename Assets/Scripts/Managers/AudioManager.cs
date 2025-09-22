using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public List<AudioClip> audioClips = new List<AudioClip>();
    public AudioSource sfxSource;

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
}
