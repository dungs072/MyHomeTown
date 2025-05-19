using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AudioLoader : MonoBehaviour
{
    private Dictionary<string, AudioClip> audioClips = new();
    public static AudioLoader Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        StartCoroutine(LoadAudio());
    }

    public AudioClip GetAudioClip(string audioKey)
    {
        if (audioClips.TryGetValue(audioKey, out var clip))
        {
            return clip;
        }
        Debug.LogError($"Audio clip with key {audioKey} not found.");
        return null;
    }

    IEnumerator LoadAudio()
    {
        var audioKeys = AudioKey.GetAllAudioKeys();

        List<AsyncOperationHandle<AudioClip>> handles = new();
        Dictionary<AsyncOperationHandle<AudioClip>, string> keyMap = new();

        // Start loading all clips in parallel
        foreach (var audioKey in audioKeys)
        {
            string audioAddress = AudioUtils.GetAudioPath(audioKey);
            var handle = Addressables.LoadAssetAsync<AudioClip>(audioAddress);
            handles.Add(handle);
            keyMap[handle] = audioAddress;
        }

        // Wait for all to finish
        yield return new WaitUntil(() => handles.All(h => h.IsDone));

        // Process results
        foreach (var handle in handles)
        {
            string key = keyMap[handle];
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                audioClips[key] = handle.Result;
            }
            else
            {
                Debug.LogError($"Failed to load AudioClip: {key}");
            }
        }

        Debug.Log("✅ All audio clips loaded in parallel.");
    }
}
