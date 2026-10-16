using UnityEngine;

//Spawns one NPC at each spawn point and randomly gives them a name, a taste, and whether they buy candy
public class NPCSpawner : MonoBehaviour
{
    [SerializeField] private CandyBuyerNPC npcPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Random Setup")]
    [SerializeField] private string[] possibleNames = { "Gus", "Marge", "Tony", "Dot", "Lou" };
    [SerializeField] private NPCTaste_ScriptableObjects[] possibleTastes;

    [Tooltip("0 = nobody buys candy, 1 = everyone does")]
    [Range(0f, 1f)]
    [SerializeField] private float candyBuyerChance = 0.8f;

    private void Start()
    {
        SpawnAll();
    }

    public void SpawnAll()
    {
        if (npcPrefab == null) return;

        foreach (Transform point in spawnPoints)
        {
            SpawnNPC(point);
        }
    }

    private CandyBuyerNPC SpawnNPC(Transform point)
    {
        CandyBuyerNPC npc = Instantiate(npcPrefab, point.position, point.rotation);

        if (possibleNames.Length > 0)
        {
            npc.npcName = possibleNames[Random.Range(0, possibleNames.Length)];
        }

        if (possibleTastes.Length > 0)
        {
            npc.taste = possibleTastes[Random.Range(0, possibleTastes.Length)];
        }

        npc.isCandyBuyer = Random.value < candyBuyerChance;
        npc.isContact = false;

        return npc;
    }
}
