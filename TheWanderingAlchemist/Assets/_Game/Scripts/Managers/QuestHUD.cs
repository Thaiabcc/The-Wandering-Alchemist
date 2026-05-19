using UnityEngine;
using TMPro;

public class QuestHUD : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject questPanel;
    [SerializeField] private TextMeshProUGUI questText;

    private void Start()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestUpdated += UpdateUI;
        }
        UpdateUI();
    }

    private void OnDestroy()
    {
        if (QuestManager.Instance != null) 
        {
            QuestManager.Instance.OnQuestUpdated -= UpdateUI;
        }
    }

    private void UpdateUI()
    {
        if (QuestManager.Instance == null) return;
        
        Quest q = QuestManager.Instance.trackedQuest; 
        
        if (q != null && q.info != null && !q.isCompleted)
        {
            questPanel.SetActive(true);
            questText.text = $"<color=yellow>{q.info.questName}</color>\nProgress: {q.currentAmount} / {q.info.requiredAmount}";
        }
        else
        {
            questPanel.SetActive(false); 
        }
    }
}