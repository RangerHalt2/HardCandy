using UnityEngine;

//Win: own every part in the requirements list
//Loss: a requirement's day ends without owning its part, or money drops to the debt limit
public class WinLossManager : MonoBehaviour
{
    public static WinLossManager Instance { get; private set; }

    [System.Serializable]
    public class PartRequirement
    {
        public PrisonPart_ScriptableObjects part;
        [Tooltip("Must own the part by the end of this day")]
        public int day;
    }

    [Header("Rules")]
    [SerializeField] private PartRequirement[] requirements;
    [Tooltip("Lose when money is at or below this (e.g. -50)")]
    [SerializeField] private int debtLimit = -50;

    [Header("UI")]
    [SerializeField] private Canvas gameCanvas;
    [SerializeField] private Canvas winCanvas;
    [SerializeField] private Canvas lossCanvas;

    private PlayerMoney playerMoney;
    private bool gameOver = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerMoney = FindAnyObjectByType<PlayerMoney>();

        if (DayManager.Instance == null)
        {
            Debug.LogError("WIN LOSS MANAGER - no DayManager in scene");
            return;
        }
        DayManager.Instance.OnNewDay += CheckDeadlines;
    }

    private void OnDestroy()
    {
        if (DayManager.Instance != null) DayManager.Instance.OnNewDay -= CheckDeadlines;
    }

    private void Update()
    {
        if (!gameOver && playerMoney != null && playerMoney.CurrentMoney <= debtLimit)
        {
            Debug.Log("WIN LOSS MANAGER - too far in debt");
            EndGame(false);
        }
    }

    //Runs when a new day starts, so "day" is the day that just began
    private void CheckDeadlines(int day)
    {
        if (gameOver) return;

        foreach (PartRequirement req in requirements)
        {
            if (day > req.day && !PrisonOwnership.Instance.Owns(req.part))
            {
                Debug.Log("WIN LOSS MANAGER - missed deadline for " + req.part.PartName);
                EndGame(false);
                return;
            }
        }
    }

    public void CheckWin()
    {
        if (gameOver || requirements.Length == 0) return;

        foreach (PartRequirement req in requirements)
        {
            if (!PrisonOwnership.Instance.Owns(req.part)) return;
        }
        EndGame(true);
    }

    private void EndGame(bool won)
    {
        gameOver = true;

        if (gameCanvas != null) gameCanvas.gameObject.SetActive(false);
        if (won && winCanvas != null) winCanvas.gameObject.SetActive(true);
        if (!won && lossCanvas != null) lossCanvas.gameObject.SetActive(true);

        PlayerController pc = FindAnyObjectByType<PlayerController>();
        if (pc != null) pc.blockInputs = true;
        if (DayManager.Instance != null) DayManager.Instance.enabled = false;   // stop the clock
        if (PauseController.instance != null) PauseController.instance.canPause = false;   // resuming would unblock the player

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
