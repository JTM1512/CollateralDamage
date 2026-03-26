using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pauseRoot;

    [Header("Scene Select")]
    [SerializeField] private string mainMenuSceneName = "";
    [SerializeField] private int mainMenuSceneBuildIndex = -1;

    [Header("Pause Toggle")]
    [SerializeField] private bool toggleWithEscape = true;
    [SerializeField] private bool showCursorOnPause = true;
    [SerializeField] private CursorLockMode resumeCursorLockState = CursorLockMode.Locked;

    private bool isPaused;

    private void Awake()
    {
        if (pauseRoot != null)
        {
            pauseRoot.SetActive(false);
        }
    }

    private void Update()
    {
        if (!toggleWithEscape || Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pauseRoot != null)
        {
            pauseRoot.SetActive(true);
        }

        if (showCursorOnPause)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseRoot != null)
        {
            pauseRoot.SetActive(false);
        }

        Cursor.visible = false;
        Cursor.lockState = resumeCursorLockState;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;

        if (mainMenuSceneBuildIndex >= 0)
        {
            SceneManager.LoadScene(mainMenuSceneBuildIndex);
            return;
        }

        if (!string.IsNullOrWhiteSpace(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
            return;
        }

        Debug.LogError($"{nameof(PauseMenu)}: Set either {nameof(mainMenuSceneName)} or {nameof(mainMenuSceneBuildIndex)}.");
    }
}

// Created with AI assistance (Cursor + GPT-5.2).

