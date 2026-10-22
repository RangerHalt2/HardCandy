using System;
using UnityEngine;

public class ItemStash : MonoBehaviour, IInteractable, IItemContainer
{
    [SerializeField] public GameObject[] inventory;
    [SerializeField] private int inventorySpace = 10;
    [SerializeField] private Transform[] inventorySlots;

    #region IItemContainer
    public event Action<int> SlotChanged;
    public int Size => inventory.Length;
    public GameObject GetItem(int i) => inventory[i];
    public bool CanAccept(GameObject item) => true;

    public void SetItem(int i, GameObject item)
    {
        inventory[i] = item;

        if (item != null)
        {
            item.SetActive(true);   // items arriving from the hotbar may be inactive

            Rigidbody rb = item.GetComponentInChildren<Rigidbody>();
            if (rb == null) rb = item.GetComponentInParent<Rigidbody>();
            CandyController c_controller = GetComponent<CandyController>();
            if (rb != null && c_controller == null) rb.isKinematic = false;

            item.transform.SetParent(inventorySlots[i], false);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
        }

        SlotChanged?.Invoke(i);
    }
    #endregion


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
        InventoryCheck();
    }

    private void InventoryCheck()
    {
        ContainerUIController containerController = GameObject.FindAnyObjectByType<ContainerUIController>(FindObjectsInactive.Include);
        if (containerController == null)
        {
            Debug.LogError("ITEM STASH - container ui controller not found and cannot be opened; returning.");
            return;
        }
        containerController.gameObject.SetActive(true);
        ItemStash itemStash = GetComponent<ItemStash>();
        containerController.SpawnInventorySlots(itemStash);
    }

    private void PlaceItemInFirstSlot(GameObject item)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null)
            {
                SetItem(i, item);
                return;
            }
        }
    }

    #endregion

}
