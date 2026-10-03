using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(InputHandler))]
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

    private CharacterController characterController;
    private InputHandler inputHandler;

    private Vector3 currentMovement;
    private float verticalVelocity;
    private float cameraPitch = 0f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputHandler = GetComponent<InputHandler>();

        // Lock and hide cursor for standard first-person gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        // 1. Calculate target speed based on sprint state
        float speed = walkSpeed * (inputHandler.SprintValue > 0f ? sprintMultiplier : 1f);

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
}