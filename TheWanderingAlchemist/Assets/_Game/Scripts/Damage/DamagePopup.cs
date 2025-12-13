using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;

    // --- CẤU HÌNH GIA VỊ (Chỉnh trong Inspector) ---
    [SerializeField] private float moveSpeedY = 20f;   // Lực bắn lên cao
    [SerializeField] private float moveSpeedX = 10f;   // Độ tản ra 2 bên (Trái/Phải)
    [SerializeField] private float disappearSpeed = 3f; // Tốc độ mờ dần

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    // Thêm tham số isCriticalHit vào hàm Setup
    public void Setup(int damageAmount, bool isCriticalHit)
    {
        textMesh.text = damageAmount.ToString();
        disappearTimer = 1f;

        if (!isCriticalHit)
        {
            // --- TRƯỜNG HỢP: ĐÁNH THƯỜNG ---
            textMesh.fontSize = 5;          // Cỡ chữ vừa phải
            textMesh.color = Color.yellow;   // Màu vàng cổ điển
            textColor = Color.yellow;

            // Hướng bay: Bình thường
            moveVector = new Vector3(Random.Range(-moveSpeedX, moveSpeedX), moveSpeedY);
            transform.localScale = Vector3.one;
        }
        else
        {
            // --- TRƯỜNG HỢP: BẠO KÍCH (CRITICAL) ---
            textMesh.fontSize = 6;          // Cỡ chữ TO
            textMesh.color = Color.red;      // Màu ĐỎ rực
            textColor = Color.red;

            // Hướng bay: Mạnh hơn, vọt cao hơn
            moveVector = new Vector3(Random.Range(-moveSpeedX, moveSpeedX) * 1.5f, moveSpeedY * 1.5f);

            // Hiệu ứng scale: To hơn 50%
            transform.localScale = Vector3.one * 1.5f;
        }
    }

    private void Update()
    {
        // --- BƯỚC 2: XỬ LÝ DI CHUYỂN (VẬT LÝ GIẢ) ---

        // Di chuyển theo vector đã tính
        transform.position += moveVector * Time.deltaTime;

        // "Ma sát": Giảm dần tốc độ di chuyển để tạo cảm giác "nảy ra rồi chậm lại"
        // Số càng lớn (8f) thì dừng lại càng nhanh
        moveVector -= moveVector * 8f * Time.deltaTime;

        // Nếu tốc độ bay lên gần hết, cho nó rơi nhẹ xuống (hiệu ứng trọng lực - tùy chọn)
        if (disappearTimer > 1f * 0.5f)
        {
            // Nửa đầu thời gian: Giữ nguyên size
        }
        else
        {
            // Nửa sau thời gian: Có thể cho rơi nhẹ nếu muốn (ở đây tôi giữ lơ lửng cho dễ đọc)
        }

        // --- BƯỚC 3: XỬ LÝ BIẾN MẤT ---
        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            // Bắt đầu giảm Alpha (độ trong suốt)
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;

            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}