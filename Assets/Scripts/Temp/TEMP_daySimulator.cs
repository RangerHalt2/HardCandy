using UnityEngine;

public class TEMP_daySimulator : MonoBehaviour, IInteractable
{
    [SerializeField] private float finalDay = 5f;             // game ends once this day is over
    [SerializeField] private float moneyWin = 25f;

    [SerializeField] private Canvas gameCanvas;
    [SerializeField] private Canvas winCanvas;
    [SerializeField] private Canvas lossCanvas;

    public bool canInteract { get; set; } = true;
    public string[] denyText { get; set; }

    private void Start()
    {
        if (DayManager.Instance == null)
        {
            Debug.LogError("TEMP_daySimulator - no DayManager in scene");
            return;
        }
        DayManager.Instance.OnNewDay += CheckGameState;
    }

    private void OnDestroy()
    {
        if (DayManager.Instance != null) DayManager.Instance.OnNewDay -= CheckGameState;
    }

    public void Interact()
    {
        if (canInteract && DayManager.Instance != null)
        {
            DayManager.Instance.NextDay();
        }
    }

    private void CheckGameState(int day)
    {
        if(day > finalDay)
        {
            PlayerMoney pm = GameObject.FindAnyObjectByType<PlayerMoney>();
            if(pm.CurrentMoney >= moneyWin)
            {
                gameCanvas.gameObject.SetActive(false);
                winCanvas.gameObject.SetActive(true);
            }
            else
            {
                gameCanvas.gameObject.SetActive(false);
                lossCanvas.gameObject.SetActive(true);
            }
            PlayerController pc = GameObject.FindAnyObjectByType<PlayerController>();
            pc.blockInputs = true;
            DayManager.Instance.enabled = false;              // stop the clock

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
