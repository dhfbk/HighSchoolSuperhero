using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource MusicAudioSource;
    public AudioSource FXAudioSource;
    public AudioClip menuMusic;
    public AudioClip townMusic;

    public static AudioClip MenuMusic;
    public static AudioClip TownMusic;
    // Start is called before the first frame update
    void Start()
    {
        MenuMusic = menuMusic;
        TownMusic = townMusic;
    }

    public void ChangeBGM(AudioClip music)
    {
        if (MusicAudioSource.clip != null)
        {
            if (MusicAudioSource.clip.name != music.name)
            {
                MusicAudioSource.Stop();
                MusicAudioSource.clip = music;
                MusicAudioSource.Play();
            }
        }
        else
        {
            MusicAudioSource.clip = music;
            MusicAudioSource.Play();
        }
    }
}
