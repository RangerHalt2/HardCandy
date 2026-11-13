using System;
using System.Collections.Generic;
using UnityEngine;

public class GuardManager : MonoBehaviour
{
    public static GuardManager Instance { get; private set; }
    
    [Serializable]
    public class ZoneAssignmentConfig
    {
        public PrisonZone zone;
        [Min(0)] public int guardsToSpawn = 2;
    }

    [Header("Global Settings")]
    [SerializeField] private GameObject guardPrefab;
    [SerializeField] private Transform guardContainer;

    [Header("Prison Area Deployments")]
    public List<ZoneAssignmentConfig> areaDeployments = new List<ZoneAssignmentConfig>();



    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        SpawnAllGuards();
    }

    public void SpawnAllGuards()
    {
        if (guardPrefab == null) return;
        if (guardContainer == null) guardContainer = new GameObject("Spawned_Guards").transform;

        foreach (var deployment in areaDeployments)
        {
            if (deployment.zone == null) continue;

            for (int i = 0; i < deployment.guardsToSpawn; i++)
            {
                Transform spawnPoint = deployment.zone.GetSpawnPoint(i);
                GameObject guardObj = Instantiate(guardPrefab, spawnPoint.position, spawnPoint.rotation, guardContainer);
                if (guardObj.TryGetComponent<GuardAI>(out var guardAI))
                {
                    guardAI.InitializeGuard(deployment.zone);
                }
            }
        }
    }

    public PrisonZone GetClosestSafeZone(Vector3 fromPosition)
    {
        PrisonZone closest = null;
        float minDistance = float.MaxValue;

        PrisonZone[] allZones = FindObjectsByType<PrisonZone>();

        foreach (PrisonZone z in allZones)
        {
            if (z != null && z.SecurityLevel != null && z.SecurityLevel.IsSafeZone)
            {
                float dist = Vector3.Distance(fromPosition, z.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = z;
                }
            }
        }
        return closest;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
