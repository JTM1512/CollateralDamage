using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = -1f;

    [Header("Events")]
    public UnityEvent onDeath;

    public bool IsDead => isDead;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private bool isDead;

    private void Awake()
    {
        if (currentHealth < 0f)
        {
            currentHealth = maxHealth;
        }

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        isDead = currentHealth <= 0f;
    }

    public void TakeDamage(float amount, GameObject source = null)
    {
        if (isDead)
        {
            return;
        }

        amount = Mathf.Max(0f, amount);
        if (amount <= 0f)
        {
            return;
        }

        currentHealth -= amount;
        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead)
        {
            return;
        }

        amount = Mathf.Max(0f, amount);
        if (amount <= 0f)
        {
            return;
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
    }

    private void Die()
    {
        isDead = true;
        onDeath?.Invoke();
    }
}

// Created with AI assistance (Cursor + GPT-5.2).

