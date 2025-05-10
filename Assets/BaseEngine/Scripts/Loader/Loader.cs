using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
namespace BaseEngine
{
    // This class is responsible for loading assets and managing the loading process.
    // It should be attached to a GameObject in the scene.
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
    public class Loader : MonoBehaviour
    {
        public static Loader Instance => instance;
        protected List<string> loadedPrefabAssets = new();
        protected List<string> loadedPrefabAssetsInFolder = new();
        protected Dictionary<string, TLoadedPrefab> loadedPrefabs = new();
        protected Dictionary<string, TLoadedPrefabInFolder> loadedPrefabsInFolder = new();
        public bool IsPrefabsLoaded { get; private set; } = false;
        public bool IsPrefabsInFolderLoaded { get; private set; } = false;
        private static Loader instance;

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
            AddPrefabPath();
            AddPrefabPathInFolder();
            StartCoroutine(LoadAllPrefabsAsync());
            StartCoroutine(LoadAllPrefabsInFolderAsync());
        }

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
            IsPrefabsLoaded = false;
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
            IsPrefabsLoaded = true;
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
    }
}

