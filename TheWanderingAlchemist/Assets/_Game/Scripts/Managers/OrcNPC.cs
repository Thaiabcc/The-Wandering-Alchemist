using UnityEngine;

public class OrcNPC : EnemyAI
{
    [Header("Cài đặt NPC Orc")]
    [Tooltip("Nếu tích vào, Orc sẽ đánh nhau như quái thường. Nếu bỏ tích, Orc chỉ đi dạo.")]
    public bool isHostile = false; // Biến này để sau này bạn chuyển nó thành quái dễ dàng

    [Tooltip("Orc có đang nói chuyện với Player không?")]
    public bool isInteracting = false;

    protected override void Start()
    {
        base.Start(); // Gọi hàm Start của cha để lấy RB, Animator, v.v.

        // Tinh chỉnh chỉ số riêng cho Orc (nếu cần)
        moveSpeed = 1.5f;   // Đi chậm rãi kiểu NPC
        patrolRadius = 5f;  // Vùng đi dạo rộng hơn chút
        waitTime = 3f;      // Đứng chơi lâu hơn
    }

    protected override void FixedUpdate()
    {
        if (isDead) return;

        // Nếu đang nói chuyện làm quest thì đứng im, không đi đâu cả
        if (isInteracting)
        {
            StopMoving();
            return;
        }

        // --- LOGIC PHÂN LOẠI ---

        if (isHostile)
        {
            // Nếu là kẻ địch: Dùng logic cũ của cha (Đuổi theo + Đánh)
            base.FixedUpdate();
        }
        else
        {
            // Nếu là NPC hiền lành: CHỈ GỌI HÀM TUẦN TRA
            // Bỏ qua đoạn check khoảng cách (distance < chaseRange)
            Patroling();
        }
    }

    // --- HÀM HỖ TRỢ HỆ THỐNG QUEST ---

    // Gọi hàm này khi Player bấm nút nói chuyện
    public void StartConversation()
    {
        isInteracting = true;
        // Quay mặt về phía Player cho lịch sự
        if (playerTransform != null)
        {
            FlipSprite(playerTransform.position);
        }
    }

    // Gọi hàm này khi hội thoại kết thúc
    public void EndConversation()
    {
        isInteracting = false;
        // Orc sẽ tiếp tục đi tuần tra ngay lập tức
    }
}