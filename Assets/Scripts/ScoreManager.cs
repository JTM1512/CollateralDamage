using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score")]
    [SerializeField] private int score;

    [Header("Events")]
    [SerializeField] private UnityEvent<int> onScoreChanged;

    public int Score => score;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ResetScore()
    {
        score = 0;
        onScoreChanged?.Invoke(score);
    }

    public void AddScore(int amount)
    {
        score += amount;
        onScoreChanged?.Invoke(score);
    }

    public void AddRepairScore(float repairSeconds, float scorePerSecond = 1f)
    {
        // "Equivalent to the amount of seconds" -> 1 point per second by default.
        // We round to int for a clean score number.
        int points = Mathf.RoundToInt(repairSeconds * scorePerSecond);
        AddScore(points);
    }
}

// Created with AI assistance (Cursor + GPT-5.2).

