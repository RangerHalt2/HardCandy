using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class ContainerUIController : MonoBehaviour
{
    [SerializeField] private RectTransform playerInventoryParent;
    [SerializeField] private RectTransform containerInventoryParent;
    [SerializeField] private InventorySlotUI slotPrefab;   // was GameObject

    private readonly List<InventorySlotUI> playerSlots = new List<InventorySlotUI>();
    private readonly List<InventorySlotUI> containerSlots = new List<InventorySlotUI>();

    private PlayerController pc;

    private CandyController c_controller;
    private bool hasCandyController = false;
    private ISellContainer sellContainer;
    private SaleContainer saleContainer;

    [SerializeField] private Button sellButton;

    // ItemStash implements IItemContainer, so existing callers passing an ItemStash still compile
    public void SpawnInventorySlots(IItemContainer container)
    {
        InteractionPrompt prompt = GameObject.FindAnyObjectByType<InteractionPrompt>();
        if (prompt != null)
        {
            prompt.canShowPrompt = false;
            prompt.TogglePrompt();
        }
        c_controller = null;
        hasCandyController = false;
        PlayerInventory playerInventory = FindAnyObjectByType<PlayerInventory>();
        pc = FindAnyObjectByType<PlayerController>();

        if (playerInventory == null)
        {
            Debug.LogError("CONTAINER UI CONTROLLER - PLAYER INVENTORY NOT FOUND; RETURNING");
            return;
        }
        if (pc == null)
        {
            Debug.LogError("CONTAINER UI CONTROLLER - the player controller could not be found; returning");
            return;
        }

        ClearSlots();   // guards against opening twice without closing

        sellButton.onClick.RemoveAllListeners();
        if (container is ISellContainer seller)
        {
            sellButton.gameObject.SetActive(true);
            sellButton.onClick.AddListener(seller.Sell);
        }
        else
        {
            sellButton.gameObject.SetActive(false);     // stashes and the workbench don't show it
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pc.blockInputs = true;

        if (container is Component c)
        {
            c_controller = c.GetComponentInParent<CandyController>();   // or GetComponent
            if (c_controller != null) hasCandyController = true;
        }
        sellContainer = container as ISellContainer;
        saleContainer = container as SaleContainer;

        BuildGrid(playerInventoryParent, playerInventory, playerSlots);
        BuildGrid(containerInventoryParent, container, containerSlots);
    }

    private void BuildGrid(RectTransform panel, IItemContainer container, List<InventorySlotUI> list)
    {
        RectTransform gridParent = panel.Find("Grid").GetComponent<RectTransform>();

        for (int i = 0; i < container.Size; i++)
        {
            InventorySlotUI slot = Instantiate(slotPrefab, gridParent);
            slot.Init(container, i);   // subscribes to SlotChanged and draws the current icon
            list.Add(slot);
        }

        Debug.Log($"CONTAINER UI CONTROLLER - {gridParent.parent.name}/Grid: {gridParent.childCount} children, container Size = {container.Size}");
        for (int i = 0; i < gridParent.childCount; i++)
            Debug.Log($"CONTAINER UI CONTROLLER -  child {i}: {gridParent.GetChild(i).name}");
    }

    private void ClearSlots()
    {
        foreach (var s in playerSlots) if (s != null) Destroy(s.gameObject);
        foreach (var s in containerSlots) if (s != null) Destroy(s.gameObject);
        playerSlots.Clear();
        containerSlots.Clear();
    }

    public void Exit()
    {
        ClearSlots();   // each slot unsubscribes from SlotChanged in OnDestroy

        sellButton.onClick.RemoveAllListeners();
        sellButton.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (pc != null) pc.blockInputs = false;

        if(c_controller != null && hasCandyController)
        {
            c_controller.Enter();
        }
        else
        {
            InteractionPrompt prompt = GameObject.FindAnyObjectByType<InteractionPrompt>();
            if (prompt != null)
            {
                prompt.canShowPrompt = true;
                prompt.lastCollider = null;
            }
        }

        if (saleContainer != null ) saleContainer.ReturnUnsoldItems();

        gameObject.SetActive(false);
    }
}