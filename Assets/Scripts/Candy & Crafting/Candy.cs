using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Candy : MonoBehaviour, IInteractable, IPickUp
{

    public string CandyName;
    public List<Traits_ScriptableObjects> traits;

    [SerializeField] private Sprite inventoryImg;
    public Sprite InventoryImg => inventoryImg;

    #region IInteractable Addressments
    public bool canInteract { get; set; } = true;

    public string[] denyText { get; set; }

    public void Interact()
    {
        if (!canInteract) return;
        PlayerInventory playerInventory = GameObject.FindAnyObjectByType<PlayerInventory>();

        if (playerInventory == null)
        {
            Debug.LogError("CANDY INGREDIENT - CRITICAL ERROR THE PLAYER INVENTORY IS NOT FOUND");
            return;
        }

        playerInventory.PickUpItem(gameObject);
    }
    #endregion



}
