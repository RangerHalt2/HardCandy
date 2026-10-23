using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.UI.Image;

public class CandyController : MonoBehaviour, IInteractable
{

    [SerializeField] private List<GameObject> ingredients;
    [SerializeField] private GameObject candyPrefab;
    [SerializeField] private Transform candyTransform;

    private Camera playerCamera;
    [SerializeField] private Camera stationCamera;

    [SerializeField] private bool isCrafting = false;

    private Camera cam;
    private LayerMask ingredientLayer;

    private InputHandler m_handler;

    //LB: work stations should be prefabbed objects and this dragZone in the end will need to be on the prefab and then will be assigned in the prefab
    //LB: if multiple work stations are added in one scene, it would be easy to set up the game to update WHICH workstation they're at and get a reference to the proper drag zone
    [SerializeField] private BoxCollider dragZone;

    private Transform held;
    private Rigidbody heldRb;
    private Plane dragPlane;
    private Vector3 grabOffset;

    #region IInteractable Addressments
    public bool canInteract { get; set; } = true;

    public string[] denyText { get; set; }

    public void Interact()
    {
        if (!canInteract) return;
        //Enter();
        InventoryCheck();
    }
    #endregion

    private void Start()
    {
        m_handler = GameObject.FindAnyObjectByType<InputHandler>().GetComponent<InputHandler>();
        ingredientLayer = LayerMask.GetMask("Ingredient");

        if (cam == null)
        {
            cam = GameObject.FindAnyObjectByType<Camera>().GetComponent<Camera>();
        }
    }

    private void Update()
    {
        HandleIngredientPickUp();
        HandleCreateCandy();
        HandleEarlyExit();
    }

    #region Add Ingredients

    #endregion

