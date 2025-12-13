using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Trạng thái Nhiệm vụ")]
    public bool isQuestStarted = false;   // Đã nhận Q chưa?
    public bool isQuestCompleted = false; // Đã xong Q (đã nhận thưởng) chưa?

    [Header("Tiến độ")]
    public int killCount = 0;      // Đã giết bao nhiêu
    public int targetKills = 3;    // Cần giết bao nhiêu

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        // Giữ lại khi chuyển cảnh (để sang map khác đánh quái vẫn tính)
        DontDestroyOnLoad(gameObject);
    }

    // Hàm gọi khi nhận nhiệm vụ từ NPC
    public void AcceptQuest()
    {
        if (!isQuestStarted)
        {
            isQuestStarted = true;
            killCount = 0; // Reset lại cho chắc
            Debug.Log("Đã nhận nhiệm vụ: Diệt Slime!");
        }
    }

    // Hàm gọi khi một con quái chết
    public void AddKill()
    {
        // Chỉ tính nếu nhiệm vụ đang chạy VÀ chưa hoàn thành xong hẳn
        if (isQuestStarted && !isQuestCompleted)
        {
            killCount++;
            Debug.Log($"Tiến độ: {killCount}/{targetKills}");

            if (killCount >= targetKills)
            {
                Debug.Log("Nhiệm vụ hoàn tất! Về gặp NPC nhận thưởng thôi.");
            }
        }
    }

    // Hàm nhận thưởng (Gọi khi nói chuyện xong với NPC lần cuối)
    public void CompleteQuest()
    {
        isQuestCompleted = true;
        Debug.Log("Đã nhận thưởng! +100 Vàng (Ví dụ)");
        // Sau này thêm code cộng tiền/vàng vào đây
    }
}
