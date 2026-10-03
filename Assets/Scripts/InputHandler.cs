using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference jumpAction;


    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public float SprintValue { get; private set; }
    public bool JumpTriggered { get; private set; }

    private void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
        sprintAction.action.Enable();
        jumpAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
        sprintAction.action.Disable();
        jumpAction.action.Disable();
    }

    private void Update()
    {
        MoveInput = moveAction.action.ReadValue<Vector2>();
        LookInput = lookAction.action.ReadValue<Vector2>();
        SprintValue = sprintAction.action.ReadValue<float>();
        JumpTriggered = jumpAction.action.WasPressedThisFrame();
    }
}