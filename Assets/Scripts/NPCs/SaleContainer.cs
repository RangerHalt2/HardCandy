using System;
using UnityEngine;

public class SaleContainer : MonoBehaviour, IItemContainer, ISellContainer
{
    [SerializeField] private int size = 10;
    [SerializeField] private Transform storage;      // empty child of the NPC to park items under

    private GameObject[] items;
    private CandyBuyerNPC buyer;
    private PlayerMoney money;

    public event Action<int> SlotChanged;
    public int Size => size;

    void Awake()
    {
        items = new GameObject[size];
        buyer = GetComponent<CandyBuyerNPC>();
    }

    void Start() => money = FindAnyObjectByType<PlayerMoney>();

    public GameObject GetItem(int i) => items[i];

    public bool CanAccept(GameObject item) =>
        item != null && item.GetComponent<Candy>() != null;      // only candy goes on the counter

    public void SetItem(int i, GameObject item)
    {
        items[i] = item;
        if (item != null)
        {
            ItemPhysics.SetKinematic(item, true);
            item.transform.SetParent(storage != null ? storage : transform, false);
            item.transform.localPosition = Vector3.zero;
            item.SetActive(false);                               // stored, not visible in the world
        }
        SlotChanged?.Invoke(i);
    }

    public void Sell()
    {
        int total = 0;
        for (int i = 0; i < items.Length; i++)
        {
            GameObject obj = items[i];
            if (obj == null) continue;

            Candy candy = obj.GetComponent<Candy>();
            if (candy == null || buyer.Refuses(candy)) continue; // refused candy stays in the slot

            total += buyer.GetOffer(candy);
            SetItem(i, null);                                    // clears slot and refreshes the UI
            Destroy(obj);
        }

        if (money != null) money.CurrentMoney += total;
        Debug.Log($"SALE CONTAINER - sold for {total}");
    }

    // Runs from ContainerUIController.Exit(): give back anything that wasn't sold
    public void ReturnUnsoldItems()
    {
        PlayerInventory inv = FindAnyObjectByType<PlayerInventory>();

        for (int i = 0; i < items.Length; i++)
        {
            GameObject obj = items[i];
            if (obj == null) continue;

            SetItem(i, null);

            bool hasRoom = inv != null && Array.Exists(inv.inventory, s => s == null);
            if (hasRoom)
            {
                inv.PickUpItem(obj);
            }
            else   // inventory full: drop it by the NPC instead of losing it
            {
                obj.transform.SetParent(null);
                obj.SetActive(true);
                ItemPhysics.SetKinematic(obj, false);
                obj.transform.position = transform.position + Vector3.up + transform.forward;
            }
        }
    }
}