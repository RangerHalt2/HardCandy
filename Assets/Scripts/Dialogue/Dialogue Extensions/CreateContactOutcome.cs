using UnityEngine;

public class CreateContactOutcome : MonoBehaviour, IDialogueOutcome
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
            Debug.Log("CREATE CONTACT OUTCOME - contact has been created");
            origin.isContact = true;
        }
    }
}
