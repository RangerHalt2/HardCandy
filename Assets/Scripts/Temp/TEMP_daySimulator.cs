using UnityEngine;

//Bed that skips to the next day. Win/loss now lives in WinLossManager
public class TEMP_daySimulator : MonoBehaviour, IInteractable
{
    public bool canInteract { get; set; } = true;
    public string[] denyText { get; set; }

    public void Interact()
    {
        if (canInteract && DayManager.Instance != null)
        {
            DayManager.Instance.NextDay();
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
