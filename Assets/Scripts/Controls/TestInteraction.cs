using UnityEngine;

public class InteractionTest : MonoBehaviour, IInteractable
{
    [SerializeField] private bool _canInteract = true;
    [SerializeField] private string[] _denyText = new string[] { "Interacted with." };

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
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Random.ColorHSV();
        }

        Debug.Log($"Successfully interacted with: {gameObject.name}");
    }
}
