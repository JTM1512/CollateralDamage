using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public class ReparingObjects : MonoBehaviour
{
    [Header("Repair Settings")]
    [SerializeField] private float repairDuration = 3f;
    [SerializeField] private float repairDistance = 2f;
    [SerializeField] private Transform repairOrigin;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private Key holdRepairKey = Key.F;
    [SerializeField] private bool resetProgressWhenOutOfRange = false;

    [Header("Replacement")]
    [SerializeField] private GameObject fixedObjectPrefab;
    [SerializeField] private bool parentFixedObjectToSameParent = true;
    [SerializeField] private bool destroyBrokenAfterReplacement = true;

    [Header("Fixed Vehicle Orientation")]
    [SerializeField] private bool forceFixedUpright = true;
    [SerializeField] private bool preserveBrokenYaw = true;
    [SerializeField] private float fixedYawOffsetDegrees = 0f;

    [Header("State Visuals (Optional)")]
    [SerializeField] private GameObject brokenStateVisual;
    [SerializeField] private GameObject repairedStateVisual;

    [Header("UI (Optional)")]
    [SerializeField] private Slider repairProgressBar;
    [Tooltip("Prefab for the world-space UI (should include a Canvas + Slider). The script will find the Slider in the instantiated prefab.")]
    [SerializeField] private GameObject worldSpaceProgressBarPrefab;
    [SerializeField] private Transform progressBarAnchor;
    [SerializeField] private Vector3 progressBarOffset = new Vector3(0f, 2f, 0f);
    [SerializeField] private bool faceCamera = true;
    [SerializeField] private bool lockUIAboveObject = true;
    [SerializeField] private float objectTopPadding = 0.3f;
    [SerializeField] private bool showRepairPrompt = true;
    [SerializeField] private string repairPromptText = "Repair";
    [SerializeField] private bool promptUsesFirstTextFound = true;

    [Header("Events")]
    [SerializeField] private UnityEvent onRepairStarted;
    [SerializeField] private UnityEvent onRepairCompleted;

    [Header("Scoring (Optional)")]
    [SerializeField] private bool awardRepairScore = true;
    [SerializeField] private float scorePerSecond = 1f;

    public bool IsRepaired => isRepaired;
    public float RepairPercent => repairDuration <= 0f ? 1f : Mathf.Clamp01(repairProgress / repairDuration);

    private Transform playerTransformCache;
    private Transform repairOriginCache;
    private Camera mainCamera;

    private bool isRepaired;
    private bool isRepairing;
    private float repairProgress;
    private Text repairPrompt;
    private TMP_Text repairPromptTMP;
    private Transform spawnedProgressUIRoot;

    private void Awake()
    {
        mainCamera = Camera.main;
        repairOriginCache = repairOrigin != null ? repairOrigin : transform;

        CreateWorldSpaceProgressBarIfNeeded();
        ApplyStateVisuals();
        UpdateProgressUI(forceHide: true);
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            UpdateProgressUI(forceHide: true);
            return;
        }

        if (isRepaired)
        {
            UpdateProgressUI(forceHide: true);
            return;
        }

        if (playerTransformCache == null)
        {
            GameObject playerGo = GameObject.FindGameObjectWithTag(playerTag);
            if (playerGo != null)
            {
                playerTransformCache = playerGo.transform;
            }
        }

        bool inRange = false;
        if (playerTransformCache != null)
        {
            inRange = Vector3.Distance(repairOriginCache.position, playerTransformCache.position) <= repairDistance;
        }

        bool holdingRepair = inRange && Keyboard.current[holdRepairKey].isPressed;

        if (!inRange)
        {
            isRepairing = false;
            if (resetProgressWhenOutOfRange)
            {
                repairProgress = 0f;
            }
            UpdateProgressUI(forceHide: true);
            return;
        }

        // Player is in range; show UI.
        UpdateProgressUI(forceHide: false);

        if (holdingRepair)
        {
            if (!isRepairing)
            {
                isRepairing = true;
                onRepairStarted?.Invoke();
            }

            repairProgress += Time.deltaTime;
            if (repairProgress >= repairDuration)
            {
                CompleteRepair();
            }
        }
        else
        {
            isRepairing = false;
        }
    }

    private void LateUpdate()
    {
        UpdateProgressBarTransform();
    }

    private void CompleteRepair()
    {
        isRepaired = true;
        repairProgress = repairDuration;
        isRepairing = false;
        HideRepairUICompletely();

        if (awardRepairScore && ScoreManager.Instance != null)
        {
            // Add score equivalent to the time it took to repair this item.
            ScoreManager.Instance.AddRepairScore(repairProgress, scorePerSecond);
        }

        // If a fixed prefab is provided, replace the broken object entirely.
        if (fixedObjectPrefab != null)
        {
            Transform fixedParent = parentFixedObjectToSameParent ? transform.parent : null;

            Quaternion fixedRotation = GetFixedSpawnRotation();
            Instantiate(fixedObjectPrefab, transform.position, fixedRotation, fixedParent);

            onRepairCompleted?.Invoke();
            ApplyStateVisuals();

            if (destroyBrokenAfterReplacement)
            {
                Destroy(gameObject);
            }
            return;
        }

        // Otherwise, fall back to optional in-place visuals.
        ApplyStateVisuals();
        onRepairCompleted?.Invoke();
    }

    private void HideRepairUICompletely()
    {
        // If this UI was spawned from prefab, hide the whole root so every child disappears.
        if (spawnedProgressUIRoot != null)
        {
            spawnedProgressUIRoot.gameObject.SetActive(false);
            return;
        }

        // Fallback for manually assigned references.
        UpdateProgressUI(forceHide: true);
    }

    private Quaternion GetFixedSpawnRotation()
    {
        if (!forceFixedUpright)
        {
            return transform.rotation;
        }

        if (!preserveBrokenYaw)
        {
            // Upright with no yaw preservation.
            return Quaternion.Euler(0f, fixedYawOffsetDegrees, 0f);
        }

        // Extract yaw from the broken object's forward projected onto the XZ plane,
        // so the fixed prefab stays upright even if the broken one is lying sideways.
        Vector3 forward = transform.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.0001f)
        {
            forward = transform.right;
            forward.y = 0f;
        }

        Quaternion yawRotation = Quaternion.LookRotation(forward.normalized, Vector3.up);
        if (Mathf.Abs(fixedYawOffsetDegrees) > 0.0001f)
        {
            yawRotation = Quaternion.AngleAxis(fixedYawOffsetDegrees, Vector3.up) * yawRotation;
        }

        return yawRotation;
    }

    private void ApplyStateVisuals()
    {
        if (brokenStateVisual != null)
        {
            brokenStateVisual.SetActive(!isRepaired);
        }

        if (repairedStateVisual != null)
        {
            repairedStateVisual.SetActive(isRepaired);
        }
    }

    private void CreateWorldSpaceProgressBarIfNeeded()
    {
        if (repairProgressBar != null || worldSpaceProgressBarPrefab == null)
        {
            return;
        }

        Transform anchor = progressBarAnchor != null ? progressBarAnchor : transform;

        GameObject spawnedRoot = Instantiate(worldSpaceProgressBarPrefab, anchor.position + progressBarOffset, Quaternion.identity);
        spawnedRoot.transform.SetParent(anchor, worldPositionStays: true);
        spawnedProgressUIRoot = spawnedRoot.transform;

        Slider spawnedBar = spawnedRoot.GetComponentInChildren<Slider>(true);
        if (spawnedBar == null)
        {
            Debug.LogError($"{nameof(ReparingObjects)}: worldSpaceProgressBarPrefab does not contain a Slider component.");
            Destroy(spawnedRoot);
            return;
        }

        repairProgressBar = spawnedBar;

        if (showRepairPrompt)
        {
            // Prefer TextMeshPro label (TMP). Fall back to legacy UnityEngine.UI.Text if needed.
            repairPromptTMP = spawnedRoot.GetComponentInChildren<TMP_Text>(true);
            if (repairPromptTMP != null)
            {
                repairPromptTMP.text = repairPromptText;
            }
            else
            {
                repairPrompt = spawnedRoot.GetComponentInChildren<Text>(true);
                if (repairPrompt != null)
                {
                    repairPrompt.text = repairPromptText;
                }
            }
        }
        else
        {
            repairPrompt = null;
            repairPromptTMP = null;
        }

        UpdateProgressBarTransform();
    }

    private void UpdateProgressUI(bool forceHide)
    {
        if (repairProgressBar == null)
        {
            return;
        }

        repairProgressBar.gameObject.SetActive(!forceHide && !isRepaired);
        repairProgressBar.value = RepairPercent;

        if (repairPrompt != null)
        {
            repairPrompt.gameObject.SetActive(!forceHide && !isRepaired);
        }

        if (repairPromptTMP != null)
        {
            repairPromptTMP.gameObject.SetActive(!forceHide && !isRepaired);
        }
    }

    private void UpdateProgressBarTransform()
    {
        if (repairProgressBar == null)
        {
            return;
        }

        Transform anchor = progressBarAnchor != null ? progressBarAnchor : transform;
        Vector3 targetPosition = anchor.position + progressBarOffset;

        if (lockUIAboveObject)
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                Bounds combinedBounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                {
                    combinedBounds.Encapsulate(renderers[i].bounds);
                }

                targetPosition = new Vector3(
                    combinedBounds.center.x,
                    combinedBounds.max.y + objectTopPadding,
                    combinedBounds.center.z
                ) + new Vector3(progressBarOffset.x, 0f, progressBarOffset.z);
            }
        }

        Transform uiRoot = spawnedProgressUIRoot != null ? spawnedProgressUIRoot : repairProgressBar.transform;
        uiRoot.position = targetPosition;

        if (!faceCamera)
        {
            return;
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera != null)
        {
            uiRoot.forward = mainCamera.transform.forward;
        }
    }

    private void OnValidate()
    {
        repairOriginCache = repairOrigin != null ? repairOrigin : transform;
    }
}

// Created with AI assistance (Cursor + GPT-5.2).
