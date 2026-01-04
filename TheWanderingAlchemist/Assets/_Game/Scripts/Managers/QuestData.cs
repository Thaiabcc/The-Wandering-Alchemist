using UnityEngine;

// Định nghĩa các loại nhiệm vụ
public enum QuestType
{
    KillEnemy,   // Giết quái
    GatherItem,  // Thu thập vật phẩm
    TalkToNPC    // Nói chuyện với NPC (Đi đưa tin/Thám thính)
}

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest System/New Quest")]
public class QuestData : ScriptableObject
{
    // ==========================================
    // 1. THÔNG TIN CƠ BẢN (HIỂN THỊ UI)
    // ==========================================
    [Header("--- THÔNG TIN CHUNG ---")]
    public string questName; // Tên nhiệm vụ (VD: Bí mật rừng sâu)

    [Tooltip("Nội dung sẽ hiện trên bảng Accept/Decline")]
    [TextArea(3, 10)]
    public string description; // Mô tả ngắn gọn (VD: Hãy đi gặp Skeleton để hỏi chuyện...)

    // ==========================================
    // 2. LOGIC & CHUỖI NHIỆM VỤ
    // ==========================================
    [Header("--- CẤU HÌNH LOGIC ---")]
    public QuestType type;

    [Tooltip("Phải làm xong Quest này mới được nhận Quest hiện tại (Móc xích nhiệm vụ)")]
    public QuestData prerequisiteQuest;

    // ==========================================
    // 3. YÊU CẦU ĐỂ HOÀN THÀNH
    // ==========================================
    [Header("--- YÊU CẦU ---")]
    [Tooltip("Điền Tên Quái (nếu Kill) hoặc Tên NPC (nếu Talk)")]
    public string targetName;

    [Tooltip("Số lượng cần giết hoặc thu thập (Nếu là TalkToNPC thì để là 1)")]
    public int requiredAmount;

    [Tooltip("Kéo Item vào đây nếu là Quest thu thập (GatherItem)")]
    public ItemData requiredItem;

    // ==========================================
    // 4. PHẦN THƯỞNG
    // ==========================================
    [Header("--- PHẦN THƯỞNG ---")]
    public int goldReward;
    public ItemData itemReward; // Ví dụ: Chìa khóa Dungeon

    // ==========================================
    // 5. HỘI THOẠI (KẾT NỐI VỚI DIALOGUE MANAGER)
    // ==========================================
    [Header("--- KỊCH BẢN HỘI THOẠI ---")]

    [Tooltip("Nói câu này TRƯỚC khi hiện bảng nhận Quest (Dẫn dắt cốt truyện)")]
    [TextArea(2, 5)]
    public string[] startDialogue;

    [Tooltip("Nói câu này khi người chơi bấm vào NPC lúc ĐANG làm nhiệm vụ (Nhắc nhở)")]
    [TextArea(2, 5)]
    public string[] progressDialogue;

    [Tooltip("Nói câu này khi người chơi hoàn thành nhiệm vụ (Khen ngợi/Cảm ơn)")]
    [TextArea(2, 5)]
    public string[] completeDialogue;
}