using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Enum để theo dõi trạng thái quest của NPC
public enum QuestState
{
    NotStarted,
    InProgress,
    Completed
}

[RequireComponent(typeof(NPC_Voice))]
public class QuestGiver : MonoBehaviour
{
    public Quest questToGive;
    public QuestState currentState = QuestState.NotStarted;
    
    private Text interactionText; 
    private bool isTalking = false; 
    public float dialogueDisplayTime = 4.0f; // Thời gian hiển thị lời thoại (4 giây)

    private void Start() 
    {
        if (SelectionManager.instance != null)
        {
            interactionText = SelectionManager.instance.interaction_text;
        }
    }

    private void Update()
    {
        if (interactionText == null)
        {
            if (SelectionManager.instance != null)
            {
                interactionText = SelectionManager.instance.interaction_text;
            }
            if (interactionText == null) return; 
        }

        if (SelectionManager.instance != null && 
            SelectionManager.instance.selectedNPC == this.gameObject)
        {
            if (!isTalking) 
            {
                switch (currentState)
                {
                    case QuestState.NotStarted:
                        interactionText.text = "Talk [E]";
                        break;
                    case QuestState.InProgress:
                        bool isComplete = QuestManager.Instance.CheckCompletion(questToGive);
                        if(isComplete)
                        {
                            interactionText.text = "Complete Quest [E]";
                        }
                        else
                        {
                            interactionText.text = "In Progress: " + questToGive.questTitle;
                        }
                        break;
                    case QuestState.Completed:
                        interactionText.text = "Thank you!";
                        break;
                }
            }
        }
    }
    
    public void Interact()
    {
        if (questToGive == null)
        {
            DialogueManager.Instance.ShowSingleDialogue("Xin chào, tôi không có việc gì cho bạn cả.");
            return;
        }
        
        switch (currentState)
        {
            case QuestState.NotStarted:
                DialogueManager.Instance.StartDialogue(questToGive.onStartDialogue, questToGive, this);
                break;

            case QuestState.InProgress:
                bool isComplete = QuestManager.Instance.CheckCompletion(questToGive);
                if (isComplete)
                {
                    DialogueManager.Instance.StartDialogue(questToGive.onCompleteDialogue);
                    QuestManager.Instance.CompleteQuest(questToGive);
                    currentState = QuestState.Completed;
                }
                else
                {
                    DialogueManager.Instance.StartDialogue(questToGive.onInProgressDialogue);
                }
                break;

            case QuestState.Completed:
                DialogueManager.Instance.ShowSingleDialogue("Cảm ơn bạn lần nữa nhé!");
                break;
        }
    }

    private IEnumerator ShowDialogue(string dialogue)
    {
        isTalking = true;
        if (interactionText != null)
        {
            interactionText.text = dialogue;
        }
        
        yield return new WaitForSeconds(dialogueDisplayTime);
        
        isTalking = false; 
    }
}