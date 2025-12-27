using UnityEngine;
using TMPro;

public class NoteUIManager : MonoBehaviour
{
    public static NoteUIManager Instance { get; private set; }

    [Header("UI Components")]
    public GameObject notePanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI contentText;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        if (notePanel != null) notePanel.SetActive(false);
    }

    public void ShowNote(string title, string content)
    {
        notePanel.SetActive(true);
        titleText.text = title;
        contentText.text = content;

        // Dừng game để quái không đánh lúc đang đọc
        Time.timeScale = 0f;
    }

    // Hàm này để gọi khi bấm nút tắt
    public void CloseNote()
    {
        notePanel.SetActive(false);

        // QUAN TRỌNG: Phải trả lại thời gian thì game mới chạy tiếp
        Time.timeScale = 1f;
    }

    private void Update()
    {
        // Chỉ kiểm tra nút tắt khi bảng đang mở
        if (notePanel.activeSelf)
        {
            // Bấm ESC hoặc bấm E lần nữa để tắt
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
            {
                CloseNote();
            }
        }
    }
}