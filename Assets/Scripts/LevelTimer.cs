using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[DisallowMultipleComponent]
public class LevelTimer : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private float levelTimeSeconds = 300f; // 5 minutes
    [SerializeField] private bool startOnAwake = true;

    [Header("Player (for death trigger)")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private Health playerHealth;

    [Header("UI (Optional)")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private bool showMinutesSeconds = true;
    [SerializeField] private string label = "Time Left: ";
    [SerializeField] private float warningTimeSeconds = 60f;
    [SerializeField] private Color warningColor = Color.red;
    [SerializeField] private Color normalColor = Color.white;

    private float remainingSeconds;
    private bool isFinished;

    private void Awake()
    {
        if (!startOnAwake)
        {
            return;
        }

        remainingSeconds = Mathf.Max(0f, levelTimeSeconds);
        isFinished = false;

        if (timeText != null)
        {
            normalColor = timeText.color;
        }
    }

    private void Start()
    {
        if (playerHealth == null)
        {
            ResolvePlayerHealth();
        }

        // If startOnAwake was false, start here once references are ready.
        if (!startOnAwake)
        {
            remainingSeconds = Mathf.Max(0f, levelTimeSeconds);
            isFinished = false;
        }

        UpdateTimeUI(forceHide: false);
    }

    private void Update()
    {
        // If the player already died, stop counting.
        if (playerHealth != null && playerHealth.IsDead)
        {
            return;
        }

        if (isFinished)
        {
            return;
        }

        if (remainingSeconds <= 0f)
        {
            FinishTimerAndKillPlayer();
            return;
        }

        // IMPORTANT: Use scaled delta time so Time.timeScale=0 (pause) freezes the countdown.
        remainingSeconds -= Time.deltaTime;
        if (remainingSeconds <= 0f)
        {
            remainingSeconds = 0f;
            FinishTimerAndKillPlayer();
        }

        UpdateTimeUI(forceHide: false);
    }

    private void FinishTimerAndKillPlayer()
    {
        if (isFinished)
        {
            return;
        }

        isFinished = true;

        // Trigger the death flow by reducing the player's health to zero.
        if (playerHealth == null)
        {
            ResolvePlayerHealth();
        }

        if (playerHealth != null)
        {
            // TakeDamage clamps internally; we ensure we exceed remaining HP.
            float amount = Mathf.Max(0.01f, playerHealth.CurrentHealth + 1f);
            playerHealth.TakeDamage(amount, gameObject);
        }
        else
        {
            Debug.LogError($"{nameof(LevelTimer)}: Could not find player Health; death screen will not trigger.");
        }

        UpdateTimeUI(forceHide: false);
    }

    private void ResolvePlayerHealth()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null)
        {
            return;
        }

        playerHealth = player.GetComponent<Health>();
        if (playerHealth == null)
        {
            playerHealth = player.GetComponentInParent<Health>();
        }
    }

    private void UpdateTimeUI(bool forceHide)
    {
        if (timeText == null)
        {
            return;
        }

        if (forceHide)
        {
            timeText.gameObject.SetActive(false);
            return;
        }

        timeText.gameObject.SetActive(true);
        timeText.color = remainingSeconds <= warningTimeSeconds ? warningColor : normalColor;

        if (!showMinutesSeconds)
        {
            timeText.text = $"{label}{remainingSeconds:0}s";
            return;
        }

        int minutes = Mathf.FloorToInt(remainingSeconds / 60f);
        int seconds = Mathf.FloorToInt(remainingSeconds % 60f);
        timeText.text = $"{label}{minutes:00}:{seconds:00}";
    }

    public void ResetTimer()
    {
        remainingSeconds = Mathf.Max(0f, levelTimeSeconds);
        isFinished = false;
        UpdateTimeUI(forceHide: false);
    }
}

// Created with AI assistance (Cursor + GPT-5.2).

