// Editor-only utility: always start Play Mode from MainMenu,
// then restore the previously open scene when exiting Play Mode.

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class PlayModeStartScene
{
    private const string StartSceneName = "MainMenu";
    private const string PrevScenePathKey = "CollateralDamage.PlayModeStartScene.PrevScenePath";

    static PlayModeStartScene()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            var activeScene = EditorSceneManager.GetActiveScene();
            if (activeScene.name == StartSceneName)
            {
                EditorPrefs.DeleteKey(PrevScenePathKey);
                return;
            }

            // If you have unsaved changes, Unity will prompt you here.
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                // User cancelled entering play mode.
                EditorApplication.isPlaying = false;
                return;
            }

            EditorPrefs.SetString(PrevScenePathKey, activeScene.path);

            var startScenePath = FindScenePathByName(StartSceneName);
            if (string.IsNullOrEmpty(startScenePath))
            {
                Debug.LogError($"PlayModeStartScene: Could not find a scene named '{StartSceneName}'. Make sure it exists and is in your project.");
                return;
            }

            EditorSceneManager.OpenScene(startScenePath);
        }

        if (state == PlayModeStateChange.EnteredEditMode)
        {
            if (!EditorPrefs.HasKey(PrevScenePathKey))
            {
                return;
            }

            var prevPath = EditorPrefs.GetString(PrevScenePathKey, "");
            EditorPrefs.DeleteKey(PrevScenePathKey);

            if (!string.IsNullOrEmpty(prevPath))
            {
                EditorSceneManager.OpenScene(prevPath);
            }
        }
    }

    private static string FindScenePathByName(string sceneName)
    {
        var guids = AssetDatabase.FindAssets($"{sceneName} t:Scene");
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var foundName = System.IO.Path.GetFileNameWithoutExtension(path);
            if (string.Equals(foundName, sceneName, System.StringComparison.OrdinalIgnoreCase))
            {
                return path;
            }
        }
        return null;
    }
}
#endif

// Created with AI assistance (Cursor + GPT-5.2).

