using UnityEngine;

public static class InventoryTransfer
{
    public static void Move(InventorySlotUI from, InventorySlotUI to)
    {
        GameObject a = from.Container.GetItem(from.Index);
        GameObject b = to.Container.GetItem(to.Index);
        if (a == null) return;

        if (!to.Container.CanAccept(a)) return;
        if (b != null && !from.Container.CanAccept(b)) return;   // swap must be legal both ways

        from.Container.SetItem(from.Index, b);   // b may be null (plain move)
        to.Container.SetItem(to.Index, a);
    }
}