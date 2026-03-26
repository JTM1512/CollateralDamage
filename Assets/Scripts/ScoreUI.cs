using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private bool prefixLabel = true;
    [SerializeField] private string prefix = "Score: ";

    private int lastScore = int.MinValue;

    private void Awake()
    {
        if (scoreText == null)
        {
            scoreText = GetComponentInChildren<TMP_Text>(true);
        }
    }

    private void Update()
    {
        if (scoreText == null)
        {
            return;
        }

        if (ScoreManager.Instance == null)
        {
            // If score manager isn't in scene yet, just keep whatever text is there.
            return;
        }

        int score = ScoreManager.Instance.Score;
        if (score == lastScore)
        {
            return;
        }

        lastScore = score;

        scoreText.text = prefixLabel ? $"{prefix}{score}" : score.ToString();
    }
}

// Created with AI assistance (Cursor + GPT-5.2).

