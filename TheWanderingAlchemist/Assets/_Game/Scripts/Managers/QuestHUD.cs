using UnityEngine;
using TMPro;

public class QuestHUD : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject questPanel;
    [SerializeField] private TextMeshProUGUI questText;

    private void Start()
    {
        // Kiểm tra an toàn
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestUpdated += UpdateUI;
            UpdateUI();
        }
        else
        {
            questPanel.SetActive(false);
        }
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
        // 1. Kiểm tra Manager có tồn tại không
        if (QuestManager.Instance == null) return;

        // 2. Lấy quest đang làm
        Quest q = QuestManager.Instance.activeQuest;

        // 3. [QUAN TRỌNG] Kiểm tra xem Quest có tồn tại không (Khắc phục lỗi Null)
        if (q != null && q.info != null && !q.isCompleted)
        {
            // Nếu có quest -> Hiện bảng
            questPanel.SetActive(true);
            questText.text = $"{q.info.questName}\nTiến độ: {q.currentAmount} / {q.info.requiredAmount}";
        }
        else
        {
            // Nếu KHÔNG có quest (Null) hoặc đã xong -> Ẩn bảng
            questPanel.SetActive(false);
        }
    }
}