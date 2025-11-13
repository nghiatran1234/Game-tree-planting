using UnityEngine;
using System.Collections.Generic;

public enum QuestType
{
    Collect,
    Kill
}

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest System/Quest")]
public class Quest : ScriptableObject
{
    [Header("Info")]
    public string questID; 
    public string questTitle;
    [TextArea(3, 5)]
    public string questDescription;
    public Sprite questIcon;

    [Header("Objectives")]
    public QuestType questType;
    public string objectiveID; 
    public int objectiveQuantity;

    [Header("Rewards")]
    public string rewardItemID; 
    public int rewardQuantity;

    [Header("Dialogue")]
    [TextArea(2, 4)]
    public List<string> onStartDialogue; 
    [TextArea(2, 4)]
    public List<string> onInProgressDialogue;
    [TextArea(2, 4)]
    public List<string> onCompleteDialogue;

    [Header("Quest Panel Text")]
    [TextArea(2, 4)]
    public string questAcceptanceText = "Bạn có đồng ý giúp tôi không?";
}