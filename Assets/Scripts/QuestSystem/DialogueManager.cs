using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; 
using System.Collections; 

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; set; }

    [Header("UI Components")]
    public GameObject dialoguePanel;
    public Text dialogueText;
    public Text questTitleText;
    public Text questDescriptionText;
    public Text questObjectiveText;
    public Button acceptButton;
    public Button continueButton; 

    [Header("Typing Effect")]
    public float typingSpeed = 0.02f;
    private Coroutine typingCoroutine;

    public bool isDialogueActive = false;

    private Quest currentQuest;
    private QuestGiver currentGiver;
    private Queue<string> sentencesQueue; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        sentencesQueue = new Queue<string>(); 
    }

    private void Start()
    {
        dialoguePanel.SetActive(false);
    }

    private void OpenDialoguePanel()
    {
        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        Time.timeScale = 0f; 
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (SelectionManager.instance != null)
        {
            SelectionManager.instance.interaction_Info_UI.SetActive(false);
            SelectionManager.instance.handIcon.gameObject.SetActive(false);
            SelectionManager.instance.centerDotImage.gameObject.SetActive(false);
        }
        
        if (PauseMenu.Instance != null)
        {
            if(PauseMenu.Instance.playerHUDPanel != null) PauseMenu.Instance.playerHUDPanel.SetActive(false);
            if(PauseMenu.Instance.quickSlotsPanel != null) PauseMenu.Instance.quickSlotsPanel.SetActive(false);
        }
    }

    public void CloseDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        Time.timeScale = 1f;
        
        if (PauseMenu.Instance != null && !PauseMenu.Instance.isPaused)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            if(PauseMenu.Instance.playerHUDPanel != null) PauseMenu.Instance.playerHUDPanel.SetActive(true);
            if(PauseMenu.Instance.quickSlotsPanel != null) PauseMenu.Instance.quickSlotsPanel.SetActive(true);
        }
    }

    public void StartDialogue(List<string> sentences, Quest quest = null, QuestGiver giver = null)
    {
        OpenDialoguePanel();
        
        currentQuest = quest; 
        currentGiver = giver;
        
        sentencesQueue.Clear();
        foreach (string sentence in sentences)
        {
            sentencesQueue.Enqueue(sentence);
        }

        questTitleText.text = "";
        questDescriptionText.text = "";
        questObjectiveText.text = "";
        acceptButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(true);

        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(DisplayNextSentence); 
        
        DisplayNextSentence(); 
    }

    public void DisplayNextSentence()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        if (sentencesQueue.Count == 0)
        {
            if (currentQuest != null)
            {
                ShowQuestAcceptancePanel();
            }
            else
            {
                CloseDialogue();
            }
            return;
        }
        
        continueButton.interactable = false;
        string sentence = sentencesQueue.Dequeue();
        typingCoroutine = StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed); 
        }
        continueButton.interactable = true;
    }

    void ShowQuestAcceptancePanel()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        
        continueButton.interactable = true;

        dialogueText.text = currentQuest.questAcceptanceText;
        questTitleText.text = currentQuest.questTitle;
        questDescriptionText.text = currentQuest.questDescription;
        questObjectiveText.text = currentQuest.objectiveID + ": 0 / " + currentQuest.objectiveQuantity;

        acceptButton.gameObject.SetActive(true);
        continueButton.gameObject.SetActive(true); 

        acceptButton.onClick.RemoveAllListeners();
        continueButton.onClick.RemoveAllListeners();
        
        acceptButton.onClick.AddListener(AcceptQuest);
        continueButton.onClick.AddListener(CloseDialogue); 
    }
    
    public void ShowSingleDialogue(string dialogue)
    {
        OpenDialoguePanel();
        
        currentQuest = null; 
        currentGiver = null;
        sentencesQueue.Clear(); 

        questTitleText.text = "";
        questDescriptionText.text = "";
        questObjectiveText.text = "";
        
        acceptButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(true);
        
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(CloseDialogue);

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        continueButton.interactable = false;
        typingCoroutine = StartCoroutine(TypeSentence(dialogue));
    }

    void AcceptQuest()
    {
        QuestManager.Instance.StartQuest(currentQuest);
        currentGiver.currentState = QuestState.InProgress;
        CloseDialogue();
    }
}