using UnityEngine;

public enum QuestType { KillEnemy, GatherItem, TalkToNPC }

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest System/New Quest")]
public class QuestData : ScriptableObject
{
    [Header("--- Genaral ---")]
    public string questName;
    [TextArea(3, 10)] public string description;

    [Header("--- LOGIC ---")]
    public QuestType type;
    public QuestData prerequisiteQuest;
    public QuestData nextQuest;
    public bool autoAcceptNextQuest = false;

    [Header("--- Require ---")]
    public string targetName;
    public int requiredAmount;
    public ItemData requiredItem;

    [Header("--- Reward ---")]
    public int goldReward;
    public ItemData itemReward;

    [Header("--- Dialog ---")]
    [TextArea(2, 5)] public string[] startDialogue;
    [TextArea(2, 5)] public string[] progressDialogue;
    [TextArea(2, 5)] public string[] completeDialogue;
    [TextArea(2, 5)] public string[] targetDialogue;

    [Header("Scene back Quest")]
    [TextArea(3, 10)]
    public string endStoryText;

}