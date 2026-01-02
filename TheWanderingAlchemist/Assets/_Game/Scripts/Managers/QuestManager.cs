using UnityEngine;
using System.Collections.Generic;
using System; // 👈 [QUAN TRỌNG] Thêm dòng này để dùng Action

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Trạng thái")]
    public Quest activeQuest; // Quest đang chạy

    [Header("Lịch sử (Chống Spam)")]
    public List<string> completedQuestNames = new List<string>();

    // 👇 [MỚI] Sự kiện báo tin (Cái loa phát thanh)
    // Các script UI sẽ lắng nghe cái này. Nếu chưa có UI thì cũng ko sao cả.
    public event Action OnQuestUpdated;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        activeQuest = null;
    }

    // Kiểm tra xem Quest này đã từng làm xong chưa
    public bool IsQuestCompleted(string questName) => completedQuestNames.Contains(questName);

    // --- 1. NHẬN QUEST ---
    public void AcceptQuest(QuestData data)
    {
        if (data == null) return;

        // Tạo một Quest runtime mới dựa trên Data
        activeQuest = new Quest(data);

        Debug.Log($"Đã nhận nhiệm vụ: {activeQuest.info.questName}");

        // 👇 Báo cập nhật ngay khi nhận
        OnQuestUpdated?.Invoke();
    }

    // --- 2. CẬP NHẬT TIẾN ĐỘ KILL ---
    public void AddKill(string enemyName)
    {
        if (activeQuest == null || activeQuest.isCompleted) return;

        // Truy cập qua .info
        if (activeQuest.info.type != QuestType.KillEnemy) return;

        // So khớp tên quái (Chính xác từng chữ)
        if (activeQuest.info.targetName == enemyName)
        {
            activeQuest.currentAmount++;
            Debug.Log($"Tiến độ: {activeQuest.currentAmount}/{activeQuest.info.requiredAmount}");

            // 👇 Báo cập nhật UI (Dù có UI hay chưa cũng gọi dòng này cho chắc)
            OnQuestUpdated?.Invoke();

            // Tự động kiểm tra hoàn thành nếu thích (Optional)
            // if (CheckCompletionCondition()) { ... }
        }
    }

    // --- 3. KIỂM TRA ĐIỀU KIỆN ---
    public bool CheckCompletionCondition()
    {
        if (activeQuest == null) return false;

        if (activeQuest.info.type == QuestType.KillEnemy)
        {
            return activeQuest.currentAmount >= activeQuest.info.requiredAmount;
        }
        else if (activeQuest.info.type == QuestType.GatherItem)
        {
            if (InventoryManager.Instance != null && activeQuest.info.requiredItem != null)
            {
                // Cập nhật số lượng hiện có để hiển thị
                int currentCount = InventoryManager.Instance.GetItemCount(activeQuest.info.requiredItem);
                activeQuest.currentAmount = currentCount;

                // 👇 Báo cập nhật UI khi nhặt đồ
                OnQuestUpdated?.Invoke();

                return InventoryManager.Instance.HasItem(activeQuest.info.requiredItem, activeQuest.info.requiredAmount);
            }
        }
        return false;
    }

    // --- 4. HOÀN THÀNH ---
    public void CompleteQuest()
    {
        if (activeQuest == null || activeQuest.isCompleted) return;

        if (CheckCompletionCondition())
        {
            // Trừ đồ thu thập
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

            // Ghi vào lịch sử
            if (!completedQuestNames.Contains(activeQuest.info.questName))
            {
                completedQuestNames.Add(activeQuest.info.questName);
            }

            Debug.Log($"Hoàn thành Quest: {activeQuest.info.questName}");
            activeQuest.isCompleted = true;
            activeQuest = null; // Reset

            // 👇 Báo cập nhật lần cuối (để UI ẩn đi)
            OnQuestUpdated?.Invoke();
        }
    }
}