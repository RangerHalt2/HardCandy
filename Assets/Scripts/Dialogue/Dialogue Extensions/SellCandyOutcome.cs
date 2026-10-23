using UnityEngine;

public class SellCandyOutcome : MonoBehaviour, IDialogueOutcome
{
    [HideInInspector] public CandyBuyerNPC origin;

    private void Start()
    {
        origin = GetComponent<CandyBuyerNPC>();
    }

    public void Execute()
    {
        if (origin != null)
        {
            origin.SellCandy();
        }
    }
}
