using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PlayerHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Text healthText;

    [Header("Display")]
    [SerializeField] private bool autoFindPlayerByTag = true;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool showAsWholeNumbers = true;

    private void Awake()
    {
        TryResolveReferences();
        SyncMaxValue();
        RefreshUI();
    }

    private void Update()
    {
        if (playerHealth == null || healthSlider == null)
        {
            TryResolveReferences();
            if (playerHealth == null || healthSlider == null)
            {
                return;
            }
        }

        RefreshUI();
    }

    private void TryResolveReferences()
    {
        if (playerHealth == null && autoFindPlayerByTag)
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

        if (healthSlider == null)
        {
            healthSlider = GetComponentInChildren<Slider>();
        }
    }

    private void SyncMaxValue()
    {
        if (healthSlider == null || playerHealth == null)
        {
            return;
        }

        healthSlider.minValue = 0f;
        healthSlider.maxValue = playerHealth.MaxHealth;
    }

    private void RefreshUI()
    {
        SyncMaxValue();

        healthSlider.value = playerHealth.CurrentHealth;
        if (healthText == null)
        {
            return;
        }

        if (showAsWholeNumbers)
        {
            int cur = Mathf.RoundToInt(playerHealth.CurrentHealth);
            int max = Mathf.RoundToInt(playerHealth.MaxHealth);
            healthText.text = $"{cur} / {max}";
        }
        else
        {
            healthText.text = $"{playerHealth.CurrentHealth:0.0} / {playerHealth.MaxHealth:0.0}";
        }
    }
}

// Created with AI assistance (Cursor + GPT-5.2).

