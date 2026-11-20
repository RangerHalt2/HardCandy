using System.Collections.Generic;
using UnityEngine;

//Holds every part of the prison and which ones the player owns
public class PrisonOwnership : MonoBehaviour
{
    public static PrisonOwnership Instance { get; private set; }

    [Tooltip("Every part of the prison that exists")]
    public PrisonPart_ScriptableObjects[] allParts;

    [Tooltip("Parts the player owns. Add some here to start the game owning them")]
    [SerializeField] private List<PrisonPart_ScriptableObjects> ownedParts = new List<PrisonPart_ScriptableObjects>();

    private void Awake()
    {
        Instance = this;
    }

    public bool Owns(PrisonPart_ScriptableObjects part)
    {
        return ownedParts.Contains(part);
    }

    public bool TryBuy(PrisonPart_ScriptableObjects part)
    {
        if (part == null || Owns(part)) return false;

        PlayerMoney pm = FindAnyObjectByType<PlayerMoney>();
        if (pm == null || pm.CurrentMoney < part.Cost)
        {
            Debug.Log("PRISON OWNERSHIP - can't afford " + part.PartName);
            return false;
        }

        pm.CurrentMoney -= part.Cost;
        ownedParts.Add(part);
        Debug.Log("PRISON OWNERSHIP - bought " + part.PartName);

        if (WinLossManager.Instance != null) WinLossManager.Instance.CheckWin();
        return true;
    }
}
