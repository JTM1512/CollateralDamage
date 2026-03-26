using UnityEngine;

[DisallowMultipleComponent]
public class HazardZone : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float damagePerSecond = 10f;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float radius = 3f;
    [SerializeField] private Transform zoneCenter;
    [SerializeField] private Transform playerTransformOverride;

    [Header("Optional Debug")]
    [SerializeField] private bool logDamageEvents = false;

    private Health cachedPlayerHealth;
    private Transform playerTransformCache;

    private void Update()
    {
        if (playerTransformCache == null)
        {
            playerTransformCache = playerTransformOverride;
            if (playerTransformCache == null)
            {
                GameObject playerGo = GameObject.FindGameObjectWithTag(playerTag);
                if (playerGo != null)
                {
                    playerTransformCache = playerGo.transform;
                }
            }
        }

        if (cachedPlayerHealth == null && playerTransformCache != null)
        {
            cachedPlayerHealth = playerTransformCache.GetComponentInParent<Health>();
        }

        if (cachedPlayerHealth == null || cachedPlayerHealth.IsDead || playerTransformCache == null)
        {
            return;
        }

        Vector3 center = zoneCenter != null ? zoneCenter.position : transform.position;
        bool inZone = Vector3.Distance(center, playerTransformCache.position) <= radius;
        if (!inZone)
        {
            return;
        }

        float damageThisFrame = damagePerSecond * Time.deltaTime;
        cachedPlayerHealth.TakeDamage(damageThisFrame, gameObject);

        if (logDamageEvents && damageThisFrame > 0f)
        {
            Debug.Log($"HazardZone damaged player: {damageThisFrame:0.###}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.35f);
        Vector3 center = zoneCenter != null ? zoneCenter.position : transform.position;
        Gizmos.DrawSphere(center, radius);
    }
}

// Created with AI assistance (Cursor + GPT-5.2).

