using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    private Dictionary<string, BaseScreen> screens = new Dictionary<string, BaseScreen>();
    public static ScreenManager Instance { get; private set; }

    private Transform screenHolder;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void SetScreenHolder(Transform holder)
    {
        screenHolder = holder;
    }

    public IEnumerator PreloadScreens(List<string> screenNames)
    {
        Debug.Log($"Preloading screens: {string.Join(", ", screenNames)}");
        foreach (var screenName in screenNames)
        {
            Debug.Log($"Preloading screen: {screenName}");
            if (screens.ContainsKey(screenName))
            {
                var screen = screens[screenName];
                screen.transform.SetParent(screenHolder, true);
                Debug.LogWarning($"Screen {screenName} is already preloaded.");
                continue;
            }
            string path = $"Screens/{screenName}";
            GameObject screenPrefab = Resources.Load<GameObject>(path);
            if (screenPrefab == null)
            {
                Debug.LogError($"Screen prefab not found at path: {path}");
                continue;
            }

            GameObject screenInstance = Instantiate(screenPrefab, screenHolder);
            BaseScreen baseScreen = screenInstance.GetComponent<BaseScreen>();
            if (baseScreen == null)
            {
                Debug.LogError($"BaseScreen component not found on prefab: {path}");
                Destroy(screenInstance);
                continue;
            }

            screens.Add(screenName, baseScreen);
            baseScreen.gameObject.SetActive(false);
            yield return new WaitForEndOfFrame();
        }
    }


    private IEnumerator PreloadScreen(string screenName)
    {
        if (screens.ContainsKey(screenName))
        {
            Debug.LogWarning($"Screen {screenName} is already preloaded.");
            yield break;
        }
        string path = $"Screens/{screenName}";
        var request = Resources.LoadAsync<GameObject>(path);
        yield return request;
        if (!request.isDone)
        {
            Debug.LogError($"Failed to load screen prefab at path: {path}");
            yield break;
        }
        GameObject screenPrefab = request.asset as GameObject;
        if (screenPrefab == null)
        {
            Debug.LogError($"Screen prefab not found at path: {path}");
            yield break;
        }

        GameObject screenInstance = Instantiate(screenPrefab, screenHolder);
        BaseScreen baseScreen = screenInstance.GetComponent<BaseScreen>();
        if (baseScreen == null)
        {
            Debug.LogError($"BaseScreen component not found on prefab: {path}");
            Destroy(screenInstance);
            yield break;
        }
        screens.Add(screenName.ToString(), baseScreen);
        baseScreen.gameObject.SetActive(false);
    }


    public static IEnumerator OpenScreenAsync(string screenName)
    {
        if (Instance == null)
        {
            Debug.LogError("ScreenManager instance is not initialized.");
            yield break;
        }
        if (!Instance.screens.ContainsKey(screenName))
        {
            yield return Instance.PreloadScreen(screenName);
        }
        BaseScreen screen = Instance.screens[screenName];
        yield return screen.OpenScreenAsync();
    }
    public static IEnumerator CloseScreenAsync(string screenName)
    {
        if (Instance == null)
        {
            Debug.LogError("ScreenManager instance is not initialized.");
            yield break;
        }
        if (!Instance.screens.ContainsKey(screenName))
        {
            Debug.LogError($"Screen {screenName} is not preloaded.");
            yield break;
        }
        BaseScreen screen = Instance.screens[screenName];
        yield return screen.CloseScreenAsync();
    }


}