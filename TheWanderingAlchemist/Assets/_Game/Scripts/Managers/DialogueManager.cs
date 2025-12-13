using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic; // Cần cái này để dùng Queue

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI Components")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI contentText;

    // Hàng đợi để chứa các câu thoại
    private Queue<string> sentences;

    // Biến kiểm tra trạng thái mở/đóng
    public bool IsDialogueActive { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        sentences = new Queue<string>();
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    // Hàm Bắt đầu hội thoại (Nhận vào 1 danh sách các câu)
    public void StartDialogue(string name, string[] lines)
    {
        IsDialogueActive = true; // Đánh dấu mở
        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        nameText.text = name;
        sentences.Clear(); // Xóa sạch các câu cũ (nếu có)

        // Nạp từng câu vào hàng đợi
        foreach (string line in lines)
        {
            sentences.Enqueue(line);
        }

        DisplayNextSentence(); // Hiện câu đầu tiên luôn
    }

    // Hàm hiện câu tiếp theo (Gắn vào nút bấm hoặc phím E)
    public void DisplayNextSentence()
    {
        // Nếu hết câu rồi thì tắt bảng
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        // Lấy câu tiếp theo ra khỏi hàng đợi và hiển thị
        string sentence = sentences.Dequeue();
        contentText.text = sentence;
    }

    public void EndDialogue()
    {
        IsDialogueActive = false; // Đánh dấu đóng
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }
}