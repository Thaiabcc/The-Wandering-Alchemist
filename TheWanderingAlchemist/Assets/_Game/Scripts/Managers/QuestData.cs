using UnityEngine;

public enum QuestType
{
    KillEnemy,
    GatherItem
}

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest System/New Quest")]
public class QuestData : ScriptableObject
{
    [Header("Thông tin chung")]
    public string questName;
    public QuestType type;

    [Header("Yêu cầu")]
    [Tooltip("Tên quái (nếu diệt) hoặc Tên hiển thị (nếu nhặt)")]
    public string targetName;
    public int requiredAmount;

    [Header("Dành cho Quest Thu Thập")]
    public ItemData requiredItem;

    [Header("Phần thưởng")]
    public int goldReward;
    public ItemData itemReward;

    [Header("Hội thoại NPC")]
    [TextArea(2, 5)] public string[] startDialogue;    // Khi nhận
    [TextArea(2, 5)] public string[] progressDialogue; // Khi đang làm
    [TextArea(2, 5)] public string[] completeDialogue; // Khi hoàn thành
}