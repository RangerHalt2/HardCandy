using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{

    [SerializeField] private List<GameObject> slots;


    public void SelectSlot(int slot)
    {
        if (slot < 0 || slot >= slots.Count)
        {
            Debug.LogWarning("INVENTORY UI CONTROLLER - slot number " + slot + " is greater than the total amount of slots or less than 0; returning");
            return;
        }

        DeselectAllSlots();

        Transform slotTransform = slots[slot].transform.Find("Selection");
        if (slotTransform == null)
        {
            Debug.LogError("INVENTORY UI CONTROLLER - Slot Transform could not be found for slot: " + slot + 1);
            return;
        }

        slotTransform.gameObject.SetActive(true);
    }

    private void DeselectAllSlots()
    {
        int index = 0;
        foreach (GameObject slot in slots)
        {
            Transform slotTransform = slots[index].transform.Find("Selection");
            if (slotTransform == null)
            {
                Debug.LogError("INVENTORY UI CONTROLLER - Slot Transform could not be found for slot: " + slot + 1);
                continue;
            }

            slotTransform.gameObject.SetActive(false);
            index++;
        }
    }

    public void SetSlotSprite(int slot, Sprite sprite)
    {
        if(sprite == null)
        {
            Debug.LogWarning("INVENTORY UI CONTROLLER - Sprite is null; Returning and cancelling SetSlotSprite");
            return;
        }

        if (slot < 0 || slot >= slots.Count)
        {
            Debug.LogWarning("INVENTORY UI CONTROLLER - slot number " + slot + " is greater than the total amount of slots or less than 0; returning");
            return;
        }

        Transform slotTransform = slots[slot].transform.Find("SlotImage");
        if (slotTransform == null)
        {
            Debug.LogError("INVENTORY UI CONTROLLER - SlotImage not found for slot: " + slot+1);
            return;
        }

        Image slotImage = slotTransform.GetComponent<Image>();
        slotImage.sprite = sprite;
        slotImage.gameObject.SetActive(true);
    }

    public void RemoveSlotItem(int slot) 
    {
        if (slot < 0 || slot >= slots.Count)
        {
            Debug.LogWarning("INVENTORY UI CONTROLLER - slot number " + slot + " is greater than the total amount of slots or less than 0; returning");
            return;
        }

        Transform slotTransform = slots[slot].transform.Find("SlotImage");
        if (slotTransform == null)
        {
            Debug.LogError("INVENTORY UI CONTROLLER - SlotImage not found for slot: " + slot);
            return;
        }

        Image slotImage = slotTransform.GetComponent<Image>();
        slotImage.sprite = null;
        slotImage.gameObject.SetActive(false);
    }

}
