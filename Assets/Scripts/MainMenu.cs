using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class MainMenu : MonoBehaviour
{
    [Header("Scene Select")]
    [SerializeField] private string gameSceneName = "";
    [SerializeField] private int gameSceneBuildIndex = -1;

    public void StartGame()
    {
        LoadGameScene();
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void LoadGameScene()
    {
        if (gameSceneBuildIndex >= 0)
        {
            SceneManager.LoadScene(gameSceneBuildIndex);
            return;
        }

        if (!string.IsNullOrWhiteSpace(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
            return;
        }

        Debug.LogError($"{nameof(MainMenu)}: Set either {nameof(gameSceneName)} or {nameof(gameSceneBuildIndex)}.");
    }
}

// Created with AI assistance (Cursor + GPT-5.2).

