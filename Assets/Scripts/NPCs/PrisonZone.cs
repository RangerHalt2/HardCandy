using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PrisonZone : MonoBehaviour
{

    public static PrisonZone CurrentPlayerZone { get; private set; }

    [Header("Zone Configuration")]
    [SerializeField] private string zoneName = "Block A";
    [SerializeField] private SecurityLevel securityLevel;

    [Header("Patrol & Spawn Data")]
    [Tooltip("Spawn locations within the zone")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [Tooltip("Roaming waypoints")]
    [SerializeField] private List<Transform> roamWaypoints = new List<Transform>();

    public string ZoneName => zoneName;
    public SecurityLevel SecurityLevel => securityLevel;
    public IReadOnlyList<Transform> SpawnPoints => spawnPoints;
    public IReadOnlyList<Transform> RoamWaypoints => roamWaypoints;

    public Transform GetSpawnPoint(int index)
    {
        if (spawnPoints != null && spawnPoints.Count > 0)
        {
            return spawnPoints[index % spawnPoints.Count];
        }
        return transform; // default to zone root
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            CurrentPlayerZone = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null && CurrentPlayerZone == this)
        {
            CurrentPlayerZone = null;
        }
    }


    private void OnDrawGizmos()
    {
        // Visual indicator in the Scene view for your whitebox layout
        Gizmos.color = (securityLevel != null) ? securityLevel.DebugZoneColor : Color.gray;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 1.5f);

        if (roamWaypoints == null) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < roamWaypoints.Count; i++)
        {
            if (roamWaypoints[i] != null)
            {
                Gizmos.DrawSphere(roamWaypoints[i].position, 0.3f);
                if (i < roamWaypoints.Count - 1 && roamWaypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(roamWaypoints[i].position, roamWaypoints[i + 1].position);
                }
            }
        }
    }
}
