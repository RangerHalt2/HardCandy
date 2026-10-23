using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{

    [SerializeField] private TextAsset dialogueJSON;
    [SerializeField] private bool isDialogueOnStart = true;
    [SerializeField] private bool isManuallyContinuing = true;
    private DialogueManager manager;
    private Dialogue dialog;


    private void Start()
    {
        manager = GetComponent<DialogueManager>();
        if (manager == null) manager = GameObject.FindAnyObjectByType<DialogueManager>();
        if(dialogueJSON != null)
            dialog = manager.LoadDialogueFromJSON(dialogueJSON);
        if (isDialogueOnStart)
            manager.StartDialogue(dialog);
    }

    public void SetDialogueJSON(TextAsset JSON)
    {
        dialogueJSON = JSON;
        dialog = manager.LoadDialogueFromJSON(dialogueJSON);
    }

    public void StartDialogue(IDialogueOutcome outcome)
    {
        manager.isManuallyContinuing = isManuallyContinuing;
        manager.outcome = outcome;
        manager.StartDialogue(dialog);
    }

}