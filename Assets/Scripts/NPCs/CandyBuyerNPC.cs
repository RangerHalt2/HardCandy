using System.Collections;
using UnityEngine;

//NPCs start as strangers. Once the player meets them they become a contact, and after that they'll buy candy from the player
//Needs a trigger collider on the NPC so the player (tagged "Player") and candy can be detected
public class CandyBuyerNPC : MonoBehaviour, IInteractable
{
    [Header("NPC Info")]
    public string npcName = "Stranger";

    [Tooltip("Not every NPC buys candy, turn this off for regular townsfolk")]
    public bool isCandyBuyer = true;

    [Tooltip("Starts false, the NPC doesn't know the player yet")]
    public bool isContact = false;

    [Header("Taste")]
    public NPCTaste_ScriptableObjects taste;

    [Header("Prices")]
    [SerializeField] private int basePrice = 5;
    [SerializeField] private int favoriteBonus = 5;


    [Header("Dialogue Related")]
    private DialogueTrigger dialogueTriggerer;
    private IDialogueOutcome dialogueOutcome;
    [SerializeField] private TextAsset notBuyingCandyJSON;
    [SerializeField] private TextAsset isntContactJSON;
    [SerializeField] private TextAsset buyingCandyJSON;

    private ContainerUIController containerUI;
    private SaleContainer saleContainer;

    private PlayerMoney playerMoney;

    private void Start()
    {
        playerMoney = FindAnyObjectByType<PlayerMoney>();
        dialogueTriggerer = GetComponent<DialogueTrigger>();

        saleContainer = GetComponent<SaleContainer>();
        if (containerUI == null)
            containerUI = FindAnyObjectByType<ContainerUIController>(FindObjectsInactive.Include);
    }

    private void OnTriggerEnter(Collider other)
    {
        /*
        if (other.CompareTag("Player"))
        {
            MeetPlayer();
            return;
        }

        //If a candy gets dropped/thrown onto the NPC, try to sell it to them
        Candy candy = other.GetComponent<Candy>();
        if (candy != null)
        {
            TrySellCandy(candy);
        }
        */
    }

    #region Interactable
    public bool canInteract { get; set; } = true;

    public string[] denyText { get; set; }

    public void Interact()
    {
        if (!canInteract) return;
        TalkToNPC();
    }
    #endregion

    #region Contact

    public void MeetPlayer()
    {
        if (!isCandyBuyer)
        {
            Say("Hey there. Nice day, huh?");
            return;
        }

        if (isContact)
        {
            Say("Good to see you again. Got any candy for me?");
            return;
        }

        //This is where an unlock condition could go later (quest, free sample, reputation, etc). For now meeting them is enough
        isContact = true;
        Say("Candy, huh? Alright, I'm in. Come find me when you've got some.");
    }

    #endregion

    #region Buying

    //Obsolete function for roll back in case.
    /*public bool TrySellCandy(Candy candy)
    {
        if (candy == null) return false;

        if (!isCandyBuyer)
        {
            Say("I don't buy candy, sorry.");
            return false;
        }

        if (!isContact)
        {
            Say("Do I know you?");
            return false;
        }

        if (Refuses(candy))
        {
            Say("Ew, no way. I'm not buying that.");
            return false;
        }

        int price = basePrice;
        if (IsFavorite(candy))
        {
            price += favoriteBonus;
            Say("Oh, my favorite! Here's a little extra.");
        }
        else
        {
            Say("Sure, I'll take it.");
        }

        if (playerMoney != null)
        {
            playerMoney.CurrentMoney += price;
        }

        PlayerInventory playerInventory = GameObject.FindAnyObjectByType<PlayerInventory>();
        if (playerInventory != null)
        {
            int index = 0;
            foreach(GameObject item in playerInventory.inventory)
            {
                if (item == null) continue; //no item to check
                Candy slotCandy = item.GetComponent<Candy>();
                if(slotCandy == candy) //Items are the same
                {
                    playerInventory.RemoveItemFromSlot(index);
                }
                index++;
            }
        }
        Destroy(candy.gameObject);
        return true;
    }
    */

    private void TalkToNPC()
    {
        if (!isCandyBuyer) dialogueTriggerer.SetDialogueJSON(notBuyingCandyJSON);
        else if (!isContact) { dialogueTriggerer.SetDialogueJSON(isntContactJSON); dialogueOutcome = GetComponent<CreateContactOutcome>(); }
        else { dialogueTriggerer.SetDialogueJSON(buyingCandyJSON); dialogueOutcome = GetComponent<SellCandyOutcome>();  }

        Debug.Log("CANDY BUYING NPC - dialogueOutcome passed: " + dialogueOutcome);
        dialogueTriggerer.StartDialogue(dialogueOutcome);
    }

    public void SellCandy()
    {
        Debug.Log("CANDY BUYING NPC - the dialogue routes correctly to attempt to sell a candy");
        StartCoroutine(OpenSaleUI());
    }

    private IEnumerator OpenSaleUI()
    {
        yield return null;                       // let the dialogue finish closing first
        containerUI = GameObject.FindAnyObjectByType<ContainerUIController>(FindObjectsInactive.Include);
        if (containerUI == null)
        {
            Debug.LogError("CANDY BUYING NPC - container ui controller not found and cannot be opened; yield breaking.");
            yield break;
        }
        containerUI.gameObject.SetActive(true);
        containerUI.SpawnInventorySlots(saleContainer);
    }

    public int GetOffer(Candy candy)
    {
        if (Refuses(candy)) return 0;
        return IsFavorite(candy) ? basePrice + favoriteBonus : basePrice;
    }

    public bool Refuses(Candy candy)
    {
        if (taste == null || taste.RefusedTraits == null) return false;

        foreach (Traits_ScriptableObjects trait in taste.RefusedTraits)
        {
            if (candy.HasTrait(trait)) return true;
        }
        return false;
    }

    public bool IsFavorite(Candy candy)
    {
        return taste != null && candy.HasTrait(taste.FavoriteTrait);
    }

    #endregion

    //TODO: Swap this for the real dialogue system once it's added
    private void Say(string line)
    {
        Debug.Log(npcName + ": " + line);
    }
}
