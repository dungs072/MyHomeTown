using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BaseEngine.Editor
{
    /// <summary>
    /// This class forces the editor to start with a specific scene when entering play mode.
    /// It saves the current scene and restores it after exiting play mode.
    /// </summary>
    [InitializeOnLoad]
    public class ForceSceneStartFirst
    {
        private const string bootScenePath = "Assets/Scenes/BootScene.unity"; // Adjust this path
        private static string previousScenePath;

        static ForceSceneStartFirst()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                    // Save current scene path before switching
                    previousScenePath = EditorSceneManager.GetActiveScene().path;
                    if (previousScenePath != bootScenePath)
                    {
                        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        {
                            EditorSceneManager.OpenScene(bootScenePath);
                        }
                        else
                        {
                            EditorApplication.isPlaying = false; // cancel play
                        }
                    }
                    break;

                case PlayModeStateChange.EnteredEditMode:
                    // Restore the previous scene after stopping
                    if (!string.IsNullOrEmpty(previousScenePath) &&
                        previousScenePath != bootScenePath &&
                        System.IO.File.Exists(previousScenePath)) // Check in case the scene was deleted
                    {
                        EditorSceneManager.OpenScene(previousScenePath);
                    }
                    break;
            }
        }
    }
}
