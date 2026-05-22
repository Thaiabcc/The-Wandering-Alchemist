using UnityEngine;
using System.Collections.Generic;
using System;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Status")]
    public List<Quest> activeQuests = new List<Quest>(); 
    public List<string> completedQuestNames = new List<string>();
    public Quest trackedQuest; 

    [Header("Default Quest")]
    public QuestData startingMainQuest; 

    public event Action OnQuestUpdated;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (startingMainQuest != null)
        {
            AcceptQuest(startingMainQuest);
            TrackQuest(activeQuests[0]); 
        }
    }

    public bool IsQuestCompleted(string questName) => completedQuestNames.Contains(questName);

    public void AcceptQuest(QuestData data)
    {
        if (data == null) return;
        if (IsQuestCompleted(data.questName) || activeQuests.Exists(q => q.info.questName == data.questName)) return;

        Quest newQuest = new Quest(data);
        activeQuests.Add(newQuest);
        
        if (trackedQuest == null) trackedQuest = newQuest;
        if (newQuest.info.type == QuestType.GatherItem) CheckCompletionCondition(newQuest);

        OnQuestUpdated?.Invoke();
    }

    public void AddKill(string enemyName)
    {
        bool hasUpdate = false;
        foreach (Quest q in activeQuests)
        {
            if (q.info.type == QuestType.KillEnemy && q.info.targetName == enemyName)
            {
                q.currentAmount++;
                CheckCompletionCondition(q);
                hasUpdate = true;
            }
        }
        if (hasUpdate) OnQuestUpdated?.Invoke();
    }

    public void TalkToNPC(string npcName)
    {
        bool hasUpdate = false;
        foreach (Quest q in activeQuests)
        {
            if (q.info.type == QuestType.TalkToNPC && q.info.targetName == npcName)
            {
                q.currentAmount++;
                CheckCompletionCondition(q);
                hasUpdate = true;
            }
        }
        if (hasUpdate) OnQuestUpdated?.Invoke();
    }

    public void UpdateGatherProgress()
    {
        bool hasUpdate = false;
        foreach (Quest q in activeQuests)
        {
            if (q.info.type == QuestType.GatherItem)
            {
                CheckCompletionCondition(q);
                hasUpdate = true;
            }
        }
        if (hasUpdate) OnQuestUpdated?.Invoke();
    }

    public bool CheckCompletionCondition(Quest q)
    {
        if (q == null) return false;
        switch (q.info.type)
        {
            case QuestType.KillEnemy:
            case QuestType.TalkToNPC:
                if (q.currentAmount >= q.info.requiredAmount) { q.isCompleted = true; return true; }
                break;
            case QuestType.GatherItem:
                if (InventoryManager.Instance != null && q.info.requiredItem != null)
                {
                    int count = InventoryManager.Instance.GetItemAmount(q.info.requiredItem);
                    q.currentAmount = count;
                    if (count >= q.info.requiredAmount) { q.isCompleted = true; return true; }
                }
                break;
        }
        return false;
    }

    public void CompleteQuest(string questName)
    {
        Quest targetQuest = activeQuests.Find(q => q.info.questName == questName);
        if (targetQuest == null || !targetQuest.isCompleted) return;

        if (targetQuest.info.type == QuestType.GatherItem && InventoryManager.Instance != null)
            InventoryManager.Instance.RemoveItem(targetQuest.info.requiredItem, targetQuest.info.requiredAmount);

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.UpdateGold(targetQuest.info.goldReward);
            if (targetQuest.info.itemReward != null) InventoryManager.Instance.AddItem(targetQuest.info.itemReward);
        }

        if (!completedQuestNames.Contains(targetQuest.info.questName))
            completedQuestNames.Add(targetQuest.info.questName);

        activeQuests.Remove(targetQuest);

        if (trackedQuest == targetQuest) 
        {
            trackedQuest = activeQuests.Count > 0 ? activeQuests[0] : null; 
        }

        OnQuestUpdated?.Invoke();

        if (targetQuest.info.nextQuest != null && targetQuest.info.autoAcceptNextQuest)
            AcceptQuest(targetQuest.info.nextQuest);
    }

    public void TrackQuest(Quest q)
    {
        trackedQuest = q;
        OnQuestUpdated?.Invoke(); 
    }

    public void UntrackQuest()
    {
        trackedQuest = null;
        OnQuestUpdated?.Invoke();
    }
}