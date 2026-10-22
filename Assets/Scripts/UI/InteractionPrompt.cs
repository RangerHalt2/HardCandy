using UnityEngine;

public class InteractionPrompt : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private float range = 3f;
    [SerializeField] private LayerMask mask;               // walls + interactables, NOT the player
    private GameObject promptUI;
    [SerializeField] private float checkInterval = 0.05f;  // 20 checks per second

    private Collider lastCollider;
    private IInteractable current;
    private bool shown;
    private float timer;

    private void Start()
    {
        cam = Camera.main;
        promptUI = GameObject.Find("InteractText");
        if(promptUI == null)
        {
            Debug.LogError("INTERACTION PROMPT - could not find interact prompt");
            return;
        }
        promptUI.SetActive(false);
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;
        timer = checkInterval;

        Check();
    }

    void Check()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, range, mask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider != lastCollider)           // only look up the component when the target changes
            {
                lastCollider = hit.collider;
                current = hit.collider.GetComponentInParent<IInteractable>();
            }
        }
        else
        {
            lastCollider = null;
            current = null;
        }

        bool shouldShow = current != null && current.canInteract;
        if (shouldShow != shown)                        // only touch the UI when the state flips
        {
            shown = shouldShow;
            promptUI.SetActive(shown);
        }
    }

    void OnDisable()
    {
        shown = false;
        lastCollider = null;
        current = null;
        if (promptUI != null) promptUI.SetActive(false);
    }
}