using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace BaseEngine
{
    public class TScreenLoader
    {
        public GameObject screenPrefab;
        public BaseScreen screen;
        public AsyncOperationHandle<GameObject> handle;
    }
    public class ScreenManager : MonoBehaviour
    {
        private Stack<BaseScreen> screenStack = new();
        private Dictionary<string, TScreenLoader> screens = new();
        public static ScreenManager ScreenManagerInstance { get; private set; }

        private Transform screenHolder;

        public T GetScreen<T>(string screenName) where T : BaseScreen
        {
            if (screens.ContainsKey(screenName))
            {
                var screen = GetScreen(screenName);
                if (screen.TryGetComponent(out T specificScreen))
                {
                    return specificScreen;
                }
                Debug.LogError($"Screen {screenName} does not have component of type {typeof(T)}.");
            }
            else
            {
                Debug.LogError($"Screen {screenName} is not preloaded.");
            }
            return null;
        }

        private BaseScreen GetScreen(string screenName)
        {
            if (screens.ContainsKey(screenName))
            {
                return screens[screenName].screen;
            }
            Debug.LogError($"Screen {screenName} is not preloaded.");
            return null;
        }
        void Awake()
        {
            if (ScreenManagerInstance == null)
            {
                ScreenManagerInstance = this;
            }

        }
        public void SetScreenHolder(Transform holder)
        {
            screenHolder = holder;
        }
        public void PreloadScreensAsync(List<string> screenNames)
        {
            foreach (var screenName in screenNames)
            {
                if (screens.ContainsKey(screenName))
                {
                    HandleScreenKeyExists(screenName);
                    continue;
                }
                var templateHandle = Addressables.LoadAssetAsync<GameObject>(screenName);
                templateHandle.Completed += (AsyncOperationHandle<GameObject> obj) =>
                {
                    OnScreenLoaded(obj, screenName);
                };
                var screenLoader = new TScreenLoader()
                {
                    handle = templateHandle,
                };
                screens.Add(screenName, screenLoader);
            }
        }
        private void HandleScreenKeyExists(string screenName)
        {
            var loadedScreen = screens[screenName];
            if (loadedScreen.screen != null)
            {
                Debug.LogWarning($"Screen {screenName} is already loaded.");
            }
            else
            {
                var screenInstance = Instantiate(loadedScreen.screenPrefab, screenHolder);
                if (screenInstance.TryGetComponent(out BaseScreen baseScreen))
                {
                    loadedScreen.screen = baseScreen;
                    baseScreen.gameObject.SetActive(false);
                    Debug.Log($"Loaded screen (existing prefab): {screenName}");
                }
            }
        }
        private void OnScreenLoaded(AsyncOperationHandle<GameObject> handle, string screenName)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject screenPrefab = handle.Result;
                var screenLoader = screens[screenName];
                if (screenLoader == null)
                {
                    Debug.LogError($"ScreenLoader not found for screen: {screenName}");
                    return;
                }
                screenLoader.screenPrefab = screenPrefab;
                GameObject screenInstance = Instantiate(screenPrefab, screenHolder);
                if (screenInstance.TryGetComponent(out BaseScreen baseScreen))
                {
                    screenLoader.screen = baseScreen;
                    baseScreen.gameObject.SetActive(false);
                    Debug.Log($"Loaded screen: {screenName}");
                }
                else
                {
                    Debug.LogError($"BaseScreen component not found on prefab: {screenName}");
                    Destroy(screenInstance);
                }
            }
            else
            {
                Debug.LogError("Failed to load screen: " + screenName + ". " + handle.OperationException);
            }
        }

        public IEnumerator LoadScreenAsync(string screenName)
        {
            if (screens.ContainsKey(screenName))
            {
                var loadedScreenData = screens[screenName];
                yield return loadedScreenData.handle;
                yield break;
            }
            var handle = Addressables.LoadAssetAsync<GameObject>(screenName);
            var screenLoader = new TScreenLoader()
            {
                handle = handle,
            };
            screens.Add(screenName, screenLoader);
            yield return handle;
            OnScreenLoaded(handle, screenName);
        }
        public static IEnumerator OpenScreenAsync(string screenName)
        {
            if (ScreenManagerInstance == null)
            {
                Debug.LogError("ScreenManager instance is not initialized.");
                yield break;
            }
            var screenLoader = ScreenManagerInstance.screens[screenName];

            if (screenLoader == null || screenLoader.screen == null)
            {
                yield return ScreenManagerInstance.LoadScreenAsync(screenName);
            }
            var screen = screenLoader.screen;
            ScreenManagerInstance.screenStack.Push(screen);
            yield return screen.OpenScreenAsync();
        }
        public static IEnumerator CloseScreenAsync(string screenName)
        {
            if (ScreenManagerInstance == null)
            {
                Debug.LogError("ScreenManager instance is not initialized.");
                yield break;
            }
            if (!ScreenManagerInstance.screens.ContainsKey(screenName))
            {
                Debug.LogError($"Screen {screenName} is not preloaded.");
                yield break;
            }
            var screenLoader = ScreenManagerInstance.screens[screenName];
            var screen = screenLoader.screen;
            ScreenManagerInstance.screenStack.Pop();
            yield return screen.CloseScreenAsync();
        }

        public BaseScreen GetCurrentScreen()
        {
            if (screenStack.Count > 0)
            {
                return screenStack.Peek();
            }
            return null;
        }


    }
}
