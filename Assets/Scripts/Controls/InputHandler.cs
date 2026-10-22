using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{

    [SerializeField] public InputActionAsset playerControls;
    [SerializeField] public string actionMapName = "Player";

    [Header("Input Names")]
    [SerializeField] private string movement = "Move";
    [SerializeField] private string look = "Look";
    [SerializeField] private string sprint = "Sprint";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string click = "Click";
    [SerializeField] private string point = "Point";
    [SerializeField] private string hotbar = "Hotbar";
    [SerializeField] private string scroll = "Scroll";
    [SerializeField] private string interact = "Interact";
    [SerializeField] private string escape = "Exit";


    [Header("Input Actions")]
    public InputAction moveAction { get; private set; }
    public InputAction lookAction { get; private set; }
    public InputAction sprintAction { get; private set; }
    public InputAction jumpAction { get; private set; }
    public InputAction clickAction { get; private set; }
    public InputAction pointAction { get; private set; }
    public InputAction hotBarAction { get; private set; }
    public InputAction scrollAction { get; private set; }
    public InputAction interactAction { get; private set; }
    public InputAction escapeAction { get; private set; }

    /*
    [Header("Input Action References")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference clickAction;
    */

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool Is_Sprinting { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool ClickTriggered { get; private set; }
    public float ScrollValue { get; private set; }
    public bool InteractTriggered { get; private set; }
    public bool EscapeTriggered { get; private set; }
    public Vector2 PointerPosition => pointAction.ReadValue<Vector2>();

    public event Action<int> OnHotbarSelected;

    public static InputHandler Instance { get; private set; }

    private void Awake()
    {
        //LB: Handles instance related stuff, still unsure where and when the destroy code runs? It never seems necessary
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        RegisterActionMapping();
        RegisterInputActions();
    }


    private void RegisterActionMapping()
    {
        InputActionMap map = playerControls.FindActionMap(actionMapName);

        moveAction = map.FindAction(movement);
        lookAction = map.FindAction(look);
        sprintAction = map.FindAction(sprint);
        jumpAction = map.FindAction(jump);
        clickAction = map.FindAction(click);
        pointAction = map.FindAction(point);
        hotBarAction = map.FindAction(hotbar);
        scrollAction = map.FindAction(scroll);
        interactAction = map.FindAction(interact);
        escapeAction = map.FindAction(escape);
    }

    private void RegisterInputActions()
    {
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;

        lookAction.performed += context => LookInput = context.ReadValue<Vector2>();
        lookAction.canceled += context => LookInput = Vector2.zero;

        sprintAction.performed += context => Is_Sprinting = true;
        sprintAction.canceled += context => Is_Sprinting = false;

        jumpAction.performed += context => JumpTriggered = true;
        jumpAction.canceled += context => JumpTriggered = false;

        clickAction.performed += context => ClickTriggered = true;
        clickAction.canceled += context => ClickTriggered = false;

        hotBarAction.performed += OnHotbarPerformed;

        scrollAction.performed += context => ScrollValue = context.ReadValue<float>();
        scrollAction.canceled += context => ScrollValue = 0f;

        interactAction.performed += context => InteractTriggered = true;
        interactAction.canceled += context => InteractTriggered = false;

        escapeAction.performed += context => EscapeTriggered = true;
        escapeAction.canceled += context => EscapeTriggered = false;
    }

    #region Hotbar Event Handling

    private void OnHotbarPerformed(InputAction.CallbackContext context)
    {
        // Index of the binding that triggered this: 0 for "1", 1 for "2", etc.
        int slot = context.action.GetBindingIndexForControl(context.control);
        OnHotbarSelected?.Invoke(slot);
    }

    #endregion


    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        sprintAction.Enable();
        jumpAction.Enable();
        clickAction.Enable();
        pointAction.Enable();
        hotBarAction.Enable();
        scrollAction.Enable();
        interactAction.Enable();
        escapeAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        sprintAction.Disable();
        jumpAction.Disable();
        clickAction.Disable();
        pointAction.Disable();
        hotBarAction.Disable();
        scrollAction.Disable();
        interactAction.Disable();
        escapeAction.Disable();
    }
}