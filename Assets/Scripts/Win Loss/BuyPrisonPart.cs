using UnityEngine;

//Put on an object in the world, interacting with it buys the part
public class BuyPrisonPart : MonoBehaviour, IInteractable
{
    [SerializeField] private PrisonPart_ScriptableObjects part;
    [SerializeField] private bool _canInteract = true;
    [SerializeField] private string[] _denyText = new string[] { "Already owned." };

    public bool canInteract
    {
        get => _canInteract && PrisonOwnership.Instance != null && !PrisonOwnership.Instance.Owns(part);
        set => _canInteract = value;
    }

    public string[] denyText
    {
        get => _denyText;
        set => _denyText = value;
    }

    public void Interact()
    {
        if (!canInteract) return;
        PrisonOwnership.Instance.TryBuy(part);
    }
}
