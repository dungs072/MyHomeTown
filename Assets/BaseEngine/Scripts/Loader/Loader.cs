using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace BaseEngine
{
    // This class is responsible for loading assets and managing the loading process.
    // It should be attached to a GameObject in the scene.

    // resource
    public class TLoadedPrefab
    {
        public GameObject prefab;
        public ResourceRequest request;
        public Action OnLoaded;

    }
    public class TLoadedPrefabInFolder
    {
        public List<GameObject> prefabs;
        public Action OnLoaded;
    }

    // addressable
    public class TLoadedLabelAsset
    {
        public List<GameObject> prefabs;
        public Action OnLoaded;
    }
    public class Loader : MonoBehaviour
    {
        public static Loader Instance => instance;
        private static Loader instance;

        // for resource
        protected List<string> loadedPrefabAssets = new();
        protected List<string> loadedPrefabAssetsInFolder = new();
        protected Dictionary<string, TLoadedPrefab> loadedPrefabs = new();
        protected Dictionary<string, TLoadedPrefabInFolder> loadedPrefabsInFolder = new();

        // for addressable
        protected List<string> loadedAddressableLabels = new();
        protected Dictionary<string, TLoadedLabelAsset> loadedAddressableAssets = new();
        private System.Threading.Tasks.Task loadTask;
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            // else if (instance != this)
            // {
            //     Destroy(gameObject);
            // }
            InitLoader();
        }
        private void InitLoader()
        {
            InitLoadInResource();
            InitLoadInAddressable();
        }
        private void InitLoadInResource()
        {
            AddPrefabPath();
            AddPrefabPathInFolder();
            StartCoroutine(LoadAllPrefabsAsync());
            StartCoroutine(LoadAllPrefabsInFolderAsync());
        }
        private void InitLoadInAddressable()
        {
            AddAddressableLabels();
            StartCoroutine(LoadAllAddressableAssetsCoroutine());
        }

        #region Resource

        protected virtual void AddPrefabPath()
        {
            // Add the paths of the prefabs you want to load here
            // loadedPrefabAssets.Add("Props/Prop1");
            // loadedPrefabAssets.Add("Props/Prop2");
            // loadedPrefabAssets.Add("Props/Prop3");
        }
        protected virtual void AddPrefabPathInFolder()
        {
            // Add the paths of the prefabs you want to load here
            // loadedPrefabAssetsInFolder.Add("Props/Prop1");
            // loadedPrefabAssetsInFolder.Add("Props/Prop2");
            // loadedPrefabAssetsInFolder.Add("Props/Prop3");
        }


        protected IEnumerator LoadAllPrefabsAsync()
        {
            var requests = new List<ResourceRequest>();
            // Start all load requests in parallel
            foreach (var prefabPath in loadedPrefabAssets)
            {
                var request = Resources.LoadAsync<GameObject>(prefabPath);
                requests.Add(request);
                var loadedPrefab = new TLoadedPrefab
                {
                    prefab = null,
                    request = request,
                };
                loadedPrefabs.Add(prefabPath, loadedPrefab);
            }

            // Wait until all requests are done
            for (int i = 0; i < requests.Count; i++)
            {
                var request = requests[i];
                yield return request;
                var prefab = request.asset as GameObject;

                if (prefab != null)
                {
                    var path = loadedPrefabAssets[i];
                    var loadedPrefab = loadedPrefabs[path];
                    loadedPrefab.prefab = prefab;
                    loadedPrefab.OnLoaded?.Invoke();
                    Debug.Log($"Loaded prefab: {prefab.name}");
                }
                else
                {
                    Debug.LogError("Failed to load prefab.");
                }
            }
        }

        protected IEnumerator LoadAllPrefabsInFolderAsync()
        {
            for (int i = 0; i < loadedPrefabAssetsInFolder.Count; i++)
            {
                var prefabPath = loadedPrefabAssetsInFolder[i];
                var loadedPrefabs = Resources.LoadAll<GameObject>(prefabPath);
                var loadedPrefabInFolder = new TLoadedPrefabInFolder
                {
                    prefabs = loadedPrefabs.ToList(),
                };
                loadedPrefabsInFolder.Add(prefabPath, loadedPrefabInFolder);
            }
            yield return null;
        }

        public GameObject GetPrefab(string prefabPath)
        {
            if (loadedPrefabs.TryGetValue(prefabPath, out var loadedPrefab))
            {
                return loadedPrefab.prefab;
            }
            Debug.LogError($"Prefab not found: {prefabPath}");
            return null;
        }
        public List<GameObject> GetPrefabsInFolder(string folderPath)
        {
            if (loadedPrefabsInFolder.TryGetValue(folderPath, out var loadedPrefabInFolder))
            {
                return loadedPrefabInFolder.prefabs;
            }
            Debug.LogError($"Prefabs in folder not found: {folderPath}");
            return null;
        }
        #endregion

        #region Addressable
        protected virtual void AddAddressableLabels()
        {
            // Add the paths of the addressable assets you want to load here
            // loadedAddressableLabels.Add("Props/Prop1");
            // loadedAddressableLabels.Add("Props/Prop2");
            // loadedAddressableLabels.Add("Props/Prop3");
        }
        protected IEnumerator LoadAllAddressableAssetsCoroutine()
        {
            loadTask = LoadAllAddressableAssetsAsync();
            yield return new WaitUntil(() => loadTask.IsCompleted);
            if (loadTask.IsFaulted)
            {
                Debug.LogException(loadTask.Exception);
            }
        }

        protected async System.Threading.Tasks.Task LoadAllAddressableAssetsAsync()
        {
            var loadTasks = new List<System.Threading.Tasks.Task>();

            foreach (var label in loadedAddressableLabels)
            {
                loadedAddressableAssets[label] = new TLoadedLabelAsset
                {
                    prefabs = new List<GameObject>(),
                };

                loadTasks.Add(LoadLabelAssetsAsync(label));
            }
            await System.Threading.Tasks.Task.WhenAll(loadTasks);
        }
        private async System.Threading.Tasks.Task LoadLabelAssetsAsync(string label)
        {
            var handle = Addressables.LoadAssetsAsync<GameObject>(label, OnPrefabLoaded);
            IList<GameObject> result = await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                OnAllPrefabsLoaded(handle, label);
            }
            else
            {
                Debug.LogError($"Failed to load label: {label}");
            }
        }

        protected virtual void OnPrefabLoaded(GameObject prefab)
        {
            // just override this function
        }

        protected virtual void OnAllPrefabsLoaded(AsyncOperationHandle<IList<GameObject>> handle, string label)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var loadedAddressableAsset = loadedAddressableAssets[label];
                loadedAddressableAsset.prefabs.AddRange(handle.Result);
                loadedAddressableAsset.OnLoaded?.Invoke();
                Debug.Log($"Loaded addressable asset: {label}");
            }
            else
            {
                Debug.LogError($"Failed to load addressable asset: {label}");
            }
        }

        private void RegisterAddressableLoaded(string label, Action onLoaded)
        {
            if (loadedAddressableAssets.TryGetValue(label, out var loadedAddressableAsset))
            {
                loadedAddressableAsset.OnLoaded += onLoaded;
            }
            else
            {
                Debug.LogError($"Label {label} not found in loaded addressable assets.");
            }
        }
        /// <summary>
        /// Get the prefabs of the specified label. If the prefabs are not loaded yet, register a callback to load them.
        /// </summary>
        /// <param name="onLoaded"></param>
        /// <returns></returns>
        public void HandleWhenPrefabsLoaded(string label, Action onLoaded)
        {
            var prefabs = loadedAddressableAssets[label].prefabs;
            if (prefabs == null || prefabs.Count == 0)
            {
                RegisterAddressableLoaded(label, onLoaded);
            }
            else
            {
                onLoaded?.Invoke();
            }
        }


        #endregion
    }
}

