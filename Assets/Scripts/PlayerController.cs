using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private Key sprintKey = Key.LeftShift;
    [SerializeField] private bool sprintRequiresMovement = true;

    [Header("Gravity")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float groundedStickForce = -2f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("Interaction")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactionLayers = ~0;

    [Header("Health (Optional)")]
    [SerializeField] private Health health;

    private CharacterController characterController;
    private Vector3 velocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (health == null)
        {
            health = GetComponent<Health>();
        }

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        if (health != null)
        {
            health.onDeath.AddListener(HandleDeath);
            // If the scene starts with health already at 0, disable immediately.
            if (health.IsDead)
            {
                HandleDeath();
            }
        }
    }

    private void OnDestroy()
    {
        if (health != null)
        {
            health.onDeath.RemoveListener(HandleDeath);
        }
    }

    private void Update()
    {
        if (health != null && health.IsDead)
        {
            return;
        }

        HandleMovement();
        ApplyGravity();
        HandleInteraction();
    }

    private void HandleMovement()
    {
        Vector2 moveInput = ReadMoveInput();
        float horizontal = moveInput.x;
        float vertical = moveInput.y;

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;
        if (inputDirection.sqrMagnitude < 0.01f)
        {
            characterController.Move(new Vector3(0f, velocity.y, 0f) * Time.deltaTime);
            return;
        }

        bool isSprinting = Keyboard.current != null && Keyboard.current[sprintKey].isPressed;
        float effectiveMoveSpeed = isSprinting ? moveSpeed * Mathf.Max(1f, sprintMultiplier) : moveSpeed;
        if (sprintRequiresMovement && inputDirection.sqrMagnitude < 0.01f)
        {
            effectiveMoveSpeed = moveSpeed;
        }

        Vector3 moveDirection = inputDirection;
        if (cameraTransform != null)
        {
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();
            moveDirection = (cameraForward * inputDirection.z + cameraRight * inputDirection.x).normalized;
        }

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        Vector3 movement = moveDirection * effectiveMoveSpeed;
        movement.y = velocity.y;
        characterController.Move(movement * Time.deltaTime);
    }

    private Vector2 ReadMoveInput()
    {
        if (Keyboard.current == null)
        {
            return Vector2.zero;
        }

        float horizontal = 0f;
        float vertical = 0f;

        if (Keyboard.current.aKey.isPressed) horizontal -= 1f;
        if (Keyboard.current.dKey.isPressed) horizontal += 1f;
        if (Keyboard.current.sKey.isPressed) vertical -= 1f;
        if (Keyboard.current.wKey.isPressed) vertical += 1f;

        return new Vector2(horizontal, vertical).normalized;
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded && velocity.y < 0f)
        {
            velocity.y = groundedStickForce;
        }

        velocity.y += gravity * Time.deltaTime;
    }

    private void HandleInteraction()
    {
        if (health != null && health.IsDead)
        {
            return;
        }

        if (Keyboard.current == null || !Keyboard.current.eKey.wasPressedThisFrame)
        {
            return;
        }

        Vector3 rayOrigin;
        Vector3 rayDirection;

        if (cameraTransform != null)
        {
            rayOrigin = cameraTransform.position;
            rayDirection = cameraTransform.forward;
        }
        else
        {
            rayOrigin = transform.position + Vector3.up;
            rayDirection = transform.forward;
        }

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, interactionDistance, interactionLayers, QueryTriggerInteraction.Collide))
        {
            hit.collider.SendMessage("Interact", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void HandleDeath()
    {
        if (characterController != null)
        {
            characterController.enabled = false;
        }
    }
}

// Created with AI assistance (Cursor + GPT-5.2).
