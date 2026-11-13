using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum GuardState
{
    Patrolling,
    Searching,
    Chasing
}

[RequireComponent(typeof(NavMeshAgent))]
public class GuardAI : MonoBehaviour, IInteractable
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float chaseSpeed = 5.5f;

    [Header("Patrol Settings")]
    [SerializeField] private float waypointTolerance = 1.0f;
    [SerializeField] private float waypointWaitTime = 2.0f;

    [Header("Search Settings")]
    [SerializeField] private float searchDuration = 5.0f;
    [SerializeField] private float searchTurnSpeed = 120f;

    [Header("Zone Config")]
    [SerializeField] private PrisonZone initialZone;
    private bool isInitialized = false;

    [Header("Chase & Testing Settings")]
    [SerializeField] private float catchDistance = 1.8f;
    [SerializeField] private bool chaseOnInteract = true;

    public bool canInteract { get; set; } = true;
    public string[] denyText { get; set; } = new string[] { "Move along." };

    // testing hooks

    public void Interact()
    {
        if (chaseOnInteract)
        {
            TestStartChase();
        }
        else
        {
            //Detected();
        }
    }

    [ContextMenu("Test: Start Chasing Player")]
    public void TestStartChase()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            Debug.Log($"[GuardAI] {name} started chasing {player.name}!");
            StartChasing(player.transform);
        }
        else
        {
            Debug.LogWarning("[GuardAI] No PlayerController found in scene to chase!");
        }
    }

    [ContextMenu("Test: Search Near Player (Simulates Rock Throw)")]
    public void TestSearchNearPlayer()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            Vector3 pos = player.transform.position;
            Debug.Log($"[GuardAI] {name} investigating coordinates ({pos.x}, {pos.y}, {pos.z})");
            StartSearching(pos.x, pos.y, pos.z);
        }
    }

    [ContextMenu("Test: Return to Patrol")]
    public void TestReturnToPatrol()
    {
        StartPatrolling();
    }




    private NavMeshAgent agent;

    public PrisonZone AssignedZone { get; private set; }
    public IReadOnlyList<Transform> Waypoints { get; private set; }

    public GuardState CurrentState { get; private set; } = GuardState.Patrolling;
    private int currentWaypointIndex = 0;
    private bool isWaiting = false;
    private Coroutine searchCoroutine;

    private Transform chaseTarget;

    public void InitializeGuard(PrisonZone zone)
    {
        isInitialized = true;
        AssignedZone = zone;
        Waypoints = zone.RoamWaypoints;

        agent.speed = walkSpeed;

        if (Waypoints != null && Waypoints.Count > 0)
        {
            currentWaypointIndex = Random.Range(0, Waypoints.Count);
            MoveToWaypoint();
        }
        else
        {
            agent.SetDestination(transform.position);
        }

        EnterPatrolState();
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!isInitialized && initialZone != null)
        {
            InitializeGuard(initialZone);
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentState)
        {
            case GuardState.Patrolling:
                UpdatePatrol();
                break;
            case GuardState.Chasing:
                UpdateChase();
                break;
            case GuardState.Searching:

                break;
        }
    }


    // patrolling state

    public void StartPatrolling()
    {
        EnterPatrolState();
    }

    private void EnterPatrolState()
    {
        //StopActiveSearch();
        CurrentState = GuardState.Patrolling;
        chaseTarget = null;
        agent.speed = walkSpeed;
        agent.isStopped = false;

        MoveToWaypoint();
    }

    private void UpdatePatrol()
    {
        if (Waypoints == null || Waypoints.Count == 0 || isWaiting) return;

        if (!agent.pathPending && agent.remainingDistance <= waypointTolerance)
        {
            StartCoroutine(WaitAtWaypointRoutine());
        }
    }

    private IEnumerator WaitAtWaypointRoutine()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waypointWaitTime);

        currentWaypointIndex = (currentWaypointIndex + 1) % Waypoints.Count;
        MoveToWaypoint();

        isWaiting = false;
    }

    private void MoveToWaypoint()
    {
        if (Waypoints != null && Waypoints.Count > 0 && Waypoints[currentWaypointIndex] != null)
        {
            agent.SetDestination(Waypoints[currentWaypointIndex].position);
        }
    }


    // chasing state

    public void StartChasing(Transform target)
    {
        StopActiveSearch();
        chaseTarget = target;
        CurrentState = GuardState.Chasing;
        agent.speed = chaseSpeed;
        agent.isStopped = false;
    }

    private void UpdateChase()
    {
        if (chaseTarget == null)
        {
            // lost the target, search around last position
            StartSearching(transform.position);
            return;
        }

        agent.SetDestination(chaseTarget.position);

        float distanceToTarget = Vector3.Distance(transform.position, chaseTarget.position);
        if (distanceToTarget <= catchDistance)
        {
            Debug.Log("[GuardAI] Guard caught the player!");
            EnterPatrolState();
            //Detected();
        }
    }


    // searching state

    public void StartSearching(Vector3 searchLocation)
    {
        StopActiveSearch();
        CurrentState = GuardState.Searching;
        chaseTarget = null;
        agent.speed = walkSpeed;

        searchCoroutine = StartCoroutine(SearchRoutine(searchLocation));
    }

    public void StartSearching(float x, float y, float z)
    {
        StartSearching(new Vector3(x, y, z));
    }

    private IEnumerator SearchRoutine(Vector3 targetLocation)
    {
        agent.isStopped = false;
        agent.SetDestination(targetLocation);

        while (agent.pathPending || agent.remainingDistance > waypointTolerance)
        {
            if (agent.pathStatus == NavMeshPathStatus.PathPartial || agent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                break;
            }
            yield return null;
        }

        // stop and look around;
        agent.isStopped = true;
        float elapsed = 0f;

        while (elapsed < searchDuration)
        {
            float lookAngle = Mathf.Sin(elapsed * 2f) * searchTurnSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, lookAngle);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // timer elapsed, return to patrolling
        EnterPatrolState();
    }

    private void StopActiveSearch()
    {
        if (searchCoroutine != null)
        {
            StopCoroutine(searchCoroutine);
            searchCoroutine = null;
        }
    }

}
