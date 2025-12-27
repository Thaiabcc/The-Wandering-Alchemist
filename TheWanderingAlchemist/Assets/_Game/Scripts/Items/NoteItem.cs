using UnityEngine;

public class NoteItem : MonoBehaviour
{
    [Header("Nội dung tờ giấy này")]
    public string noteTitle = "Tiêu đề giấy";

    [TextArea(5, 10)] // Dòng này giúp ô nhập liệu trong Unity to ra để viết văn
    public string noteContent = "Nhập nội dung vào đây...";

    private bool isPlayerNearby = false;
    // Biến để tránh việc vừa bật lên lại tắt ngay lập tức
    private bool justOpened = false;

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            // Kiểm tra xem UI có đang mở không để tránh xung đột
            // Nếu muốn logic đơn giản: Cứ bấm E là hiện
            NoteUIManager.Instance.ShowNote(noteTitle, noteContent);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("Nhấn E để đọc");
            // Bạn có thể hiện một icon "Bàn tay" hoặc chữ "E" nhỏ trên đầu vật phẩm ở đây
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            // Nếu đi ra xa thì tự đóng bảng luôn cho tiện
            NoteUIManager.Instance.CloseNote();
        }
    }
}