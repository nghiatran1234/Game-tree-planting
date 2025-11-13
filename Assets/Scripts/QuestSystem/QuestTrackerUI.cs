using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuestTrackerUI : MonoBehaviour
{
    public static QuestTrackerUI Instance { get; set; }

    public GameObject questTrackerPanel;
    public Text questTitleText;
    public Text questObjectiveText;

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

    private void Start()
    {
        questTrackerPanel.SetActive(false);
    }

    private void Update()
    {
        if (QuestManager.Instance == null || InventorySystem.Instance == null)
        {
            return;
        }

        List<QuestProgress> activeQuests = QuestManager.Instance.activeQuests;

        if (activeQuests.Count == 0)
        {
            questTrackerPanel.SetActive(false);
            return;
        }
        
        questTrackerPanel.SetActive(true);
        QuestProgress firstQuest = activeQuests[0];

        questTitleText.text = firstQuest.questData.questTitle;

        int currentProgress = 0;
        int maxProgress = firstQuest.questData.objectiveQuantity;
        string objectiveID = firstQuest.questData.objectiveID;

        if (firstQuest.questData.questType == QuestType.Kill)
        {
            currentProgress = firstQuest.currentProgress;
        }
        else if (firstQuest.questData.questType == QuestType.Collect)
        {
            currentProgress = InventorySystem.Instance.CountItem(objectiveID);
        }

        questObjectiveText.text = objectiveID + ": " + currentProgress + " / " + maxProgress;
    }
}