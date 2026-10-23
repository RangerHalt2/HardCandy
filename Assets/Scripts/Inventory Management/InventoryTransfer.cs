using UnityEngine;

public static class InventoryTransfer
{
    public static bool Move(InventorySlotUI from, InventorySlotUI to)
    {
        Debug.Log("INVENTORY TRANSFER - Move called");
        GameObject a = from.Container.GetItem(from.Index);
        GameObject b = to.Container.GetItem(to.Index);
        if (a == null) return false;

        if (!to.Container.CanAccept(a)) return false;
        if (b != null && !from.Container.CanAccept(b)) return false;   // swap must be legal both ways

        from.Container.SetItem(from.Index, b);   // b may be null (plain move)
        to.Container.SetItem(to.Index, a);
        return true;
    }
}