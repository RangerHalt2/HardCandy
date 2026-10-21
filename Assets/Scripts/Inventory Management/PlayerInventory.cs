using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public GameObject[] inventory;
    private int selectedInventorySlot = 0;

    private GameObject itemInHand;

    private InputHandler inputs;
    private InventoryUIController UIInventoryController;

    [SerializeField] private Transform hand;

    private float hotbarScrollTimer = 0f;
    [SerializeField] private float hotbarScrollCooldown = 0.5f;

    private void Awake()
    {
        inventory = new GameObject[9];
        inputs = GameObject.FindAnyObjectByType<InputHandler>();
        UIInventoryController = GameObject.FindAnyObjectByType<InventoryUIController>();
    }

    private void Update()
    {
        HandleHotbarScrolling();
        HandleTimers();
    }

    //InventoryIsFull returns true if it's full and false otherwise
    private bool InventoryIsFull()
    {
        bool isFull = true; //base assumption is the inventory is full
        foreach (GameObject slot in inventory)
        {
            if (slot == null) isFull = false; //Inventory slot found
        }
        return isFull; //if the value is true this will change to false, 
    }

    #region Picking Up an Item

    public void PickUpItem(GameObject item)
    {
        if (InventoryIsFull())
        {
            Debug.Log("PLAYER INVENTORY - Inventory is full; preventing pick-up");
            return;
        }

        if(UIInventoryController == null)
        {
            Debug.LogError("PLAYER INVENTORY - UI INVENTORY COULD NOT BE FOUND; CRITICAL ERROR; RETURNING.");
            return;
        }

        if (item == null)
        {
            Debug.LogWarning("PLAYER INVENTORY - Item is null; returning.");
            return;
        }

        PlaceItemInFirstSlot(item);

        Rigidbody rb = item.GetComponentInChildren<Rigidbody>();
        if (rb == null) rb = item.GetComponentInParent<Rigidbody>();
        rb.isKinematic = true;
        item.transform.position = hand.position;
        item.transform.rotation = hand.rotation;
        item.transform.SetParent(hand);

        IPickUp pickedItem = item.GetComponent<IPickUp>();
        if(pickedItem == null)
        {
            Debug.LogWarning("PLAYER INVENTORY - you tried to pick up an item that is not supposed to be able to be picked up or otherwise is missing requirements to be picked up");
            return;
        }
        if(pickedItem.InventoryImg == null)
        {
            Debug.LogWarning("PLAYER INVENTORY - the item picked up does not have a listed sprite attached");
            return;
        }
        Debug.Log("PLAYER INVENTORY - updating the sprite of: " + selectedInventorySlot + " on the hotbar");
        UIInventoryController.SetSlotSprite(selectedInventorySlot, pickedItem.InventoryImg);

    }

    private void PlaceItemInFirstSlot(GameObject item)
    {
        for(int i = 0; i < inventory.Length; i++)
        {
            GameObject slot = inventory[i];
            if (slot == null)
            {
                inventory[i] = item;
                selectedInventorySlot = i; // automatically swap to the placed item
                Debug.Log("PLAYER INVENTORY - found slot was: " + i);
                SwapHeldItem();
                return;
            }
        }
    }

    #endregion

    #region HotBar Handling
    void OnEnable() => InputHandler.Instance.OnHotbarSelected += SelectSlot;
    void OnDisable() => InputHandler.Instance.OnHotbarSelected -= SelectSlot;
    void SelectSlot(int index) { selectedInventorySlot = index; SwapHeldItem(); }
    private void HandleHotbarScrolling()
    {
        if (inputs == null)
        {
            Debug.LogWarning("PLAYERINVENTORY - INPUTS NOT FOUND");
            return;
        }

        if (inputs.ScrollValue != 0 && hotbarScrollTimer <= 0f)
        {
            hotbarScrollTimer = hotbarScrollCooldown;

            int scrollDirection = Math.Sign(inputs.ScrollValue);
            if (scrollDirection == 0) return;

            selectedInventorySlot = (selectedInventorySlot - scrollDirection + 9) % 9;
            SwapHeldItem();
        }
    }

    private void SwapHeldItem()
    {
        if(itemInHand != null)
        {
            itemInHand.SetActive(false);
        }

        if (inventory[selectedInventorySlot] != null)
        {
            inventory[selectedInventorySlot].SetActive(true);
            itemInHand = inventory[selectedInventorySlot];
        }
        UIInventoryController.SelectSlot(selectedInventorySlot);
    }

    

    #endregion


    private void HandleTimers()
    {
        hotbarScrollTimer -= Time.deltaTime;
    }
}
