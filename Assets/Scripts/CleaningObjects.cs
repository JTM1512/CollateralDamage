using UnityEngine;
using UnityEngine.InputSystem;

public class CleaningObjects : MonoBehaviour
{
    [Header("Cleaning")]
    [SerializeField] private float cleaningRadius = 2.5f;
    [SerializeField] private LayerMask cleanableLayers = ~0;
    [SerializeField] private Key interactKey = Key.E;

    [Header("Scoring (Optional)")]
    [SerializeField] private bool awardScore = true;
    [SerializeField] private int scorePerClean = 1;

    private void Update()
    {
        if (Keyboard.current == null || !Keyboard.current[interactKey].wasPressedThisFrame)
        {
            return;
        }

        TryCleanNearestObject();
    }

    private void TryCleanNearestObject()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, cleaningRadius, cleanableLayers, QueryTriggerInteraction.Collide);
        if (nearbyColliders.Length == 0)
        {
            return;
        }

        Collider nearest = null;
        float nearestSqrDistance = float.MaxValue;

        foreach (Collider candidate in nearbyColliders)
        {
            float sqrDistance = (candidate.transform.position - transform.position).sqrMagnitude;
            if (sqrDistance < nearestSqrDistance)
            {
                nearestSqrDistance = sqrDistance;
                nearest = candidate;
            }
        }

        if (nearest != null)
        {
            if (awardScore && ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(scorePerClean);
            }
            Destroy(nearest.gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.9f, 1f, 0.35f);
        Gizmos.DrawSphere(transform.position, cleaningRadius);
    }
}

// Created with AI assistance (Cursor + GPT-5.2).
