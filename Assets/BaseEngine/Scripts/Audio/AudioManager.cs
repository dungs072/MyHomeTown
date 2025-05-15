using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Music")]
    [Tooltip("List of audio sources for music. For smooth transition between two music.")]
    [SerializeField] private List<AudioSource> musicSources;
    [Header("Sound Effects")]
    [SerializeField] private AudioSource soundSource;

    [SerializeField] private float transitionDuration = 1f;

    private int currentMusicIndex = 0;
    private int nextMusicIndex = 1;
    private float musicVolume = 0f;
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        for (int i = 0; i < musicSources.Count; i++)
        {
            musicSources[i].volume = volume;
        }
    }
    public void SetSoundVolume(float volume)
    {
        soundSource.volume = volume;
    }
    public void PlayCurrentMusic()
    {
        musicSources[currentMusicIndex].Play();
    }
    public void StopCurrentMusic()
    {
        musicSources[currentMusicIndex].Stop();
    }
    public void PlayNextMusic()
    {
        musicSources[nextMusicIndex].Play();
    }
    public void StopNextMusic()
    {
        musicSources[nextMusicIndex].Stop();
    }
    public void PlaySoundEffect(string audioKey)
    {
        var audioClip = AudioLoader.Instance.GetAudioClip(audioKey);
        if (!audioClip) return;
        soundSource.PlayOneShot(audioClip);
    }
    public void ToggleMuteMusic(bool isMuted)
    {
        for (int i = 0; i < musicSources.Count; i++)
        {
            musicSources[i].mute = isMuted;
        }
    }
    public void ToggleMuteSound(bool isMuted)
    {
        soundSource.mute = isMuted;
    }

    public IEnumerator TransitionToNewMusic(string audioKey)
    {
        var audioClip = AudioLoader.Instance.GetAudioClip(audioKey);
        if (!audioClip) yield break;
        PlayCurrentMusic();
        PlayNextMusic();
        musicSources[nextMusicIndex].clip = audioClip;
        // reduce the volume of the current music gradually
        float currentValue = musicVolume;
        var reduceVolumeTween = DOTween.To(() => currentValue, x =>
        {
            SetCurrentMusicVolume(x);
        }, 0f, transitionDuration)
        .SetEase(Ease.Linear);

        // increase the volume of the next music gradually
        float nextValue = 0;
        var increaseVolumeTween = DOTween.To(() => nextValue, x =>
        {
            SetNextMusicVolume(x);
        }, 1f, transitionDuration);
        yield return new WaitUntil(() => reduceVolumeTween.IsComplete() && increaseVolumeTween.IsComplete());
        // swap index
        currentMusicIndex = nextMusicIndex;
        nextMusicIndex = (nextMusicIndex + 1) % musicSources.Count;
        PlayCurrentMusic();
        StopNextMusic();
    }
    private void SetCurrentMusicVolume(float volume)
    {
        musicSources[currentMusicIndex].volume = volume;
    }
    private void SetNextMusicVolume(float volume)
    {
        musicSources[nextMusicIndex].volume = volume;
    }

}
