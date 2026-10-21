using UnityEngine;
using static UnityEditor.Progress;

public class ItemStash : MonoBehaviour, IInteractable
{
    [SerializeField] public GameObject[] inventory;
    [SerializeField] private int inventorySpace = 10;

    [SerializeField] private Transform[] inventorySlots;

    private void Awake()
    {
        inventory = new GameObject[inventorySpace];
    }

    private bool InventoryIsFull()
    {
        bool isFull = true; //base assumption is the inventory is full
        foreach (GameObject slot in inventory)
        {
            if (slot == null) isFull = false; //Inventory slot found
        }
        return isFull; //if the value is true this will change to false, 
    }

    #region Interactable Addressments
    public bool canInteract { get; set; } = true;

    public string[] denyText { get; set; }

    public void Interact()
    {
        if (!canInteract) return;

        if (InventoryIsFull())
        {
            Debug.Log("ITEM STASH - inventory is full cannot place item in here");
            return;
        }

        PlayerInventory playerInventory = GameObject.FindAnyObjectByType<PlayerInventory>();
        int slot = playerInventory.selectedInventorySlot;

        if (playerInventory.inventory[slot] != null) //If the player isn't holding anything nothing should happen
        {
            PlaceItemInFirstSlot((playerInventory.inventory[slot]));
            playerInventory.RemoveItemFromSlot(slot);
        }
    }

    private void PlaceItemInFirstSlot(GameObject item)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            GameObject slot = inventory[i];
            if (slot == null)
            {
                inventory[i] = item;
                Rigidbody rb = item.GetComponentInChildren<Rigidbody>();
                if (rb == null) rb = item.GetComponentInParent<Rigidbody>();
                rb.isKinematic = false;
                item.transform.position = inventorySlots[i].position;
                item.transform.rotation = inventorySlots[i].rotation;
                item.transform.SetParent(inventorySlots[i], true);
                return;
            }
        }
    }

    #endregion

}
