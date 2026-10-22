using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image icon;

    public IItemContainer Container { get; private set; }
    public int Index { get; private set; }

    public void Init(IItemContainer container, int index)
    {
        Container = container;
        Index = index;
        Container.SlotChanged += OnSlotChanged;
        Refresh();
    }

    void OnDestroy()
    {
        if (Container != null) Container.SlotChanged -= OnSlotChanged;
    }

    void OnSlotChanged(int changed) { Debug.Log($"[{name}] SlotChanged({changed}), my Index = {Index}"); if (changed == Index) Refresh(); }

    public void Refresh()
    {
        GameObject item = Container.GetItem(Index);
        IPickUp pickUp = item != null ? item.GetComponent<IPickUp>() : null;

        icon.sprite = pickUp?.InventoryImg;
        Debug.Log($"[{name}] Refresh idx {Index}: item={(item ? item.name : "null")}, sprite={(icon.sprite ? icon.sprite.name : "null")}");
        icon.enabled = icon.sprite != null;
        icon.color = Color.white;
    }

    public void OnBeginDrag(PointerEventData e)
    {
        if (!icon.enabled) return;                 // empty slot, nothing to drag
        DragGhost.Instance.Show(icon.sprite);
        icon.color = new Color(1, 1, 1, 0.4f);     // dim the original
    }

    public void OnDrag(PointerEventData e) => DragGhost.Instance.Follow(e.position);

    public void OnEndDrag(PointerEventData e)
    {
        DragGhost.Instance.Hide();
        Refresh();                                  // restores the dimmed icon
    }

    public void OnDrop(PointerEventData e)
    {
        Debug.Log($"Drop on {name}, idx {Index}");
        Refresh();
        if (e.pointerDrag == null) return;
        var from = e.pointerDrag.GetComponent<InventorySlotUI>();
        if (from == null || from == this) return;
        InventoryTransfer.Move(from, this);
        
        if(from.Container is PlayerInventory pi)
        {
            pi.RemoveItemFromSlot(from.Index);
        }

    }
}