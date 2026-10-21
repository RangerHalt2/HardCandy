using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CandyController : MonoBehaviour
{

    [SerializeField] private List<GameObject> ingredients;
    [SerializeField] private GameObject candyPrefab;
    [SerializeField] private Transform candyTransform;

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

    private void Start()
    {
        m_handler = GameObject.FindAnyObjectByType<InputHandler>().GetComponent<InputHandler>();
        ingredientLayer = LayerMask.GetMask("Ingredient");

        if (cam == null)
        {
            cam = GameObject.FindAnyObjectByType<Camera>().GetComponent<Camera>();
        }
    }

    //The attached collider right now just triggers if an ingredient touches the collider that's set on the workstation, this will need to change when it becomes more complicated
    private void OnTriggerEnter(Collider other)
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
        ingredients.Clear();
    }


    #endregion


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
