using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Music")]
    [Tooltip("List of audio sources for music. For smooth transition between two music.")]
    [SerializeField] private List<AudioSource> musicSources;
    [Header("Sound Effects")]
    [SerializeField] private AudioSource soundSource;

    private int currentMusicIndex = 0;
    private int nextMusicIndex = 1;


    public void PlayMusic()
    {
        musicSources[currentMusicIndex].Play();
    }
    public void StopMusic()
    {
        musicSources[currentMusicIndex].Stop();
    }
    public void PlayChangeToNewMusic(AudioClip newMusic)
    {

    }

}
