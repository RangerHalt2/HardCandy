using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CandyController : MonoBehaviour, IInteractable
{

    [SerializeField] private List<GameObject> ingredients;
    [SerializeField] private GameObject candyPrefab;
    [SerializeField] private Transform candyTransform;

    private Camera playerCamera;
    [SerializeField] private Camera stationCamera;

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
        Enter();
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

    [SerializeField] private TriggerRelay potTrigger;
    void OnEnable() => potTrigger.Entered += OnPotEntered;
    void OnDisable() => potTrigger.Entered -= OnPotEntered;
    void OnPotEntered(Collider other)
    {

        if (other.gameObject.CompareTag("Ingredient"))
        {
            ingredients.Add(other.gameObject);
            other.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        HandleIngredientPickUp();
        HandleCreateCandy();
    }

    #region Create Candy
    
    private void HandleCreateCandy()
    {
        if (ingredients.Count < 2) return;

        GameObject newCandy = Instantiate(candyPrefab, candyTransform.position, Quaternion.identity);
        Candy candyRef = newCandy.GetComponent<Candy>();
        candyRef.CandyName = DetermineCandyName();
        candyRef.traits = ApplyTraits();

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


    private void Enter()
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
        //pc.blockInputs = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    #region Ingredient PickUp
    private void HandleIngredientPickUp()
    {
        if (m_handler == null) return;

        if (m_handler.ClickTriggered && (held == null && heldRb == null)) TryPickUp();
        if (m_handler.ClickTriggered && (held != null && heldRb != null)) Drag();
        if (!m_handler.ClickTriggered) Drop();
    }

    private void TryPickUp()
    {
        //Debug.Log("CANDY CONTROLLER - Trying PickUp\nPointer Position: " + m_handler.PointerPosition);
        Ray ray = cam.ScreenPointToRay(m_handler.PointerPosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 100f, ingredientLayer, QueryTriggerInteraction.Ignore)) return;

        held = hit.transform;
        heldRb = held.GetComponent<Rigidbody>();
        if (heldRb != null)
        {
            heldRb.isKinematic = true;   // stop physics fighting the drag
        }

        // Plane perpendicular to the camera's forward, passing through the grab point
        dragPlane = new Plane(-cam.transform.forward, hit.point);

        // Keep the object from snapping its center to the cursor
        grabOffset = held.position - hit.point;

        //Debug.Log("CANDY CONTROLLER -\nheld is: " + held + "\nheldRb is: " + heldRb);
    }

    private void Drag()
    {
        //Debug.Log("CANDY CONTROLLER - trying to drag candy ingredient");
        Ray ray = cam.ScreenPointToRay(m_handler.PointerPosition);
        if (!dragPlane.Raycast(ray, out float distance)) return;

        Vector3 target = ray.GetPoint(distance) + grabOffset;

        target = dragZone.ClosestPoint(target);

        held.position = target;
    }

    void Drop()
    {
        //Debug.Log("CANDY CONTROLLER - try to drop");
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
