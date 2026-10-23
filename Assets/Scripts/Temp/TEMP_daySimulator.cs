using TMPro;
using UnityEngine;

public class TEMP_daySimulator : MonoBehaviour, IInteractable
{
    [SerializeField] private float day = 0f;
    [SerializeField] private float finalDay = 5f;
    [SerializeField] private float moneyWin = 25f;

    [SerializeField] private Canvas gameCanvas;
    [SerializeField] private Canvas winCanvas;
    [SerializeField] private Canvas lossCanvas;

[SerializeField] private TextMeshProUGUI dayCounter;

    public bool canInteract { get; set; } = true;
    public string[] denyText { get; set; }

    private void Start()
    {
        SetDay();
    }

    public void Interact()
    {
        if (canInteract)
        {
            day++;
            SetDay();
            CheckGameState();
        }
    }

    private void CheckGameState()
    {
        if(day >= finalDay)
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

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void SetDay()
    {
        dayCounter.text = "Day #" + day.ToString();
    }

}
