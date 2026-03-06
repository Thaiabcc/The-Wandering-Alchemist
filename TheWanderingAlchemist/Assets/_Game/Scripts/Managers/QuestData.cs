using UnityEngine;

public enum QuestType { KillEnemy, GatherItem, TalkToNPC }

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest System/New Quest")]
public class QuestData : ScriptableObject
{
    [Header("--- THÔNG TIN CHUNG ---")]
    public string questName;
    [TextArea(3, 10)] public string description;

    [Header("--- LOGIC ---")]
    public QuestType type;
    public QuestData prerequisiteQuest;
    public QuestData nextQuest;
    public bool autoAcceptNextQuest = false;

    [Header("--- YÊU CẦU ---")]
    public string targetName;
    public int requiredAmount;
    public ItemData requiredItem;

    [Header("--- PHẦN THƯỞNG ---")]
    public int goldReward;
    public ItemData itemReward;

    [Header("--- HỘI THOẠI ---")]
    [TextArea(2, 5)] public string[] startDialogue;
    [TextArea(2, 5)] public string[] progressDialogue;
    [TextArea(2, 5)] public string[] completeDialogue;
    [TextArea(2, 5)] public string[] targetDialogue;

    [Header("Scene trả Quest")]
    [TextArea(3, 10)]
    public string endStoryText;

}