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


    [Header("Input Actions")]
    public InputAction moveAction { get; private set; }
    public InputAction lookAction { get; private set; }
    public InputAction sprintAction { get; private set; }
    public InputAction jumpAction { get; private set; }
    public InputAction clickAction { get; private set; }
    public InputAction pointAction { get; private set; }

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
    public Vector2 PointerPosition => pointAction.ReadValue<Vector2>();

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
    }

    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        sprintAction.Enable();
        jumpAction.Enable();
        clickAction.Enable();
        pointAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        sprintAction.Disable();
        jumpAction.Disable();
        clickAction.Disable();
        pointAction.Disable();
    }
}