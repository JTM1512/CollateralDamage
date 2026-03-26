using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class ThirdPersonCameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 lookAtOffset = new Vector3(0f, 1.5f, 0f);

    [Header("Position")]
    [SerializeField] private Vector3 positionOffset = new Vector3(0f, 2.5f, -6f);
    [SerializeField] private float positionSmoothTime = 0.08f;

    [Header("Rotation")]
    [SerializeField] private float rotationSmoothTime = 0.08f;
    [SerializeField] private float mouseSensitivity = 2.5f;
    [SerializeField] private float minPitch = -35f;
    [SerializeField] private float maxPitch = 60f;

    [Header("Cursor (Optional)")]
    [SerializeField] private bool lockCursor = true;
    [SerializeField] private bool hideCursor = true;

    private Vector3 velocity;
    private float yaw;
    private float pitch;
    private Transform targetTransformCache;

    private void Start()
    {
        targetTransformCache = target;

        if (targetTransformCache != null)
        {
            Vector3 dir = transform.position - targetTransformCache.position;
            float distance = dir.magnitude;

            if (distance > 0.0001f)
            {
                // Derive yaw/pitch from current camera position.
                yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                pitch = -Mathf.Asin(dir.y / distance) * Mathf.Rad2Deg;
                pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            }
        }

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = !hideCursor;
        }
    }

    private void LateUpdate()
    {
        if (targetTransformCache == null)
        {
            return;
        }

        // Mouse orbit.
        if (Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            yaw += mouseDelta.x * mouseSensitivity;
            pitch -= mouseDelta.y * mouseSensitivity; // mouse up -> look up
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        Quaternion orbitRot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPosition = targetTransformCache.position + orbitRot * positionOffset;

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, positionSmoothTime);

        Vector3 lookAtPoint = targetTransformCache.position + lookAtOffset;
        Quaternion desiredRotation = Quaternion.LookRotation(lookAtPoint - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            rotationSmoothTime <= 0f ? 1f : Time.deltaTime / rotationSmoothTime
        );
    }
}

// Created with AI assistance (Cursor + GPT-5.2).

