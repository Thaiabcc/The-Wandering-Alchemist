using UnityEngine;
using TMPro; // Nhớ dùng TextMeshPro cho đẹp

public class QuestHUD : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject questPanel;     // Cái khung chứa (Panel)
    [SerializeField] private TextMeshProUGUI questText; // Dòng chữ hiển thị

    private void Start()
    {
        // Đăng ký nhận tin từ QuestManager
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestUpdated += UpdateUI;
            UpdateUI(); // Cập nhật ngay khi game bắt đầu
        }
        else
        {
            // Nếu map này chưa có Manager thì ẩn đi
            questPanel.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        // QUAN TRỌNG: Hủy đăng ký khi chuyển map để tránh lỗi
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestUpdated -= UpdateUI;
        }
    }

    private void UpdateUI()
    {
        Quest q = QuestManager.Instance.activeQuest;

        // Chỉ hiện bảng nếu: Có quest VÀ Quest đó chưa xong
        if (q != null && !q.isCompleted)
        {
            questPanel.SetActive(true);
            questText.text = $"{q.info.questName}\nTiến độ: {q.currentAmount} / {q.info.requiredAmount}";
        }
        else
        {
            // Không có quest hoặc làm xong rồi thì ẩn bảng đi
            questPanel.SetActive(false);
        }
    }
}