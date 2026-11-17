using UnityEngine;
using UnityEngine.InputSystem;

public class PauseController : MonoBehaviour
{
    public static PauseController instance;

    [Tooltip("Set to false (e.g. while in a workbench) so ESC doesn't open the pause menu.")]
    public bool canPause = true;

    [SerializeField] private int gameplayPageIndex = 0;
    [SerializeField] private int pausePageIndex = 1;

    public bool IsPaused { get; private set; }

    private bool wasAllowedLastFrame = true;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    private void Update()
    {
        // Only allow pausing if canPause was also true on the previous frame.
        // This stops the same ESC press that closes a workbench from also pausing.
        bool allowed = canPause && wasAllowedLastFrame;
        wasAllowedLastFrame = canPause;

        if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame) return;

        if (IsPaused) Resume();
        else if (allowed) Pause();
    }

    public void Pause()
    {
        if (UIController.instance == null)
        {
            Debug.LogWarning("PAUSE CONTROLLER - NO UI CONTROLLER FOUND");
            return;
        }

        Debug.Log("PAUSE CONTROLLER - PAUSED");
        IsPaused = true;
        UIController.instance.GoToPage(pausePageIndex); // shows menu, frees cursor, blocks player
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        Debug.Log("PAUSE CONTROLLER - RESUMED");
        IsPaused = false;
        Time.timeScale = 1f;
        UIController.instance.GoToPage(gameplayPageIndex); // hides menu, locks cursor, unblocks player
    }
}