    #region Pot Collision Handling
    [SerializeField] private TriggerRelay potTrigger;
    void OnEnable() => potTrigger.Entered += OnPotEntered;
    void OnDisable() => potTrigger.Entered -= OnPotEntered;
    void OnPotEntered(Collider other)
    {
        if (!isCrafting) return;
        if (other.gameObject.CompareTag("Ingredient"))
        {
            ingredients.Add(other.gameObject);
            other.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Create Candy

    private void HandleCreateCandy()
    {
        if (ingredients.Count < 2) return;

        GameObject newCandy = Instantiate(candyPrefab, candyTransform.position, Quaternion.identity);
        Candy candyRef = newCandy.GetComponent<Candy>();
        candyRef.CandyName = DetermineCandyName();
        candyRef.traits = ApplyTraits();

        ItemStash stashedInventory = GetComponent<ItemStash>();
        for (int i = 0; i < stashedInventory.inventory.Length; i++)
        {
            stashedInventory.inventory[i] = null;
        }

        ingredients.Clear();
        Exit();
    }

    private List<Traits_ScriptableObjects> ApplyTraits()
    {
        List<Traits_ScriptableObjects> ret = new List<Traits_ScriptableObjects>();
        foreach (GameObject ingredient in ingredients)
        {
            CandyIngredient candyIngredient = ingredient.GetComponent<CandyIngredient>();
            CandyItem_ScriptableObjects candyIngredientScriptable = candyIngredient.candyIngredient;

            foreach (Traits_ScriptableObjects trait in candyIngredientScriptable.IngredientTraits)
            {
                if (trait != null)
                {
                    ret.Add(trait);
                }
            }
        }
        return ret;
    }

    private string DetermineCandyName()
    {
        string ret = "";

        foreach (GameObject ingredient in ingredients)
        {
            CandyIngredient candyIngredient = ingredient.GetComponent<CandyIngredient>();
            CandyItem_ScriptableObjects candyIngredientScriptable = candyIngredient.candyIngredient;
            
            foreach(Traits_ScriptableObjects trait in candyIngredientScriptable.IngredientTraits)
            {
                if (trait != null)
                {
                    ret += trait.TraitNameMod;
                    ret += " ";
                }
            }
        }
        ret += "Candy";

        return ret;
    }

    #endregion

    #region Enter/Exit Workbench Logic
    
    private void InventoryCheck()
    {
        ContainerUIController containerController = GameObject.FindAnyObjectByType<ContainerUIController>(FindObjectsInactive.Include);
        if (containerController == null)
        {
            Debug.LogError("CANDY CONTROLLER - container ui controller not found and cannot be opened; returning.");
            return;
        }
        containerController.gameObject.SetActive(true);
        ItemStash itemStash = GetComponent<ItemStash>();
        containerController.SpawnInventorySlots(itemStash);
    }
    
    public void Enter()
    {
        playerCamera = Camera.main;
        playerCamera.enabled = false;
        stationCamera.enabled = true;

        //Tell the player controller to not be in use
        PlayerController pc = GameObject.FindAnyObjectByType<PlayerController>();
        if (pc == null)
        {
            Debug.LogError("CANDY CONTROLLER - THE PLAYER CONTROLLER COULD NOT BE FOUND");
            return;
        }
        pc.blockInputs = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isCrafting = true;
    }

    private void Exit()
    {
        stationCamera.enabled = false;
        playerCamera.enabled = true;

        PlayerController pc = GameObject.FindAnyObjectByType<PlayerController>();
        if (pc == null)
        {
            Debug.LogError("CANDY CONTROLLER - THE PLAYER CONTROLLER COULD NOT BE FOUND");
            return;
        }
        pc.blockInputs = false;
        isCrafting = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        InteractionPrompt prompt = GameObject.FindAnyObjectByType<InteractionPrompt>();
        if (prompt != null)
        {
            prompt.canShowPrompt = true;
            prompt.lastCollider = null;
        }
    }

    private void HandleEarlyExit()
    {
        if(m_handler != null && m_handler.EscapeTriggered && isCrafting)
        {
            Exit();
        }
    }
    #endregion

    #region Ingredient PickUp
    private void HandleIngredientPickUp()
    {
        if (m_handler == null || !isCrafting) return;

        if (m_handler.ClickTriggered && (held == null && heldRb == null)) TryPickUp();
        if (m_handler.ClickTriggered && (held != null && heldRb != null)) Drag();
        if (!m_handler.ClickTriggered) Drop();
    }

    private void TryPickUp()
    {
        Debug.Log("CANDY CONTROLLER - Trying PickUp\nPointer Position: " + m_handler.PointerPosition);
        Ray ray = stationCamera.ScreenPointToRay(m_handler.PointerPosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 100f, ingredientLayer, QueryTriggerInteraction.Ignore)) return;
        Debug.DrawLine(ray.origin, hit.point, Color.red);
        held = hit.transform;
        heldRb = held.GetComponent<Rigidbody>();
        if (heldRb != null)
        {
            heldRb.isKinematic = true;   // stop physics fighting the drag
        }

        // Plane perpendicular to the camera's forward, passing through the grab point
        dragPlane = new Plane(-stationCamera.transform.forward, hit.point);

        // Keep the object from snapping its center to the cursor
        grabOffset = held.position - hit.point;

        Debug.Log("CANDY CONTROLLER -\nheld is: " + held + "\nheldRb is: " + heldRb);
    }

    private void Drag()
    {
        Debug.Log("CANDY CONTROLLER - trying to drag candy ingredient");
        Ray ray = stationCamera.ScreenPointToRay(m_handler.PointerPosition);
        if (!dragPlane.Raycast(ray, out float distance)) return;

        Vector3 target = ray.GetPoint(distance) + grabOffset;

        target = dragZone.ClosestPoint(target);

        held.position = target;
    }

    void Drop()
    {
        Debug.Log("CANDY CONTROLLER - try to drop");
        if (heldRb != null)
        {
            heldRb.isKinematic = false;  // let it fall onto the table
            heldRb.linearVelocity = Vector3.zero;
        }
        held = null;
        heldRb = null;
    }
    #endregion
}
