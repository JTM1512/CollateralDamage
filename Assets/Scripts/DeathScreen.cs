using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[DisallowMultipleComponent]
public class DeathScreen : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private string playerTag = "Player";

    [Header("UI")]
    [SerializeField] private GameObject deathRoot;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private string finalScoreLabel = "Final Score: ";

    [Header("Behavior")]
    [SerializeField] private bool freezeTimeOnDeath = true;

    [Header("Main Menu (for Back button)")]
    [SerializeField] private string mainMenuSceneName = "";
    [SerializeField] private int mainMenuSceneBuildIndex = -1;

    private bool subscribed;

    private void Awake()
    {
        if (deathRoot != null)
        {
            deathRoot.SetActive(false);
        }
        else
        {
            Debug.LogError($"{nameof(DeathScreen)}: Assign {nameof(deathRoot)} in the Inspector.");
        }

        TryResolveAndSubscribe();
    }

    private void OnEnable()
    {
        TryResolveAndSubscribe();
    }

    private void OnDisable()
    {
        UnsubscribeIfNeeded();
    }

    private void Update()
    {
        // If the player spawns later or scripts load order changes, keep trying.
        if (!subscribed)
        {
            TryResolveAndSubscribe();
        }
    }

    private void TryResolveAndSubscribe()
    {
        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
            {
                playerHealth = player.GetComponent<Health>();
                if (playerHealth == null)
                {
                    playerHealth = player.GetComponentInParent<Health>();
                }
            }
        }

        if (playerHealth == null || subscribed)
        {
            return;
        }

        playerHealth.onDeath.AddListener(ShowDeathScreen);
        subscribed = true;

        if (playerHealth.IsDead)
        {
            ShowDeathScreen();
        }
    }

    private void UnsubscribeIfNeeded()
    {
        if (!subscribed || playerHealth == null)
        {
            subscribed = false;
            return;
        }

        playerHealth.onDeath.RemoveListener(ShowDeathScreen);
        subscribed = false;
    }

    private void ShowDeathScreen()
    {
        if (freezeTimeOnDeath)
        {
            Time.timeScale = 0f;
        }

        if (deathRoot != null)
        {
            deathRoot.SetActive(true);
        }

        if (finalScoreText != null && ScoreManager.Instance != null)
        {
            finalScoreText.text = $"{finalScoreLabel}{ScoreManager.Instance.Score}";
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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

        Debug.LogError($"{nameof(DeathScreen)}: Set either {nameof(mainMenuSceneName)} or {nameof(mainMenuSceneBuildIndex)}.");
    }
}

// Created with AI assistance (Cursor + GPT-5.2).

