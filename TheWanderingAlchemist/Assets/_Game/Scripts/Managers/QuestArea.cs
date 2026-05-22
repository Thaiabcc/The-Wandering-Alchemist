using UnityEngine;

public class QuestAreaTrigger : MonoBehaviour
{
    [Header("--- QUEST CONFIGURATION ---")]
    [SerializeField] private QuestData questToTrigger; 

    [Header("--- TIME CONDITIONS ---")]
    [SerializeField] private int startValidHour = 18;
    [SerializeField] private int endValidHour = 6;

    [Header("--- TRIGGER OPTIONS ---")]
    [SerializeField] private bool showPopupPrompt = false;

    private bool isPlayerInside = false;
    private bool hasAccepted = false; 

    private void Update()
    {
        if (!hasAccepted && isPlayerInside)
        {
            TryTriggerQuest();
        }

        if (hasAccepted)
        {
            CheckAndAutoReward();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }

    private void TryTriggerQuest()
    {
        if (questToTrigger == null || TimeManager.Instance == null) return;
        if (!IsCorrectTimeWindow()) return;

        if (QuestManager.Instance.IsQuestCompleted(questToTrigger.questName))
        {
            gameObject.SetActive(false);
            return;
        }

        if (QuestManager.Instance.activeQuests.Exists(q => q.info.questName == questToTrigger.questName))
        {
            hasAccepted = true;
            return;
        }

        if (showPopupPrompt)
        {
            if (QuestPopupUI.Instance != null)
            {
                QuestPopupUI.Instance.ShowPopup(questToTrigger);
                hasAccepted = true; 
            }
        }
        else
        {
            QuestManager.Instance.AcceptQuest(questToTrigger);
            
            Quest acceptedQuest = QuestManager.Instance.activeQuests.Find(q => q.info.questName == questToTrigger.questName);
            if (acceptedQuest != null)
            {
                QuestManager.Instance.TrackQuest(acceptedQuest);
            }
            
            hasAccepted = true; 
        }
    }

    private void CheckAndAutoReward()
    {
        if (questToTrigger == null || QuestManager.Instance == null) return;

        Quest runningQuest = QuestManager.Instance.activeQuests.Find(q => q.info.questName == questToTrigger.questName);

        if (runningQuest == null)
        {
            if (QuestManager.Instance.IsQuestCompleted(questToTrigger.questName))
            {
                gameObject.SetActive(false); 
            }
            return;
        }

        if (runningQuest.isCompleted)
        {
            QuestManager.Instance.CompleteQuest(questToTrigger.questName);
            gameObject.SetActive(false);
        }
    }

    private bool IsCorrectTimeWindow()
    {
        float currentHour = TimeManager.Instance.CurrentHour;
        if (startValidHour > endValidHour)
        {
            return (currentHour >= startValidHour || currentHour < endValidHour);
        }
        else
        {
            return (currentHour >= startValidHour && currentHour < endValidHour);
        }
    }
}