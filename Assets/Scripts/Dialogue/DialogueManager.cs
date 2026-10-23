using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI name_text_box;
    [SerializeField] private TextMeshProUGUI dialogue_text;
    [SerializeField] private Button continue_button;

    [SerializeField] private Canvas dialogue_canvas; //This will store the canvas that all the dialogue text shows up in
    [SerializeField] private Canvas game_canvas; //This will store any canvases that the game might use that need to be hidden.

    [SerializeField] private Canvas[] choice_panels;
    [SerializeField] private TextMeshProUGUI[] choice_text;

    private Button[] choice_buttons;

    private bool isActive = false;

    private Dialogue currentDialogue;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private InputHandler input_manager;
    [HideInInspector] public IDialogueOutcome outcome;

    [HideInInspector] public bool isManuallyContinuing = true;

    private void Start()
    {
        input_manager = GameObject.FindAnyObjectByType<InputHandler>();
        choice_buttons = new Button[choice_panels.Length];
        for (int i = 0; i < choice_panels.Length; i++)
        {
            Button btn = choice_panels[i].GetComponentInChildren<Button>();
            choice_buttons[i] = btn;
        }
    }

    private void Update()
    {
        if(input_manager.ClickTriggered && isManuallyContinuing && isActive)
        {
            input_manager.ClickTriggered = false;
            if(isActive)
                DisplayNextSentence();
        }
    }

    public void StartDialogue(Dialogue dialog)
    {
        isActive = true;
        dialogue_canvas.gameObject.SetActive(true);
        game_canvas.gameObject.SetActive(false);
        currentDialogue = dialog;
        currentLineIndex = 0;

        PlayerController pc = GameObject.FindAnyObjectByType<PlayerController>();
        pc.blockInputs = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        DisplayLine();
    }

    private void DisplayLine()
    {
        if(currentLineIndex >= currentDialogue.lines.Length)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = currentDialogue.lines[currentLineIndex];
        name_text_box.text = line.speakerName;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeSentence(line.sentence));

        foreach (var panel in choice_panels)
        {
            panel.gameObject.SetActive(false);
        }

        bool hasChoices = line.choices != null && line.choices.Length > 0;
        if(continue_button != null)
            continue_button.gameObject.SetActive(!hasChoices);

        if (hasChoices)
        {
            StartCoroutine(ShowChoicesDelayed(line));
        }


    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogue_text.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogue_text.text += letter;
            yield return new WaitForSeconds(0.02f); //Speed of typing
        }
        isTyping = false;
    }

    IEnumerator ShowChoicesDelayed(DialogueLine line)
    {
        while (isTyping)
        {
            yield return null;
        }

        for (int i = 0; i < choice_panels.Length; i++)
        {
            if (i < line.choices.Length)
            {
                choice_panels[i].gameObject.SetActive(true);
                choice_text[i].text = line.choices[i].choiceText;

                choice_buttons[i].onClick.RemoveAllListeners();


                DialogueChoice currentChoice = line.choices[i];
                int nextIndex = line.choices[i].nextLineIndex;

                choice_buttons[i].onClick.AddListener(() =>
                {
                    OnChoicesSelected(currentChoice);
                    //currentLineIndex = nextIndex;
                    DisplayLine();
                });
            }

            else
            {
                choice_panels[i].gameObject.SetActive(false);
            }
        }
    }


    private void DisplayNextSentence()
    {
        //This, contrary to the function name, will just finish the typing and skip the typing just making the text show for the sentence but not actually show the next sentence.
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogue_text.text = currentDialogue.lines[currentLineIndex].sentence;
            isTyping = false;
            return;
        }

        DialogueLine line = currentDialogue.lines[currentLineIndex];

        if (line.choices == null || line.choices.Length == 0)
        {
            if (line.nextLineIndex != -1)
            {
                currentLineIndex = line.nextLineIndex;
                DisplayLine();
            }
            else
            {
                EndDialogue();
            }
        }

    }


    void OnChoicesSelected(DialogueChoice choice)
    {
        if (choice.nextLineIndex >= 0)
        {
            currentLineIndex = choice.nextLineIndex;
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        Debug.Log("DIALOGUE MANAGER - ending the dialogue");
        bool shouldOutcome = currentDialogue.lines[currentLineIndex].outcome;
        dialogue_canvas.gameObject.SetActive(false);
        game_canvas.gameObject.SetActive(true);
        isActive = false;

        PlayerController pc = GameObject.FindAnyObjectByType<PlayerController>();
        pc.blockInputs = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if(shouldOutcome)
            HandleDialogueOutcome();

        return;
    }

    public void HandleDialogueOutcome()
    {
        if(outcome == null)
        {
            Debug.LogWarning("DIALOGUE MANAGER - the outcome is null; returning.");
            return;
        }
        outcome.Execute();
    }

    public Dialogue LoadDialogueFromJSON(TextAsset jsonFile)
    {
        return JsonUtility.FromJson<Dialogue>(jsonFile.text);
    }

}
