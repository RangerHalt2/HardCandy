using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.6f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpForce = 5.0f;

    [Header("Look Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float lookSensitivity = 0.1f;
    [SerializeField] private float maxLookAngle = 85f;

    [Header("Interaction Settings")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private float interactCooldown = 0.5f;
    private bool canInteract = true;
    private bool interactFail = false;

    [HideInInspector] public bool blockInputs = false;

    private CharacterController characterController;
    private InputHandler inputHandler;

    private Vector3 currentMovement;
    private float verticalVelocity;
    private float cameraPitch = 0f;
    

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        // Lock and hide cursor for standard first-person gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        // InputHandler is a singleton on its own object, grab it here so its Awake has already run
        inputHandler = InputHandler.Instance;
    }

    private void Update()
    {
        if (inputHandler == null) return;

        HandleMovement();
        HandleRotation();
        if (inputHandler.InteractTriggered && !blockInputs)
        {
            Interact();
        }
    }

    private void HandleMovement()
    {
        if (blockInputs) return;
        // 1. Calculate target speed based on sprint state
        float speed = walkSpeed * (inputHandler.Is_Sprinting ? sprintMultiplier : 1f);

        // 2. Map input to local horizontal movement and convert to world space
        Vector3 inputDirection = new Vector3(inputHandler.MoveInput.x, 0f, inputHandler.MoveInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection);

        if (worldDirection.sqrMagnitude > 1f)
        {
            worldDirection.Normalize();
        }

        currentMovement.x = worldDirection.x * speed;
        currentMovement.z = worldDirection.z * speed;

        HandleJumping();
        currentMovement.y = verticalVelocity;

        // 4. Move controller
        characterController.Move(currentMovement * Time.deltaTime);
    }

    void HandleJumping()
    {
        if (blockInputs) return;
        if (characterController.isGrounded)
        {
            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            if (inputHandler.JumpTriggered)
            {
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    private void HandleRotation()
    {
        if (blockInputs) return;
        Vector2 look = inputHandler.LookInput * lookSensitivity;

        // Horizontal rotation: Rotate the player body left/right (Yaw)
        transform.Rotate(Vector3.up * look.x);

        // Vertical rotation: Tilt the camera up/down (Pitch) with clamping
        cameraPitch -= look.y;
        cameraPitch = Mathf.Clamp(cameraPitch, -maxLookAngle, maxLookAngle);

        if (cameraTransform != null)
        {
            cameraTransform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
        }
    }

    private void Interact()
    {
        if (!canInteract) return;

        canInteract = false;

        Transform cam = cameraTransform != null ? cameraTransform : (Camera.main != null ? Camera.main.transform : null);
        if (cam == null)
        {
            Debug.LogWarning("No main camera found for interaction!");
            return;
        }

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableMask, QueryTriggerInteraction.Ignore))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                if (interactable.canInteract)
                {
                    interactable.Interact();
                }
                else
                {
                    if (interactable.denyText != null && interactable.denyText.Length > 0)
                    {
                        Debug.Log(interactable.denyText[0]);
                    }
                }
            }
        }
            StartCoroutine(InteractionCooldown());
    }

    private IEnumerator InteractionCooldown()
    {
        yield return new WaitForSeconds(interactCooldown);
        canInteract = true;
    }
}