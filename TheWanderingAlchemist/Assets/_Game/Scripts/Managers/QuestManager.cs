using UnityEngine;
using System.Collections.Generic;
using System;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Trạng thái")]
    public Quest activeQuest; // Quest đang làm
    public List<string> completedQuestNames = new List<string>(); // Lịch sử quest đã xong

    // Sự kiện để UI cập nhật (nếu bro có UI hiển thị tiến độ góc màn hình)
    public event Action OnQuestUpdated;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Kiểm tra quest đã xong chưa (để check điều kiện nhận quest sau)
    public bool IsQuestCompleted(string questName) => completedQuestNames.Contains(questName);

    // --- 1. NHẬN QUEST ---
    public void AcceptQuest(QuestData data)
    {
        if (data == null) return;
        activeQuest = new Quest(data);

        // Nếu là quest thu thập, check ngay xem trong túi có sẵn đồ chưa
        if (activeQuest.info.type == QuestType.GatherItem) CheckCompletionCondition();

        Debug.Log($"Đã nhận nhiệm vụ: {activeQuest.info.questName}");
        OnQuestUpdated?.Invoke();
    }

    // --- 2. CẬP NHẬT TIẾN ĐỘ ---

    // a. Giết quái (Gọi từ script EnemyHealth khi quái chết)
    public void AddKill(string enemyName)
    {
        if (activeQuest == null || activeQuest.info.type != QuestType.KillEnemy) return;
        if (activeQuest.info.targetName == enemyName)
        {
            activeQuest.currentAmount++;
            OnQuestUpdated?.Invoke();
            CheckAutoComplete();
        }
    }

    // b. Nói chuyện NPC (Gọi từ QuestNPC)
    public void TalkToNPC(string npcName)
    {
        if (activeQuest == null || activeQuest.info.type != QuestType.TalkToNPC) return;
        if (activeQuest.info.targetName == npcName)
        {
            activeQuest.currentAmount = 1; // Đánh dấu là đã gặp
            OnQuestUpdated?.Invoke();
            CheckAutoComplete();
        }
    }

    // c. Nhặt đồ (Gọi từ InventoryManager khi nhặt đồ)
    public void UpdateGatherProgress()
    {
        if (activeQuest == null || activeQuest.info.type != QuestType.GatherItem) return;
        CheckCompletionCondition();
        OnQuestUpdated?.Invoke();
    }

    // --- 3. KIỂM TRA & HOÀN THÀNH ---
    public bool CheckCompletionCondition()
    {
        if (activeQuest == null) return false;

        switch (activeQuest.info.type)
        {
            case QuestType.KillEnemy:
                return activeQuest.currentAmount >= activeQuest.info.requiredAmount;

            case QuestType.TalkToNPC:
                return activeQuest.currentAmount >= 1;

            case QuestType.GatherItem:
                if (InventoryManager.Instance != null && activeQuest.info.requiredItem != null)
                {
                    int count = InventoryManager.Instance.GetItemCount(activeQuest.info.requiredItem);
                    activeQuest.currentAmount = count;
                    return InventoryManager.Instance.HasItem(activeQuest.info.requiredItem, activeQuest.info.requiredAmount);
                }
                break;
        }
        return false;
    }

    private void CheckAutoComplete()
    {
        if (CheckCompletionCondition()) OnQuestUpdated?.Invoke(); // Báo UI là xong rồi
    }

    public void CompleteQuest()
    {
        if (activeQuest == null || !CheckCompletionCondition()) return;

        // Trừ đồ quest thu thập
        if (activeQuest.info.type == QuestType.GatherItem)
        {
            InventoryManager.Instance.RemoveItem(activeQuest.info.requiredItem, activeQuest.info.requiredAmount);
        }

        // Trao thưởng
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.UpdateGold(activeQuest.info.goldReward);
            if (activeQuest.info.itemReward != null)
                InventoryManager.Instance.AddItem(activeQuest.info.itemReward);
        }

        // Lưu lịch sử
        if (!completedQuestNames.Contains(activeQuest.info.questName))
            completedQuestNames.Add(activeQuest.info.questName);

        Debug.Log($"HOÀN THÀNH QUEST: {activeQuest.info.questName}");
        activeQuest = null; // Reset để nhận quest mới
        OnQuestUpdated?.Invoke();
    }
}