using UnityEngine;
using System.Collections.Generic;
using System;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Trạng thái")]
    public Quest activeQuest;
    public List<string> completedQuestNames = new List<string>();

    public event Action OnQuestUpdated;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool IsQuestCompleted(string questName) => completedQuestNames.Contains(questName);

    public void AcceptQuest(QuestData data)
    {
        if (data == null) return;
        activeQuest = new Quest(data);
        if (activeQuest.info.type == QuestType.GatherItem) CheckCompletionCondition();

        Debug.Log($"Đã nhận: {activeQuest.info.questName}");
        OnQuestUpdated?.Invoke();
    }

    public void UpdateQuestProgress(int amount)
    {
        if (activeQuest == null || activeQuest.info == null) return;

        activeQuest.currentAmount += amount;

        if (activeQuest.currentAmount > activeQuest.info.requiredAmount)
            activeQuest.currentAmount = activeQuest.info.requiredAmount;

        Debug.Log($"Tiến độ: {activeQuest.currentAmount}/{activeQuest.info.requiredAmount}");
        OnQuestUpdated?.Invoke();
    }

    public void AddKill(string enemyName)
    {
        if (activeQuest != null && activeQuest.info.type == QuestType.KillEnemy && activeQuest.info.targetName == enemyName)
        {
            UpdateQuestProgress(1);
            CheckAutoComplete();
        }
    }

    public void TalkToNPC(string npcName)
    {
        if (activeQuest != null && activeQuest.info.type == QuestType.TalkToNPC && activeQuest.info.targetName == npcName)
        {
            UpdateQuestProgress(1);
            CheckAutoComplete();
        }
    }

    public void UpdateGatherProgress()
    {
        if (activeQuest != null && activeQuest.info.type == QuestType.GatherItem)
        {
            CheckCompletionCondition();
            OnQuestUpdated?.Invoke();
        }
    }
    public bool CheckCompletionCondition()
    {
        if (activeQuest == null) return false;

        switch (activeQuest.info.type)
        {
            case QuestType.KillEnemy:
            case QuestType.TalkToNPC:
                return activeQuest.currentAmount >= activeQuest.info.requiredAmount;

            case QuestType.GatherItem:
                if (InventoryManager.Instance != null && activeQuest.info.requiredItem != null)
                {
                    int count = InventoryManager.Instance.GetItemAmount(activeQuest.info.requiredItem);
                    activeQuest.currentAmount = count;
                    return InventoryManager.Instance.HasItem(activeQuest.info.requiredItem, activeQuest.info.requiredAmount);
                }
                break;
        }
        return false;
    }

    private void CheckAutoComplete()
    {
        // Check auto complete quest
        // if (CheckCompletionCondition()) OnQuestUpdated?.Invoke(); 
    }

    public void CompleteQuest()
    {
        if (activeQuest == null || !CheckCompletionCondition()) return;
        if (activeQuest.info.type == QuestType.GatherItem && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.RemoveItem(activeQuest.info.requiredItem, activeQuest.info.requiredAmount);
        }
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.UpdateGold(activeQuest.info.goldReward);
            if (activeQuest.info.itemReward != null)
                InventoryManager.Instance.AddItem(activeQuest.info.itemReward);
        }
        if (!completedQuestNames.Contains(activeQuest.info.questName))
            completedQuestNames.Add(activeQuest.info.questName);

        Debug.Log($"HOÀN THÀNH: {activeQuest.info.questName}");

        activeQuest = null; 
        OnQuestUpdated?.Invoke();
        QuestData finishedQuestData = activeQuest.info;
        if (finishedQuestData.nextQuest != null)
        {
            if (finishedQuestData.autoAcceptNextQuest)
            {
                AcceptQuest(finishedQuestData.nextQuest);

                Debug.Log(">>> Tự động nhận quest tiếp theo: " + finishedQuestData.nextQuest.questName);
            }
            else
            {
                Debug.Log(">>> Quest tiếp theo đã mở khóa, hãy đi tìm NPC!");
            }
        }
    }
}