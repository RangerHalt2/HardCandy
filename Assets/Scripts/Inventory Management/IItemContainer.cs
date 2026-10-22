using System;
using UnityEngine;

public interface IItemContainer
{
    int Size { get; }
    GameObject GetItem(int index);
    void SetItem(int index, GameObject item);     // must also raise SlotChanged
    bool CanAccept(GameObject item);
    event Action<int> SlotChanged;
}