using System.Collections.Generic;
using UnityEngine;

// Lớp này dùng để theo dõi tiến độ
[System.Serializable]
public class QuestProgress
{
    public string questID;
    public Quest questData;
    public int currentProgress;
    
    public QuestProgress(Quest quest)
    {
        this.questID = quest.questID;
        this.questData = quest;
        this.currentProgress = 0;
    }
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; set; }

    public List<QuestProgress> activeQuests = new List<QuestProgress>();
    public List<string> completedQuestIDs = new List<string>();

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
    }

    // Hàm này được gọi bởi NPC để bắt đầu quest
    public void StartQuest(Quest questToStart)
    {
        if (!completedQuestIDs.Contains(questToStart.questID))
        {
            QuestProgress newQuest = new QuestProgress(questToStart);
            activeQuests.Add(newQuest);
            Debug.Log("Bắt đầu Quest: " + questToStart.questTitle);
            // (Bạn có thể gọi UI Quest Log ở đây)
        }
    }

    // Hàm này được gọi bởi Kẻ thù (khi chết) hoặc Player (khi nhặt)
    public void UpdateProgress(string objectiveID, int amount)
    {
        foreach (QuestProgress quest in activeQuests)
        {
            if (quest.questData.objectiveID == objectiveID)
            {
                // Chỉ tăng tiến độ nếu quest là loại "Kill"
                // (Loại "Collect" sẽ được kiểm tra trực tiếp từ Inventory)
                if(quest.questData.questType == QuestType.Kill)
                {
                    quest.currentProgress += amount;
                    Debug.Log("Quest progress: " + quest.currentProgress + " / " + quest.questData.objectiveQuantity);
                }
            }
        }
    }

    // Hàm này được gọi bởi NPC để kiểm tra
    public bool CheckCompletion(Quest questToCheck)
    {
        foreach (QuestProgress quest in activeQuests)
        {
            if (quest.questID == questToCheck.questID)
            {
                if (quest.questData.questType == QuestType.Collect)
                {
                    // Kiểm tra Inventory
                    if (InventorySystem.Instance == null) return false;
                    
                    int itemCount = InventorySystem.Instance.CountItem(quest.questData.objectiveID);
                    return (itemCount >= quest.questData.objectiveQuantity);
                }
                else if (quest.questData.questType == QuestType.Kill)
                {
                    return (quest.currentProgress >= quest.questData.objectiveQuantity);
                }
            }
        }
        return false;
    }

    // Hàm này được gọi bởi NPC khi hoàn thành
    public void CompleteQuest(Quest questToComplete)
    {
        // 1. Tìm quest trong activeQuests
        QuestProgress quest = activeQuests.Find(q => q.questID == questToComplete.questID);
        if (quest == null) return;

        // 2. Xóa item (nếu là quest Collect)
        if(quest.questData.questType == QuestType.Collect)
        {
            InventorySystem.Instance.RemoveItem(quest.questData.objectiveID, quest.questData.objectiveQuantity);
        }

        // 3. Đưa phần thưởng
        InventorySystem.Instance.AddToInventory(quest.questData.rewardItemID, quest.questData.rewardQuantity);

        // 4. Chuyển quest sang Completed
        activeQuests.Remove(quest);
        completedQuestIDs.Add(quest.questID);

        Debug.Log("Hoàn thành Quest: " + quest.questData.questTitle);
        // (Cập nhật UI)
    }
}