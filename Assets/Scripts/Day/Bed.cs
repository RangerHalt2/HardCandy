using UnityEngine;

public class Bed : MonoBehaviour, IInteractable
{
    [SerializeField] private bool _canInteract = true;
    [SerializeField] private string[] _denyText = new string[] { "You can't sleep right now." };

    public bool canInteract
    {
        get => _canInteract;
        set => _canInteract = value;
    }

    public string[] denyText
    {
        get => _denyText;
        set => _denyText = value;
    }

    public void Interact()
    {
        if (!canInteract || DayManager.Instance == null) return;
        DayManager.Instance.NextDay();
    }
}
